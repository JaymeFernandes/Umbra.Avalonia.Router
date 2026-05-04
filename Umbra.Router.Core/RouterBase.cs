using Microsoft.Extensions.DependencyInjection;
using Umbra.Router.Core.Events;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Services;
using Umbra.Router.Core.Work;
using Umbra.Router.Core.Work.Navigation;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core;

public class RouterBase<TViewModelBase, TView> where TViewModelBase : class, IRoutePage where TView : class
{
    private readonly GuardServices<TViewModelBase> _guards;

    private readonly IRouterResolver<TViewModelBase> _resolver;

    private readonly IServiceProvider _serviceProvider;
    private RouterResult? _current;

    private TViewModelBase? _currentViewModel;
    
    public event EventHandler<NavigationResultEventArgs<TView>>? PageChanged;

    public RouterBase(IServiceProvider serviceProvider, GuardServices<TViewModelBase> guards)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        _guards = guards;

        _resolver = serviceProvider.GetRequiredService<IRouterResolver<TViewModelBase>>();
    }

    protected RouterResult? Current
    {
        get => _current;

        set
        {
            _current = value;
            CurrentViewModel = value.ViewModel as TViewModelBase;
        }
    }

    protected TViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;

        set
        {
            if (_currentViewModel == value)
                return;

            if (_currentViewModel != null)
                _currentViewModel.Dispose();

            _currentViewModel = value;

            if (_current.View is null)
                throw new Exception($"No view registered for {_currentViewModel.GetType().Name}");

            var control = ActivatorUtilities.CreateInstance(_serviceProvider, _current.View) as TView;

            ConfigureTView(ref control, _currentViewModel);

            PageChanged?.Invoke(this, new NavigationResultEventArgs<TView>(control, _currentViewModel.Context, true));
        }
    }

    public virtual TViewModelBase? Navigate(string url, object? body = null)
    {
        var routerResult = ResolveViewModelAsync(url, body).Result;

        if (routerResult is null)
            return Current?.ViewModel as TViewModelBase;
        
        
        Current = routerResult;

        return routerResult.ViewModel as TViewModelBase;
    }

    public virtual async Task<TViewModelBase?> NavigateAsync(string url, object? body = null)
    {
        var routerResult = await ResolveViewModelAsync(url, body);

        if (routerResult is null)
            return null;
        
        Current = routerResult;
        
        return routerResult.ViewModel as TViewModelBase;
    }
    
    protected async Task<RouterResult?> ResolveViewModelAsync(string url, object? body, int depth = 0)
        => await ResolveViewModelAsync(new RouteSnapshot(url, body), depth);

    protected async Task<RouterResult?> ResolveViewModelAsync(RouteSnapshot snapshot, int depth = 0)
    {
        var canMatch = await _guards.CanMatchAsync(snapshot);
        
        if(canMatch is null)
            return null;
        
        if (canMatch.Decision != GuardDecision.Allow)
        {
            if (canMatch.Decision == GuardDecision.Redirect)
                if(depth <= 5)
                    return await ResolveViewModelAsync(canMatch.RedirectTo!, null, depth + 1);
            
            await HandlerGuardResult(canMatch, false);

            return null;
        }
        
        var canDeactivate = await _guards.CanDeactivateAsync(snapshot);
        
        if(canDeactivate is null)
            return null;

        if (canDeactivate.Decision != GuardDecision.Allow)
        {
            if (canDeactivate.Decision == GuardDecision.Redirect)
                throw new InvalidOperationException("Redirect not allowed in CanDeactivate");
            
            await HandlerGuardResult(canDeactivate, true);
            
            return null;
        }
            

        if (string.IsNullOrWhiteSpace(snapshot.Path))
            throw new ArgumentException("Route cannot be null or empty.", nameof(snapshot.Path));

        var vm = _resolver.Resolve(snapshot) ??
                 throw new InvalidOperationException($"Route '{snapshot.Path}' could not be resolved.");

        return vm;
    }

    protected virtual void ConfigureTView(ref TView? view, TViewModelBase viewModel)
    {
    }

    public async Task HandlerGuardResult(GuardResult result, bool isDeactivate)
    {
        if (Current != null)
        {
            if (!isDeactivate)
            {
                if (result.BlockedData != null && result.BlockedData.Reason.Equals("NotFound"))
                    await Current.ViewModel.OnBlockedNavigationAsync(result.Context, result.BlockedData);
                else
                    await Current.ViewModel.OnDenyNavigationAsync(result.Context);
                
                return;
            }
            
            if (result.BlockedData != null)
                await Current.ViewModel.OnBlockedNavigationAsync(result.Context, result.BlockedData);
        }
    }
}