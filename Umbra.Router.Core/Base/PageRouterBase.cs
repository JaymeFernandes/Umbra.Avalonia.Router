using CommunityToolkit.Mvvm.ComponentModel;
using Umbra.Router.Core.Events;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work;
using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Base;

public enum RouterStatus
{
    None,

    Navigating,

    Completed,

    Disposed
}

public abstract class PageRouterBase : ObservableObject, IRoutePage
{
    private CancellationTokenSource? _cts;

    private bool _isInitialize;

    public RouterStatus Status { get; private set; } = RouterStatus.None;

    private CancellationToken _ctx => _cts?.Token ?? CancellationToken.None;

    public NavigationContext Context { get; private set; } = default!;

    public virtual void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        
        Context = null!;
        Status = RouterStatus.Disposed;
    }

    public async Task InitializeAsync(NavigationContext context)
    {
        if (_isInitialize)
            return;

        _cts?.Cancel();
        _cts?.Dispose();

        _cts = new CancellationTokenSource();

        Context = context;
        Status = RouterStatus.Navigating;

        try
        {
            await OnNavigatedToAsync(_ctx);

            if (!_cts.IsCancellationRequested)
            {
                Status = RouterStatus.Completed;
                await OnCompletedAsync();
                _isInitialize = true;
            }
            else
            {
                await OnCancellationAsync();
            }
        }
        catch (OperationCanceledException)
        {
            Status = RouterStatus.Disposed;

            await OnCancellationAsync();
        }
        catch (Exception ex)
        {
            if (await OnNavigationErrorAsync(ex))
                return;

            throw;
        }
    }
    
    public abstract Task OnNavigatedToAsync(CancellationToken ctx);

    public virtual Task OnBlockedNavigationAsync(NavigationContext context, NavigationBlocked blocked) 
        => Task.CompletedTask;

    public virtual Task OnDenyNavigationAsync(NavigationContext context)
        => Task.CompletedTask;
    
    public Task CancelNavigation()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            Status = RouterStatus.Disposed;
        }

        return Task.CompletedTask;
    }
    
    protected virtual Task OnCancellationAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }
    protected virtual Task OnCompletedAsync() => Task.CompletedTask;
    protected virtual Task<bool> OnNavigationErrorAsync(Exception ex) => Task.FromResult(false);
}