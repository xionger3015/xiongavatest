using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CefNet.Input;
using CefNet;
using System;
using System.Linq;
using System.IO;

namespace AvaloniaApp
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            bool externalMessagePump = args.Contains("--external-message-pump");
            string basePath = AppContext.BaseDirectory;
            string cefPath = Path.Combine(basePath, "cef");
            //Cef 
            var settings = new CefSettings();
            settings.Locale = "zh-CN";
            settings.MultiThreadedMessageLoop = !externalMessagePump;
            settings.ExternalMessagePump = externalMessagePump;
            settings.NoSandbox = true;
            settings.WindowlessRenderingEnabled = true;
            settings.LocalesDirPath = Path.Combine(cefPath, "locales");
            settings.ResourcesDirPath = cefPath;
            settings.LogSeverity = CefLogSeverity.Warning;
            settings.UncaughtExceptionStackSize = 8;
            settings.CommandLineArgsDisabled = true;
            var app = new CefAppImpl();

            KeycodeConverter.Default = new FixChineseInputKeycodeConverter();

            app.Initialize(PlatformInfo.IsMacOS ? cefPath : cefPath, settings);//Path.Combine(cefPath, "Release")

            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}

namespace CefNet.Input
{
    /// <summary>
    /// https://github.com/CefNet/CefNet/issues/21
    /// 修复中文输入报错问题
    /// </summary>
    public class FixChineseInputKeycodeConverter : KeycodeConverter
    {
        public override VirtualKeys CharacterToVirtualKey(char character)
        {
            // https://github.com/CefNet/CefNet/blob/master/CefNet/Input/KeycodeConverter.cs#L41
            // https://github.com/CefNet/CefNet/blob/90.5.21109.1453/CefNet/Input/KeycodeConverter.cs#L41
            try
            {
                return base.CharacterToVirtualKey(character);
            }
            catch (Exception e)
            {
                if (e.Message == "Incompatible input locale.")
                {
                    return VirtualKeys.None;
                }
                throw;
            }
        }
    }
}