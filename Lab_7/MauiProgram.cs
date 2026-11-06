using Lab_7.Views;
using Lab_7.ViewModels;
using Lab_7.Services;
using Microsoft.Extensions.Logging;

namespace Lab_7
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Реєстрація сервісів та баз даних
            builder.Services.AddSingleton<TeacherService>();

            // Реєстрація ViewModels
            builder.Services.AddSingleton<TeachersListViewModel>();
            builder.Services.AddTransient<TeacherEditViewModel>();
            builder.Services.AddTransient<TeacherDetailsViewModel>();

            // Реєстрація Сторінок (Views)
            builder.Services.AddSingleton<TeachersListPage>();
            builder.Services.AddTransient<TeacherEditPage>();
            builder.Services.AddTransient<TeacherDetailsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Реєстрація маршрутів для навігації між сторінками MAUI
            Routing.RegisterRoute(nameof(TeacherEditPage), typeof(TeacherEditPage));
            Routing.RegisterRoute(nameof(TeacherDetailsPage), typeof(TeacherDetailsPage));

            return app;
        }
    }
}