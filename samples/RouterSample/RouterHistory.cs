using System;
using Avalonia.Controls;
using Umbra.Router.Core;
using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Services;

namespace RouterSample;

public class RouterHistory<TViewModel> : RouterHistoryBase<TViewModel, Control>
    where TViewModel : class, IRoutePage
{
    public RouterHistory(IServiceProvider serviceProvider, RouterConfig<TViewModel> config, GuardServices<TViewModel> guards) : 
        base(serviceProvider, config, guards)
    {
        
    }

    protected override void ConfigureTView(ref Control? view, TViewModel viewModel)
    {
        view.DataContext = viewModel;
        base.ConfigureTView(ref view, viewModel);
    }
}