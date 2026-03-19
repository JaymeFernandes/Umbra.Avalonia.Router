using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Services;
using Umbra.Router.Core.Work.Navigation;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core;

public class RouterHistoryBase<TViewModelBase, TView> : 
    RouterBase<TViewModelBase, TView> where TViewModelBase : class, IRoutePage
    where TView : class
{
    private readonly uint _historyMaxSize = 10;

    private List<RouteSnapshot> _history = new();

    private int _historyIndex = -1;

    private List<string> _titles = new();

    private UriContext CurrentRouter;

    public RouterHistoryBase(IServiceProvider serviceProvider, RouterConfig<TViewModelBase> config, GuardServices<TViewModelBase> guards) 
        : base(serviceProvider, guards)
    {
        _historyMaxSize = (uint)config.HistorySize;
    }

    public event Action<string>? TitleChanged;

    public bool HasNext => _history.Count > 0 && _historyIndex < _history.Count - 1;

    public bool HasPrev => _historyIndex > -1;

    public IReadOnlyCollection<RouteSnapshot> History => _history.AsReadOnly();

    public TViewModelBase? Back() => HasPrev ? Go(-1) : default;

    public TViewModelBase? Forward() => HasNext ? Go(1) : default;

    public RouteSnapshot? GetHistoryItem(int offset, out string title)
    {
        title = "";

        var newIndex = _historyIndex + offset;
        if (newIndex < 0 || newIndex > _history.Count - 1)
        {
            return default;
        }

        title = _titles.ElementAt(newIndex);
        return _history.ElementAt(newIndex);
    }

    public TViewModelBase? Go(int offset = 0)
    {
        if (offset == 0)
        {
            return default;
        }

        var context = GetHistoryItem(offset, out var title);
        if (context == null)
        {
            return default;
        }

        var routerResult = ResolveViewModel(context);

        _historyIndex += offset;
        Current = routerResult;

        TitleChanged?.Invoke(title);

        return routerResult.ViewModel as TViewModelBase;
    }

    public TViewModelBase Navigate(string[] keys, object? body = null, string title = "")
    {
        return Navigate(new RouteSnapshot(keys, body), title);
    }

    public TViewModelBase Navigate(string url, object? body = null, string title = "")
    {
        return Navigate(new RouteSnapshot(url, body), title);
    }
    
    private TViewModelBase Navigate(RouteSnapshot snapshot, string title = "")
    {
        var routerResult = ResolveViewModel(snapshot);

        if (CurrentRouter == snapshot)
            return routerResult.ViewModel as TViewModelBase;

        Current = routerResult;
        CurrentRouter = snapshot;

        TitleChanged?.Invoke(title);

        Push(snapshot, title);
        return routerResult.ViewModel as TViewModelBase;
    }

    private void Push(RouteSnapshot item, string title = "")
    {
        if (HasNext)
        {
            _history = _history.Take(_historyIndex + 1).ToList();
            _titles = _titles.Take(_historyIndex + 1).ToList();
        }
        _titles.Add(title);
        _history.Add(item);
        _historyIndex = _history.Count - 1;
        if (_history.Count > _historyMaxSize)
        {
            _titles.RemoveAt(0);
            _history.RemoveAt(0);

            _historyIndex--;
        }
    }
}
