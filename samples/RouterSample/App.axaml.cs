using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RouterSample.Pages;
using RouterSample.Pages._404;
using RouterSample.Pages.Example;
using RouterSample.Pages.FirstSub;
using RouterSample.Pages.SecondSub;
using RouterSample.Pages.ThirdSub;
using RouterSample.ViewModels;
using RouterSample.ViewModels.Error404;
using RouterSample.ViewModels.Example;
using RouterSample.ViewModels.FirstSub;
using RouterSample.ViewModels.SecondSub;
using RouterSample.ViewModels.Shared;
using RouterSample.ViewModels.ThirdSub;
using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Extensions;

namespace RouterSample;

public partial class App : Application
{
    public static IContainer Container { get; set; }

    public override void Initialize()
    {
        ConfigureServices();
    
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        services.AddUmbraRouter<Control, PageViewModelBase>(x =>
        {
            // Forma 1
            x.Register<HomePage, HomeViewModel>("home");
            x.Register<ParamsPage, ParamsModelView>("example/params");
            x.Register<FirstSubPage, FirstSubViewModel>("sub/first");
            x.Register<SecondSubPage, SecondSubViewModel>("sub/second");
            x.Register<ThirdSubPage, ThirdSubViewModel>("sub/third");
            x.Register<Error404Page, Error404ViewModel>("**");
            
            // Forma 2
            x.UseAngularStyleRoutes(new RoutesAngularStyle
            {
                new RouteAngularStyle
                {
                    Path = "home",
                    Component =  typeof(HomePage),
                    ViewModel = typeof(HomeViewModel)
                },
                new RouteAngularStyle
                {
                    Path = "example/params",
                    Component = typeof(ParamsPage),
                    ViewModel = typeof(ParamsModelView)
                },
                new RouteAngularStyle
                {
                    Path = "sub",
                    Children =
                    [
                        new RouteAngularStyle
                        {
                            Path = "first",
                            Component =  typeof(FirstSubPage),
                            ViewModel = typeof(FirstSubViewModel)
                        },
                        new RouteAngularStyle
                        {
                            Path = "second",
                            Component =  typeof(SecondSubPage),
                            ViewModel = typeof(SecondSubViewModel)
                        },
                        new RouteAngularStyle
                        {
                            Path = "third",
                            Component =  typeof(ThirdSubPage),
                            ViewModel = typeof(ThirdSubViewModel)
                        }
                    ]
                },
                new RouteAngularStyle
                {
                    Path = "**",
                    Component = typeof(Error404Page),
                    ViewModel = typeof(Error404ViewModel)
                }
            });
        });

        services.AddRouterHistory<RouterHistory<PageViewModelBase>, Control, PageViewModelBase>();
    
        var dryIoc = new Container()
            .WithDependencyInjectionAdapter(services); 
    
        dryIoc.Populate(services);
    
        Container = dryIoc;
    }
}