using Akavache;
using InventoryManagement.Services;
using InventoryManagement.ViewModels;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using QuestPDF.Infrastructure;

namespace InventoryManagement
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Set license
            QuestPDF.Settings.License = LicenseType.Community;

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureServices();

            builder.Services.AddMauiBlazorWebView();

            #if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
            #endif

            return builder.Build();
        }

        private static void ConfigureServices(this MauiAppBuilder builder)
        {
            // Set the Application Name before using Akavache
            BlobCache.ApplicationName = "InventoryManagementApp";

            builder.Services.AddMudServices();
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;

                config.SnackbarConfiguration.PreventDuplicates = true;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 2000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });

            builder.Services.AddSingleton<HttpClient>();

            // Ensure Akavache is properly initialized
            builder.Services.AddSingleton<IBlobCache>(BlobCache.LocalMachine);

            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<EmployeeService>();
            builder.Services.AddScoped<InvoiceService>();
            builder.Services.AddScoped<ExpenseService>();

            builder.Services.AddSingleton<RealTimerService>();

            builder.Services.AddSingleton<InventoryViewModel>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<OrdersViewModel>();
            builder.Services.AddSingleton<EmployeeViewModel>();
            builder.Services.AddSingleton<InvoiceViewModel>();
            builder.Services.AddSingleton<ExpenseViewModel>();
        }
    }
}
