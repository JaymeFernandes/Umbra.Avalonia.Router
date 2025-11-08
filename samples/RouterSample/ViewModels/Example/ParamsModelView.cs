using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RouterSample.ViewModels.Shared;
using Umbra.Avalonia.Router;
using Umbra.Avalonia.Router.Context;

namespace RouterSample.ViewModels.Example;

public partial class ParamsModelView : PageViewModelBase
{
    public string UserName { get; private set; } = string.Empty;
    
    
    private readonly RouterHistory<PageViewModelBase>  _history;

    [ObservableProperty] private int _page;
    [ObservableProperty] private string _date;
    

    public ParamsModelView(RouterHistory<PageViewModelBase> history)
    {
        _history = history; 
    }

    public override void OnNavigatedTo(NavigationContext context)
    {
        UserName = context.Query.GetValue("name");
        
        Page = context.Query.TryGetValueNumber("page", out var page)
            ? page : 0;

        if (context.Body.Value is ParamsBody body)
            Date = body.Date.ToString("hh:mm:ss");
           
    }

    [RelayCommand]
    public void GotoNextPage()
    {
        var nextPage = Page + 1;
        
        _history.Navigate(
            url: $"example/params?name={UserName}&page={nextPage}", 
            title: $"Page - {nextPage}", 
            body: new ParamsBody(DateTime.UtcNow));
    }
    
    [RelayCommand]
    public void GotoPreviousPage()
    {
        if (Page <= 0) return;
        var prevPage = Page - 1;
        
        _history.Navigate(
            url: $"example/params?name={UserName}&page={prevPage}", 
            title: $"Page - {prevPage}", 
            body: new ParamsBody(DateTime.UtcNow));
    }
}

public record ParamsBody(DateTime Date);