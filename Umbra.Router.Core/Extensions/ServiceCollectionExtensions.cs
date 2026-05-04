using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Services;

namespace Umbra.Router.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUmbraRouter<TControl, TViewModel>(this IServiceCollection serviceCollection, Action<RouterConfig<TViewModel>> options) 
        where TControl : class where TViewModel : class, IRoutePage
    {
      var config = new RouterConfig<TViewModel>();
      options(config);
      
      foreach (var page in config.GetAllDefinitions())
      {
          var method =
              typeof(ServiceCollectionExtensions).GetMethod(nameof(AddControl), BindingFlags.NonPublic | BindingFlags.Static);
          
          if(method == null)
              continue;
          
          var generic = method.MakeGenericMethod(page.View, page.ViewModel);
          generic.Invoke(null, new object[] { serviceCollection });
      }

      serviceCollection.AddSingleton(config);
      serviceCollection.AddSingleton<IRouterResolver<TViewModel>, RouterResolver<TViewModel>>();
      serviceCollection.AddSingleton<GuardServices<TViewModel>>();
        
      return serviceCollection;
    }

    public static IServiceCollection AddRouterHistory<TRouterHistory, TControl, TViewModel>(this IServiceCollection serviceCollection)
        where TViewModel : class, IRoutePage
        where TControl : class
        where TRouterHistory : RouterHistoryBase<TViewModel, TControl>
    {
        serviceCollection.AddSingleton<TRouterHistory>();
        
        return serviceCollection;
    }
    
    private static IServiceCollection AddControl<TControl, TModel>(this IServiceCollection services)
        where TControl : class, new() where TModel : class
    {
        services.AddTransient<TModel>();
        services.AddTransient<TControl>();

        return services;
    }
}