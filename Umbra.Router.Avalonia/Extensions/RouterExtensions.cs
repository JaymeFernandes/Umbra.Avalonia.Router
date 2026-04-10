using System.Reflection;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Services;

namespace Umbra.Router.Avalonia.Extensions;

public static class RouterExtensions
{
    public static IServiceCollection AddAvaloniaRouter<ViewModelBase>(this IServiceCollection services,
        Action<RouterConfig<ViewModelBase>> options) where ViewModelBase : class, IRoutePage
    {
        var config = new RouterConfig<ViewModelBase>();

        options(config);

        foreach (var page in config.GetAllDefinitions())
        {
            var method =
                typeof(RouterExtensions).GetMethod(nameof(AddControl), BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(page.View, page.ViewModel);
            generic.Invoke(null, new object[] { services });
        }

        services.AddSingleton(config);
        services.AddSingleton<IRouterResolver<ViewModelBase>, RouterResolver<ViewModelBase>>();
        services.AddSingleton<RouterHistory<ViewModelBase>>();
        services.AddSingleton<GuardServices<ViewModelBase>>();

        return services;
    }

    private static IServiceCollection AddControl<TControl, TModel>(this IServiceCollection services)
        where TControl : Control, new() where TModel : class
    {
        services.AddTransient<TModel>();
        services.AddTransient<TControl>(x => new TControl
        {
            DataContext = x.GetRequiredService<TModel>()
        });

        return services;
    }
}