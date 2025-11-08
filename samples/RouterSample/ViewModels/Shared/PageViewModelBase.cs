using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;

namespace RouterSample.ViewModels.Shared;

public class PageViewModelBase : ObservableObject, IRoutePage
{
    public virtual void OnNavigatedTo(NavigationContext context) { }

    public virtual Task OnNavigatedToAsync(NavigationContext context)
        => Task.CompletedTask;
    
    public void Dispose() { }
}