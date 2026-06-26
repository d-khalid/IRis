using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using IRis.ViewModels.Main;
using IRis.Views.Main;
using IRis.Views;
using IRis.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using IRis.Services.Singleton;
using IRis.Services;


namespace IRis;


public partial class App : Application
{
    public IServiceProvider Services { get; }
    public new static App Current => (App)Application.Current!;
    public new static IClassicDesktopStyleApplicationLifetime ApplicationLifetime => (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!;


    public App()
    {
        Services = ConfigureServices();
    }


    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = Services.GetRequiredService<AppViewModel>();
    }


    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindowView();
        }

        base.OnFrameworkInitializationCompleted();
    }


    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<AppState>();
        services.AddSingleton<Simulation>();
        services.AddSingleton<Selection>();
        services.AddSingleton<Preview>();
        services.AddSingleton<WirePreview>();
        services.AddSingleton<SelectionBox>();
        services.AddSingleton<AppViewModel>();

        return services.BuildServiceProvider();
    }
}
