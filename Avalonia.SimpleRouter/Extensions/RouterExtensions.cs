using System.Reflection;
using Avalonia.Controls;
using Avalonia.SimpleRouter.Configuration;
using Avalonia.SimpleRouter.Interfaces;
using Avalonia.SimpleRouter.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.SimpleRouter.Extensions;

public static class RouterExtensions
{
    public static IServiceCollection AddAvaloniaRouter<ViewModelBase>(this IServiceCollection services, Action<RouterConfig> options) where ViewModelBase : class, IRoutePage
    {
        var config = new RouterConfig();
        
        options(config);

        foreach (var page in config._pages)
        {
            var method = typeof(RouterExtensions).GetMethod(nameof(AddControl), BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(page.Value, page.Key);
            generic.Invoke(null, new object[] { services });
        }
        
        services.AddSingleton(config);
        services.AddSingleton<RouterMapper>();
        services.AddSingleton<RouterHistoryManager<ViewModelBase>>();
        
        
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