using System.Threading.Tasks;
using Avalonia.SimpleRouter.Context;
using Avalonia.SimpleRouter.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RouterSample.ViewModels.Shared;

public class PageViewModelBase : ObservableObject, IRoutePage
{
    public virtual void OnNavigatedTo(NavigationContext context) { }

    public virtual Task OnNavigatedToAsync(NavigationContext context)
        => Task.CompletedTask;
    
    public void Dispose() { }
}