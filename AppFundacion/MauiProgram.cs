using AppFundacion.Models;
using AppFundacion.Views;
using AppFundacion.Controllers;
using AppFundacion.ViewModels;
using Microsoft.Extensions.Logging;
using UraniumUI;
using Microsoft.EntityFrameworkCore;
using AppFundacion.ExcelServices;

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

            builder.Services.AddScoped<DonanteController>();

            builder.Services.AddSingleton<DonantesView>();
            builder.Services.AddSingleton<DonantesViewModel>();

            builder.Services.AddTransient<ReportesView>();
            builder.Services.AddTransient<ReportesViewModel>();

            builder.Services.AddTransient<DonanteModificarView>();
            builder.Services.AddTransient<DonantesModificarViewModel>();

            builder.Services.AddTransient<DonanteAgregarView>();
            builder.Services.AddTransient<DonanteAgregarViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
