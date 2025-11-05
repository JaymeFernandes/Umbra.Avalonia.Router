# Avalonia.SimpleRouter

Cross platform router library targeting AvaloniaUI.

## New: NavigationContext + IRoutePage support (experimental PR)

This PR introduces an optional new router API designed around:

- `NavigationContext`
- strongly typed pages via an `IRoutePage` ViewModel base
- async navigation hooks
- seamless integration with `IServiceCollection`
- DI independent (works with DryIoc / Autofac / etc)

This API is fully opt-in — the existing `HistoryRouter` is **not removed**.

---

# Install

📦 NuGet: https://nuget.org/packages/Sandreas.Avalonia.SimpleRouter

```bash
dotnet add package Sandreas.Avalonia.SimpleRouter
````

# Features

* No dependencies
* IoC / DI friendly
* ViewModel parameter injection
* Navigation history (Back + Forward)
* Extendable and flexible
* Nested routing (experimental)
* NEW: NavigationContext + IRoutePage API (optional)

# New API Overview (experimental)

### Base Page

```csharp
using Avalonia.Router.Context;
using Avalonia.Router.Interfaces;

public abstract class ViewModelBasePage : IRoutePage
{
    public void OnNavigatedTo(NavigationContext context) { }

    public Task OnNavigatedToAsync(NavigationContext context)
        => Task.CompletedTask;

    public virtual void Dispose() {}
}
```

### Page ViewModel

```csharp
public partial class HomeViewModel : ViewModelBasePage
{
}
```

### Register routes via DI

```csharp
services.AddAvaloniaRouter<ViewModelBasePage>(options =>
{
    options.Register<HomePage, HomeViewModel>("home");
});
```

### Navigate

```csharp
_router.Navigate("home");
_router.Navigate("store");
```

---

# DryIoc Example

```csharp
using DryIoc.Microsoft.DependencyInjection;

var builder = new ServiceCollection();
builder.AddRouterAvalonia();

var container = new Container()
    .WithDependencyInjectionAdapter(builder)
    .Populate(builder);
```

---

# Classic API (still available)

Existing `HistoryRouter<ViewModelBase>` API is **still supported** exactly as before.

No breaking changes.

Existing documentation remains valid.

---

## Roadmap

* unify NavigationContext data passing
* optional query style parameters
* nested routing integration into the new API
