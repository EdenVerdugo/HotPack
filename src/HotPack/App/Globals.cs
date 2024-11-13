using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.App
{
    public class Globals
    {
        public static Globals Instance { get; set; } = new Globals();

        public Globals()
        {
            ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
            ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name!;
            ApplicationVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()!;
        }

        public Globals(string applicationPath)
        {
            ApplicationPath = applicationPath;

            ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name!;
            ApplicationVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()!;
        }


        // Verifica si el ensamblado 'Microsoft.AspNetCore.Http' (indicativo de ASP.NET Core) está cargado
        public static bool IsAspNetCoreApp()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.GetName().Name!.StartsWith("Microsoft.AspNetCore"));
        }

        // Verifica si el ensamblado 'System.Windows.Forms' o 'PresentationFramework' (indicativo de aplicaciones de escritorio) está cargado
        public static bool IsDesktopApp()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.GetName().Name == "System.Windows.Forms" || a.GetName().Name == "PresentationFramework");
        }

        // Verifica si es una aplicación de consola (no es ASP.NET Core ni una aplicación de escritorio)
        public static bool IsConsoleApp()
        {
            // Si no es ASP.NET Core ni una app de escritorio, es una consola
            return !IsAspNetCoreApp() && !IsDesktopApp();
        }

        public string ApplicationPath { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
    }
}
