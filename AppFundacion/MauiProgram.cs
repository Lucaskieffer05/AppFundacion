using AppFundacion.Models;
using AppFundacion.Views;
using AppFundacion.Controllers;
using AppFundacion.ViewModels;
using Microsoft.Extensions.Logging;
using UraniumUI;
using Microsoft.EntityFrameworkCore;
using AppFundacion.ExcelServices;
using Microsoft.Maui.LifecycleEvents;

namespace AppFundacion
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFontAwesomeIconFonts();
                });

#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                // Make sure to add "using Microsoft.Maui.LifecycleEvents;" in the top of the file
                events.AddWindows(windowsLifecycleBuilder =>
                {
                    windowsLifecycleBuilder.OnWindowCreated(window =>
                    {
                        window.ExtendsContentIntoTitleBar = false;
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

                        if (appWindow is not null)
                        {
                            Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(id, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
                            if (displayArea is not null)
                            {
                                var CenteredPosition = appWindow.Position;
                                CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                                CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                                appWindow.Move(CenteredPosition);
                            }
                        }
                    });
                });
            });
#endif

            builder.Services.AddScoped<DonanteController>();

            builder.Services.AddSingleton<DonantesView>();
            builder.Services.AddSingleton<DonantesViewModel>();

            builder.Services.AddTransient<ReportesView>();
            builder.Services.AddTransient<ReportesViewModel>();

            builder.Services.AddTransient<DonanteModificarView>();
            builder.Services.AddTransient<DonantesModificarViewModel>();

            builder.Services.AddTransient<DonanteAgregarView>();
            builder.Services.AddTransient<DonanteAgregarViewModel>();

            builder.Services.AddTransient<CobradorAgregarView>();
            builder.Services.AddTransient<CobradorAgregarViewModel>();

            builder.Services.AddTransient<FuncionalidadesExtrasView>();
            builder.Services.AddTransient<FuncionalidadesExtrasViewModel>();

            builder.Services.AddTransient<HomeView>();
            builder.Services.AddTransient<HomeViewModel>();

            builder.Services.AddTransient<CobradorModificarView>();
            builder.Services.AddTransient<CobradorModificarViewModel>();

            builder.Services.AddTransient<ZonaAgregarView>();
            builder.Services.AddTransient<ZonaAgregarViewModel>();

            builder.Services.AddTransient<ConfiguracionView>();
            builder.Services.AddTransient<ConfiguracionViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
