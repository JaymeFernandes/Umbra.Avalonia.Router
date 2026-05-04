# Umbra.Avalonia.Router

Cross-platform routing library for AvaloniaUI.

> This project **started as a fork** of `Sandreas.Avalonia.SimpleRouter`,
> but has since evolved — the API and internals are now significantly different.

---

## New: NavigationContext + IRoutePage

Optional modern API built around:

- `NavigationContext`
- strongly-typed route pages via `IRoutePage` (ViewModel)
- async navigation hooks
- natural integration with `IServiceCollection`
- container-agnostic (DryIoc / Autofac / Microsoft DI / etc)

---

## Install

```bash
dotnet add package Umbra.Avalonia.Router
````

---

## Features

* lightweight
* IoC / DI friendly
* automatic parameter injection into ViewModels
* navigation history (Back / Forward)
* extensible
* **new**: NavigationContext + IRoutePage API

---

## New API Overview

### Base Page

```csharp
public abstract class ViewModelBasePage : PageRouterBase
{
    public override Task OnNavigatedToAsync(CancellationToken ctx)
      => Task.CompletedTask;
}
```

### ViewModels

```csharp
public partial class HomeViewModel : ViewModelBasePage
{
    private readonly RouterHistory<ViewModelBasePage> _router;

    public HomeViewModel(RouterHistory<ViewModelBasePage> router)
        => _router = router;

    [RelayCommand]
    public void NavigateToStore()
        => _router.Navigate("store?query=games&page=1&size=20",
                            "My App Store",
                            new SessionParams(1, DateTime.UtcNow));
}

public partial class StoreViewModel : ViewModelBasePage
{
    [ObservableProperty] private string _query;
    [ObservableProperty] private int _page;
    [ObservableProperty] private int _size;

    private SessionParams _body;
    private readonly RouterHistory<ViewModelBasePage> _router;

    public StoreViewModel(RouterHistory<ViewModelBasePage> router)
        => _router = router;

    public override Task OnNavigatedToAsync(CancellationToken ctx)
    {
        Query = Context.Query.TryGetValue("query", out string query) 
            ? query : ""; 

        Page = Context.Query.TryGetValueNumber("page", out int page) 
            ? page : 0; 
            
        Size = Context.Query.TryGetValueNumber("size", out int size) 
            ? size : 0;

        if (Context.Body.TryGetValue(out ParamsBody? body))
            Date = body.Date.ToString("hh:mm:ss");

        return Task.CompletedTask;
    }

    [RelayCommand]
    public void NavigateToHome()
        => _router.Navigate("home", "My App Home");
}

public record SessionParams(int Id, DateTime Date);
```

### Register Routes

```csharp
services.AddAvaloniaRouter<ViewModelBasePage>(options =>
{
    options.Register<HomePage,  HomeViewModel>("home");
    options.Register<StorePage, StoreViewModel>("store");
    options.Register<Error404Page, Error404ViewModel>("**");
});
```

### MainWindow Integration

```csharp
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private Control _content;
    [ObservableProperty] private string _title;

    public MainWindowViewModel()
    {
        this.RouterHistory.TitleChanged += (title) =>
        {
            Title = string.IsNullOrWhiteSpace(title) ? "Sample Router" : $"Sample Router - {title}";
        };
        this.RouterHistory.PageChanged += NavigateEvent;
        
        this.RouterHistory.GoTo("home", "Home");
    }
    
    private void NavigateEvent(object? sender, NavigationResultEventArgs<Control> e)
    {
        Content = e.Page;
    }
}
```

XAML:

```xml
<StackPanel Grid.Column="1" Margin="10">
    <ContentControl Content="{Binding Content}"/>
</StackPanel>
```

## DryIoc Example

```csharp
public static IContainer Container { get; set; }

private void ConfigureServices()
{
    var services = new ServiceCollection();

    services.AddAvaloniaRouter<ViewModelBasePage>(options =>
    {
        options.HistorySize = 5;

        options.Register<HomePage,  HomeViewModel>("home");
        options.Register<StorePage, StoreViewModel>("store");
        options.Register<Error404Page, Error404ViewModel>("**");
    });

    var dryIoc = new Container()
        .WithDependencyInjectionAdapter(services)
        .Populate(services);

    Container = dryIoc;
}
```

---

## Roadmap

* unify object & query data flow inside NavigationContext
* optional query-style param helpers

