using Microsoft.Extensions.DependencyInjection;
using Umbra.Router.Core.Events;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Services;
using Umbra.Router.Core.Work;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core;

public class RouterBase<TViewModelBase, TView> where TViewModelBase : class, IRoutePage where TView : class
{
    private readonly GuardServices<TViewModelBase> _guards;

    private readonly IRouterResolver<TViewModelBase> _resolver;

    private readonly IServiceProvider _serviceProvider;

    private TViewModelBase? _currentViewModel = default!;
    private RouterResult? _current = null;

    public RouterBase(IServiceProvider serviceProvider, GuardServices<TViewModelBase> guards)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        
        _guards = guards;
    
        _resolver = serviceProvider.GetRequiredService<IRouterResolver<TViewModelBase>>();
    }

    public event EventHandler<NavigationResultEventArgs<TView>>? PageChanged;

    protected RouterResult? Current
    {
        get => _current;

        set
        {
            _current = value;
            CurrentViewModel = value.ViewModel as TViewModelBase;
        }
    }
    
    private TViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;

        set
        {
            if (_currentViewModel == value) 
                return;
            
            if(_currentViewModel != null)
                _currentViewModel.Dispose();
            
            _currentViewModel = value;
            
            if (_current.View is null)
                throw new Exception($"No view registered for {_currentViewModel.GetType().Name}");
            
            var control = ActivatorUtilities.CreateInstance(_serviceProvider, _current.View) as TView;
            
            ConfigureTView(ref control, _currentViewModel);
            
            PageChanged?.Invoke(this, new NavigationResultEventArgs<TView>(control, _currentViewModel.Context, true));
        }
    }

    public virtual TViewModelBase Navigate(string url, object? body = null)
    {
        var routerResult = ResolveViewModel(url, body);
        Current = routerResult;
        
        return routerResult.ViewModel as TViewModelBase;
    }

    protected RouterResult ResolveViewModel(string url, object? body)
    {
        var uri = new RouteSnapshot(url, body);
        
        var guardResult = _guards.CanNavigateAsync(uri).Result;
        
        if(guardResult.Action == GuardAction.Cancel)
            return Current;
        
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Route cannot be null or empty.", nameof(url));

        var vm = _resolver.Resolve(url, body) ?? throw new InvalidOperationException($"Route '{url}' could not be resolved.");

        return vm;
    }

    protected RouterResult ResolveViewModel(RouteSnapshot snapshot)
    {
        var guardResult = _guards.CanNavigateAsync(snapshot).Result;
        
        if(guardResult.Action == GuardAction.Cancel)
            return Current;
        
        var vm = _resolver.Resolve(snapshot) 
                 ?? throw new InvalidOperationException($"Route '{snapshot.Raw}' could not be resolved.");

        return vm;
    }

    protected virtual void ConfigureTView(ref TView? view, TViewModelBase viewModel) { }
}
