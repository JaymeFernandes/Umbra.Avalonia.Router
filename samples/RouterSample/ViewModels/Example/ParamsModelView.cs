using System;
using System.Threading.Tasks;
using Avalonia.SimpleRouter.Context;
using CommunityToolkit.Mvvm.ComponentModel;
using RouterSample.ViewModels.Shared;

namespace RouterSample.ViewModels.Example;

public partial class ParamsModelView : PageViewModelBase
{
    public string UserName { get; private set; } = string.Empty;

    [ObservableProperty] private string _date;
    
    public override void OnNavigatedTo(NavigationContext context)
    {
        UserName = context.Query.GetValue("name");

        if (context.Body.Value is ParamsBody body)
           Date = body.Date.ToString("hh:mm:ss");
    }
}

public record ParamsBody(DateTime Date);