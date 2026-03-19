using System;
using CommunityToolkit.Mvvm.ComponentModel;
using DryIoc;
using Umbra.Router.Avalonia;

namespace RouterSample.ViewModels.Shared;

public class ViewModelBase : ObservableObject
{
    protected IServiceProvider ServiceProvider = App.Container.Resolve<IServiceProvider>();
    protected RouterHistory<PageViewModelBase> RouterHistory = App.Container.Resolve<RouterHistory<PageViewModelBase>>();
}