using System;
using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RouterSample.ViewModels.Example;
using RouterSample.ViewModels.Shared;

namespace RouterSample.ViewModels;

public partial class HomeViewModel : PageViewModelBase
{
    [ObservableProperty] private string _name;
    private RouterHistoryManager<PageViewModelBase> _router;

    public HomeViewModel(RouterHistoryManager<PageViewModelBase> router)
    {
        _router = router;
    }

    [RelayCommand]
    public void Go()
    {
        if (!string.IsNullOrWhiteSpace(_name))
            _router.GoTo($"example/params?name={_name}", new ParamsBody(DateTime.UtcNow));
    }
}