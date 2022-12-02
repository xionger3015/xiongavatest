using CefNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApp
{
    public class CefAppImpl : CefNetApplication
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            base.OnBeforeCommandLineProcessing(processType, commandLine);

            //Console.WriteLine("ChromiumWebBrowser_OnBeforeCommandLineProcessing");
            //Console.WriteLine(commandLine.CommandLineString);

            //commandLine.AppendSwitchWithValue("proxy-server", "127.0.0.1:8888");

            commandLine.AppendSwitch("ignore-certificate-errors");
            commandLine.AppendSwitchWithValue("remote-debugging-port", "9222");

            //enable-devtools-experiments
            commandLine.AppendSwitch("enable-devtools-experiments");

            //e.CommandLine.AppendSwitchWithValue("user-agent", "Mozilla/5.0 (Windows 10.0) WebKa/" + DateTime.UtcNow.Ticks);

            //("force-device-scale-factor", "1");

            //commandLine.AppendSwitch("disable-gpu");
            //commandLine.AppendSwitch("disable-gpu-compositing");
            //commandLine.AppendSwitch("disable-gpu-vsync");

            commandLine.AppendSwitch("enable-begin-frame-scheduling");
            commandLine.AppendSwitch("enable-media-stream");

            commandLine.AppendSwitchWithValue("enable-blink-features", "CSSPseudoHas");
            ////开启https允许http资源
            //commandLine.AppendSwitchWithValue("allow-running-insecure-content", "1");
            ////关闭同源策略,允许跨域
            //commandLine.AppendSwitchWithValue("disable-web-security", "1");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                commandLine.AppendSwitch("no-zygote");
                commandLine.AppendSwitch("no-sandbox");
            }
        }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            base.OnContextCreated(browser, frame, context);
            frame.ExecuteJavaScript(@"
            {
            const newProto = navigator.__proto__;
            delete newProto.webdriver;
            navigator.__proto__ = newProto;
            }", frame.Url, 0);

            //var window = context.GetGlobal();

            //var cefObj = CefV8Value.CreateObject();
            //var handler = new V8FunctionHandler();
            //foreach (var item in MethodsNames)
            //{
            //    var func = CefV8Value.CreateFunction(item, handler); 
            //    var attributes = CefV8PropertyAttribute.ReadOnly | CefV8PropertyAttribute.DontDelete;
            //    cefObj.SetValue(item, func, attributes);
            //}

            //window?.SetValue(ObjectName, cefObj, CefV8PropertyAttribute.None);
        }
    }
}
