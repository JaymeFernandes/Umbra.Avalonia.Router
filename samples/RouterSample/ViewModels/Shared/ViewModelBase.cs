using System;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using DryIoc;

namespace RouterSample.ViewModels.Shared;

public class ViewModelBase : ObservableObject
{
    protected IServiceProvider ServiceProvider = App.Container.Resolve<IServiceProvider>();
    protected RouterHistoryManager<PageViewModelBase> RouterHistoryManager = App.Container.Resolve<RouterHistoryManager<PageViewModelBase>>();
}