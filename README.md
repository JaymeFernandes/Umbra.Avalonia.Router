# Umbra.Avalonia.Router

Biblioteca de roteamento cross-platform para AvaloniaUI.

> Este projeto **foi originalmente um fork** de `Sandreas.Avalonia.SimpleRouter`, porém foi amplamente modificado e evoluiu para algo significativamente diferente.

---

## Novidade: NavigationContext + IRoutePage

Nova API opcional baseada em:

- `NavigationContext`
- páginas fortemente tipadas via `IRoutePage` (ViewModel)
- hooks de navegação assíncronos
- integração natural com `IServiceCollection`
- independente do contêiner (DryIoc / Autofac / Microsoft DI / etc)

---

## Instalação

📦 NuGet

```bash
dotnet add package Umbra.Avalonia.Router
````

---

## Features

* leve
* IoC / DI friendly
* injeção de parâmetros em ViewModels
* histórico (Back / Forward)
* extensível
* **novo**: NavigationContext + IRoutePage

---

## Overview da Nova API

### Base Page

```csharp
public abstract class ViewModelBasePage : IRoutePage
{
    public void OnNavigatedTo(NavigationContext context) { }

    public Task OnNavigatedToAsync(NavigationContext context)
        => Task.CompletedTask;

    public virtual void Dispose() {}
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

    public override Task OnNavigatedToAsync(NavigationContext context)
    {
        Query = context.Query.TryGetValue("query", out string query) ? query : ""; 
        Page = context.Query.TryGetValueNumber("page", out int page) ? page : 0; 
        Size = context.Query.TryGetValueNumber("size", out int size) ? size : 0;

        if (context.Body.Value is SessionParams body)
            _body = body;

        return Task.CompletedTask;
    }

    [RelayCommand]
    public void NavigateToHome()
        => _router.Navigate("home", "My App Home");
}

public record SessionParams(int Id, DateTime Date);
```

### Registrar rotas

```csharp
services.AddAvaloniaRouter<ViewModelBasePage>(options =>
{
    options.Register<HomePage,  HomeViewModel>("home");
    options.Register<StorePage, StoreViewModel>("store");
});
```

### Integração com a janela (MainWindow)

```csharp
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private Control _content;
    [ObservableProperty] private string _title;

    public MainWindowViewModel()
    {
        this.RouterHistory.TitleChanged += (title) =>
        {
            Title = string.IsNullOrWhiteSpace(title)
                ? "Sample Router"
                : $"Sample Router - {title}";
        };

        this.RouterHistory.PageChanged += (page) => Content = page;

        this.RouterHistory.GoTo("home", "Home");
    }
}
```

XAML:

```xml
<StackPanel Grid.Column="1" Margin="10">
    <ContentControl Content="{Binding Content}"/>
</StackPanel>
```

## DryIoc (exemplo)

```csharp
public static IContainer Container { get; set; }

private void ConfigureServices()
{
    var services = new ServiceCollection();

    services.AddAvaloniaRouter<ViewModelBasePage>(options =>
    {
        options.Register<HomePage,  HomeViewModel>("home");
        options.Register<StorePage, StoreViewModel>("store");

        // Configurando base da rota myapp://SampleRoutereApp/...
        options.Scheme = "myapp"; 
        options.AppName = "SampleRoutereApp"; 

        options.HistorySize = 5;
    });

    var dryIoc = new Container()
        .WithDependencyInjectionAdapter(services)
        .Populate(services);

    Container = dryIoc;
}
```

---

## Roadmap

* unificar passagem de dados no NavigationContext
* parâmetros estilo query opcionais