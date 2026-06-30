using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using IRis.Services.Singleton;
using IRis.ViewModels;
using IRis.ViewModels.Main;
using IRis.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IRis;

public partial class App : Application
{
    public IServiceProvider Services { get; }
    public static new App Current => (App)Application.Current!;

    public static new IClassicDesktopStyleApplicationLifetime ApplicationLifetime =>
        (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!;

    public static MainWindowViewModel MainWindow =>
        (MainWindowViewModel)ApplicationLifetime.MainWindow!.DataContext!;

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
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<CanvasViewModel>();
        services.AddSingleton<GenerateFromImageWindowViewModel>();
        services.AddSingleton<LeftSidebarViewModel>();

        return services.BuildServiceProvider();
    }
}
