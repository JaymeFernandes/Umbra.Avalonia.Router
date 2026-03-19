using Avalonia.Controls;
using Umbra.Router.Core;
using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Services;

namespace Umbra.Router.Avalonia;

public class RouterHistory<TViewModelBase> : RouterHistoryBase<TViewModelBase, Control> where TViewModelBase : class, IRoutePage
{
    public RouterHistory(IServiceProvider serviceProvider, RouterConfig<TViewModelBase> config, GuardServices<TViewModelBase> guards) 
        : base(serviceProvider, config, guards) { }

    protected override void ConfigureTView(ref Control? view, TViewModelBase viewModel)
    {
        view.DataContext = viewModel;
    }
}