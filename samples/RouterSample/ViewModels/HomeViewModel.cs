using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RouterSample.ViewModels.Example;
using RouterSample.ViewModels.Shared;
using Umbra.Avalonia.Router;

namespace RouterSample.ViewModels;

public partial class HomeViewModel : PageViewModelBase
{
    [ObservableProperty] private string _name;
    private RouterHistory<PageViewModelBase> _router;

    public HomeViewModel(RouterHistory<PageViewModelBase> router)
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