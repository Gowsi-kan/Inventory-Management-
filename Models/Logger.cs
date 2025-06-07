using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace InventoryManagement.Models
{
    public static class Logger
    {
        private static bool isInitialized = false;

        public static void InitializeLogger()
        {
            if (!isInitialized)
            {
                string logPath = "C:\\Logs\\logfile.log";

                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(logPath,
                        restrictedToMinimumLevel: LogEventLevel.Error,
                        rollingInterval: RollingInterval.Month,
                        fileSizeLimitBytes: 4096,
                        rollOnFileSizeLimit: true)
                    .WriteTo.File(new JsonFormatter(), logPath)
                    .CreateLogger();

                isInitialized = true;
            }
        }

        public static void WriteLogDebug(string message)
        {
            if (!isInitialized)
            {
                InitializeLogger();
            }

            Log.Logger.Debug($"{message}");
        }

        public static void WriteLogInformation(string message)
        {
            if (!isInitialized)
            {
                InitializeLogger();
            }

            Log.Logger.Information($"{message}");
        }

        public static void WriteLogError(string message)
        {
            if (!isInitialized)
            {
                InitializeLogger();
            }

            Log.Logger.Error($"{message}");
        }

        public static void WriteLogFatal(string message)
        {
            if (!isInitialized)
            {
                InitializeLogger();
            }

            Log.Logger.Fatal($"{message}");
        }

        public static void WriteLogWarning(string message)
        {
            if (!isInitialized)
            {
                InitializeLogger();
            }

            Log.Logger.Warning($"{message}");
        }
    }
}
