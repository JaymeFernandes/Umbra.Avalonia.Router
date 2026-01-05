using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RouterSample.ViewModels.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;
using Umbra.Avalonia.Router;

namespace RouterSample.ViewModels.Example;

public partial class ParamsModelView : PageViewModelBase
{
    private readonly RouterHistory<PageViewModelBase> _history;

    [ObservableProperty] private string? _date;

    [ObservableProperty] private int _page;

    [ObservableProperty] private string? _username;

    public ParamsModelView(RouterHistory<PageViewModelBase> history)
    {
        _history = history;
    }

    [RelayCommand]
    public void GotoNextPage()
    {
        var nextPage = Page + 1;

        _history.Navigate(
            url: $"example/params?name={Username}&page={nextPage}",
            title: $"Page - {nextPage}",
            body: new ParamsBody(DateTime.UtcNow));
    }

    [RelayCommand]
    public void GotoPreviousPage()
    {
        if (Page <= 0) return;
        var prevPage = Page - 1;

        _history.Navigate(
            url: $"example/params?name={Username}&page={prevPage}",
            title: $"Page - {prevPage}",
            body: new ParamsBody(DateTime.UtcNow));
    }

    public override async Task OnNavigatedToAsync(CancellationToken ctx)
    {
        Username = Context.Query.GetValue("name");

        Page = Context.Query.TryGetValueNumber("page", out var page)
            ? page : 0;

        if (Context.Body.Value is ParamsBody body)
            Date = body.Date.ToString("hh:mm:ss");
    }
}

public record ParamsBody(DateTime Date);
