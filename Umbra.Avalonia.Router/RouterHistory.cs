using Umbra.Avalonia.Router.Configuration;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;

namespace Umbra.Avalonia.Router;

public class RouterHistory<TViewModelBase> : Router<TViewModelBase> where TViewModelBase : class, IRoutePage
{
    private int _historyIndex = -1;
    
    private List<NavigationContext> _history = new();
    private List<string> _titles = new();
    
    private readonly uint _historyMaxSize = 10;
    private string CurrentRouter = "";

    public event Action<string>? TitleChanged;
    
    public bool HasNext => _history.Count > 0 && _historyIndex < _history.Count - 1;
    public bool HasPrev => _historyIndex > -1;
    
    public IReadOnlyCollection<NavigationContext> History => _history.AsReadOnly();

    public RouterHistory(IServiceProvider serviceProvider, RouterConfig config) : base(serviceProvider, config)
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
        
        TitleChanged.Invoke(title);
        
        return viewModel;
    }
    
    public TViewModelBase? Back() => HasPrev ? Go(-1) : default;
    
    public TViewModelBase? Forward() => HasNext ? Go(1) : default;
    
    public override TViewModelBase Navigate(string url, object body = null)
    {
        var destination = ResolveViewModel(url, body);

        if (CurrentRouter == url)
            return destination;
        
        CurrentViewModel = destination;
        CurrentRouter = url;
        if(TitleChanged != null)
            TitleChanged.Invoke("");
        
        Push(new NavigationContext(url, body, config.Scheme, config.AppName));
        return destination;
    }
    
    public TViewModelBase Navigate(string url, string title, object body = null)
    {
        var destination = ResolveViewModel(url, body);

        if (CurrentRouter == url)
            return destination;
        
        CurrentViewModel = destination;
        CurrentRouter = url;
        
        TitleChanged.Invoke(title);
        
        Push(new NavigationContext(url, body, config.Scheme, config.AppName));
        return destination;
    }
}