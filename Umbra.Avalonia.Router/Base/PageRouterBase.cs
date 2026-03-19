using CommunityToolkit.Mvvm.ComponentModel;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Base
{
    public enum RouterStatus
    {
        None,

        Navigating,

        Completed,

        Disposed
    }

    public abstract partial class PageRouterBase : ObservableObject, IRoutePage
    {
        private CancellationTokenSource? _cts;

        private bool _isInitialize = false;

        private NavigationContext _navigationContext = default!;

        [ObservableProperty] 
        private RouterStatus _status = RouterStatus.None;

        public NavigationContext Context => _navigationContext;

        private CancellationToken _ctx => _cts?.Token ?? CancellationToken.None;

        public async Task CancelNavigation()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                await OnCancelationNavigatedToAsync();

                Status = RouterStatus.Disposed;
            }
        }

        public virtual void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();

            _cts = null;
            _navigationContext = null!;
        }

        public async Task InitializeAsync(NavigationContext context)
        {
            if (_isInitialize)
                return;

            _cts?.Cancel();
            _cts?.Dispose();

            _cts = new CancellationTokenSource();

            _navigationContext = context;

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

        protected virtual Task OnCancelationNavigatedToAsync()
        {
            Dispose();

            return Task.CompletedTask;
        }

        protected virtual Task OnCompletedAsync()
                    => Task.CompletedTask;

        protected virtual Task<bool> OnNavigationErrorAsync(Exception ex)
            => Task.FromResult(false);
    }
}
