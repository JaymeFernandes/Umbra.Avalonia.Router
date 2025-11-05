using Avalonia.SimpleRouter.Configuration;
using Avalonia.SimpleRouter.Context;
using Avalonia.SimpleRouter.Interfaces;

namespace Avalonia.SimpleRouter;

public class RouterHistoryManager<TViewModelBase> : Router<TViewModelBase> where TViewModelBase : class, IRoutePage
{
    private int _historyIndex = -1;
    
    private List<NavigationContext> _history = new();
    private List<string> _titles = new();
    
    private readonly uint _historyMaxSize = 10;
    private string CurrentRouter = "";

    public event Action<string>? CurrentTitleChanged;
    
    public bool HasNext => _history.Count > 0 && _historyIndex < _history.Count - 1;
    public bool HasPrev => _historyIndex > -1;
    
    public IReadOnlyCollection<NavigationContext> History => _history.AsReadOnly();

    public RouterHistoryManager(IServiceProvider serviceProvider, RouterConfig config) : base(serviceProvider, config)
    {
        _historyMaxSize = (uint)config.HistorySize;
    }
    
    public NavigationContext? GetHistoryItem(int offset, out string title)
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
    
    public void Push(NavigationContext item, string title = "")
    {
        if (HasNext)
        {
            _history = _history.Take(_historyIndex + 1).ToList();
            _titles =  _titles.Take(_historyIndex + 1).ToList();
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

        var viewModel = ResolveViewModel(context);

        _historyIndex += offset;
        CurrentViewModel = viewModel;
        
        CurrentTitleChanged.Invoke(title);
        
        return viewModel;
    }
    
    public TViewModelBase? Back() => HasPrev ? Go(-1) : default;
    
    public TViewModelBase? Forward() => HasNext ? Go(1) : default;
    
    public override TViewModelBase GoTo(string url, object body = null)
    {
        var destination = ResolveViewModel(url, body);

        if (CurrentRouter == url)
            return destination;
        
        CurrentViewModel = destination;
        CurrentRouter = url;
        if(CurrentTitleChanged != null)
            CurrentTitleChanged.Invoke("");
        
        Push(new NavigationContext(body, NormalizeUrl(url), url));
        return destination;
    }
    
    public TViewModelBase GoTo(string url, string title, object body = null)
    {
        var destination = ResolveViewModel(url, body);

        if (CurrentRouter == url)
            return destination;
        
        CurrentViewModel = destination;
        CurrentRouter = url;
        
        CurrentTitleChanged.Invoke(title);
        
        Push(new NavigationContext(body, NormalizeUrl(url), url), title);
        return destination;
    }
    
    private string  NormalizeUrl(string url)
        => url.StartsWith("app://", StringComparison.OrdinalIgnoreCase)
            ? url
            : "app://" + url;
}