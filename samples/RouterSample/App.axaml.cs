using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.SimpleRouter.Extensions;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RouterSample.Pages;
using RouterSample.Pages.Example;
using RouterSample.Pages.FirstSub;
using RouterSample.Pages.SecondSub;
using RouterSample.Pages.ThirdSub;
using RouterSample.ViewModels;
using RouterSample.ViewModels.Example;
using RouterSample.ViewModels.FirstSub;
using RouterSample.ViewModels.SecondSub;
using RouterSample.ViewModels.Shared;
using RouterSample.ViewModels.ThirdSub;

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

        services.AddAvaloniaRouter<PageViewModelBase>(x =>
        {
            x.Register<HomePage, HomeViewModel>("home");
            x.Register<ParamsPage, ParamsModelView>("example/params");
            x.Register<FirstSubPage, FirstSubViewModel>("sub/first");
            x.Register<SecondSubPage, SecondSubViewModel>("sub/second");
            x.Register<ThirdSubPage, ThirdSubViewModel>("sub/third");
        });
        
        var dryIoc = new Container()
            .WithDependencyInjectionAdapter(services); 
        
        dryIoc.Populate(services);
        
        Container = dryIoc;
    }
}