
# 📦 Umbra.Router.Core

A lightweight, framework-agnostic routing system for .NET UI applications.

Works with:

* Avalonia
* WPF
* WinUI / Uno
* MAUI
* WinForms (sim, até lá se você quiser sofrer um pouco 😄)

---

# ✨ Overview

Umbra Router was designed to provide a **clean navigation pipeline with IoC-first resolution, guards, parameters, and view-model binding**, without locking you into a specific UI framework.

---

# ⚡ Core Idea

The routing pipeline follows this strict order:

```
URL → Resolver → Guard → ViewModel → View → UI
```

### Why this order?

Performance and predictability.

* **Resolver is cheap (types only)**
* Guards run BEFORE ViewModel creation
* ViewModel is only created if navigation is allowed
* View is only instantiated at the end

This avoids unnecessary allocations and initialization work.

---

# 🧠 Key Concepts

## 1. Route Registration

Routes are registered via `RoutesBuilder`.

### Simple registration

```csharp
x.Register<HomePage, HomeViewModel>("home");
```

### Angular-style nested routes

```csharp
x.UseAngularStyleRoutes(new RoutesAngularStyle
{
    new RouteAngularStyle
    {
        Path = "sub",
        Children =
        {
            new RouteAngularStyle
            {
                Path = "first",
                Component = typeof(FirstSubPage),
                ViewModel = typeof(FirstSubViewModel)
            }
        }
    }
});
```

---

## 2. Title System (Important)

Each route can define a title template:

```csharp
SetTitle("Params {0}")
```

Then navigation replaces `{0}` dynamically:

```csharp
_history.NavigateAsync(
    url: "example/params?page=2",
    title: "2"
);
```

### Result:

```
Params 2
```

---

## 3. Navigation Flow

When you call:

```csharp
_history.NavigateAsync("example/params?page=2");
```

This is what happens internally:

### 🔁 Pipeline

1. **URL Parsing**
2. **Route Resolver (IoC-based)**
3. **Guard execution (CanMatch)**
4. **Guard execution (CanDeactivate)**
5. **ViewModel resolution**
6. **View creation**
7. **Binding (ConfigureTView)**
8. **UI update event**

---

# 🛡️ Guards System

Guards control navigation BEFORE ViewModel is created.

### Base class

```csharp
public abstract class NavigationGuardBase : IGuard
{
    public async Task<GuardResult> ExecuteGuardAsync(NavigationContext context)
    {
        var result = await GuardAsync(context);

        if (result.Decision == GuardDecision.Allow)
            await OnGuardAllow(context);
        else
            await OnGuardDeny(context);

        return result;
    }

    protected abstract Task<GuardResult> GuardAsync(NavigationContext context);
}
```

---

### Example usage

```csharp
x.Register<HomePage, HomeViewModel>("home")
 .CanMatchGuard<AuthGuard>()
 .CanDeactivateGuard<UnsavedChangesGuard>();
```

---

### Guard lifecycle

* `CanMatch` → executed BEFORE ViewModel is created
* `CanDeactivate` → executed BEFORE leaving current page

If guard returns:

* `Allow` → navigation continues
* `Deny` → navigation stops
* `Redirect` → navigates to another route

---

# 📦 ViewModel Context (IMPORTANT)

This is where many people get confused.

Inside a ViewModel:

```csharp
public override async Task OnNavigatedToAsync(CancellationToken ctx)
{
    Username = Context.Query["name"];
    Page = Context.Query["page"];

    if (Context.Body.TryGetValue(out ParamsBody body))
        Date = body.Date.ToString("hh:mm:ss");
}
```

### ❓ Where does `Context` come from?

It is injected automatically by the router.

Each ViewModel that implements `IRoutePage` receives:

### `NavigationContext`

It contains:

* `Query` → URL query string (`?page=1`)
* `Body` → navigation payload object
* `Path` → route path
* `Title` → computed title
* `RouteSnapshot` → full resolved route state

So nothing is "magic" — it's **injected during ViewModel resolution**.

---

# 🧩 View Binding (Framework Adapter)

Umbra Router does NOT assume how your UI binds.

Instead, you define it:

```csharp
public class RouterHistory<TViewModel> : RouterHistoryBase<TViewModel, Control>
{
    protected override void ConfigureTView(ref Control? view, TViewModel viewModel)
    {
        view.DataContext = viewModel;
    }
}
```

### Why this exists?

Because Umbra Router is **framework-agnostic**.

You decide:

* Avalonia → `DataContext`
* WPF → `DataContext`
* MAUI → `BindingContext`
* WinUI → `DataContext`

The router does NOT enforce UI behavior.

---

# 🧭 Router Registration Example

```csharp
services.AddUmbraRouter<Control, PageViewModelBase>(x =>
{
    x.Register<HomePage, HomeViewModel>("home");
    x.Register<ParamsPage, ParamsModelView>("example/params");

    x.Register<Error404Page, Error404ViewModel>("**");
});
```

---

# 💡 IoC First Design

Everything is resolved through DI:

* ViewModels
* Views
* Guards
* Router services

No `new` outside the container.

---

# 🚀 Why this router exists

* Avoid heavy navigation frameworks
* Full control over pipeline
* Fast guard-first filtering
* No unnecessary ViewModel instantiation
* Framework-independent design

---

# 🧪 Mental Model

Think of it like a conveyor belt:

```
URL
 ↓
Route Resolver (cheap lookup)
 ↓
Guards (can I even continue?)
 ↓
ViewModel creation (DI)
 ↓
View creation (UI layer)
 ↓
Binding injection
 ↓
UI update event
```

If anything fails early → everything after is skipped.

---

# 🧼 Design Philosophy

* Minimal allocations
* Early rejection (guards first)
* IoC everywhere
* No UI coupling
* Explicit lifecycle hooks

---

# 📌 Final Note

If something looks like “magic” in this system, it's usually just:

> dependency injection + structured context passing + strict pipeline order

