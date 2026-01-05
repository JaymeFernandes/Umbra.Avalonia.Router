using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Umbra.Avalonia.Router.Configuration;
using Umbra.Avalonia.Router.Interfaces;
using Umbra.Avalonia.Router.Services;

namespace Umbra.Avalonia.Router.Extensions;

public static class RouterExtensions
{
    public static IServiceCollection AddAvaloniaRouter<ViewModelBase>(this IServiceCollection services, Action<RouterConfig<ViewModelBase>> options) where ViewModelBase : class, IRoutePage
    {
        var config = new RouterConfig<ViewModelBase>();

        options(config);

        foreach (var page in config._pages)
        {
            var method = typeof(RouterExtensions).GetMethod(nameof(AddControl), BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(page.Value, page.Key);
            generic.Invoke(null, new object[] { services });
        }

        services.AddSingleton(config);
        services.AddSingleton<IRouterResolver<ViewModelBase>, RouterResolver<ViewModelBase>>();
        services.AddSingleton<RouterHistory<ViewModelBase>>();

        return services;
    }

    private static IServiceCollection AddControl<TControl, TModel>(this IServiceCollection services) where TControl : Control, new() where TModel : class
    {
        services.AddTransient<TModel>();
        services.AddTransient<TControl>(x => new TControl()
        {
            DataContext = x.GetRequiredService<TModel>()
        });

        return services;
    }
}
