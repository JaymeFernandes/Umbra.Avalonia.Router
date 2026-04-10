using CommunityToolkit.Mvvm.ComponentModel;
using Umbra.Router.Core.Interfaces;
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

            if (_cts != null && !_cts.IsCancellationRequested)
            {
                Status = RouterStatus.Completed;

                await OnCompletedAsync();

                _isInitialize = true;
            }
            else
            {
                Status = RouterStatus.Disposed;
                await OnCancelationNavigatedToAsync();
            }
        }
        catch (OperationCanceledException)
        {
            Status = RouterStatus.Disposed;

            await OnCancelationNavigatedToAsync();
        }
        catch (Exception ex)
        {
            if (await OnNavigationErrorAsync(ex))
                return;

            throw;
        }
    }

    public abstract Task OnNavigatedToAsync(CancellationToken ctx);

    public async Task CancelNavigation()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            await OnCancelationNavigatedToAsync();

            Status = RouterStatus.Disposed;
        }
    }

    protected virtual Task OnCancelationNavigatedToAsync()
    {
        Dispose();

        return Task.CompletedTask;
    }

    protected virtual Task OnCompletedAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task<bool> OnNavigationErrorAsync(Exception ex)
    {
        return Task.FromResult(false);
    }
}