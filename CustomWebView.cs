using Avalonia.Input;
using Avalonia.Interactivity;
using CefNet.Avalonia;
using CefNet.Input;
using CefNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApp
{
    sealed class CustomWebView : WebView
    {
        public static RoutedEvent<FullscreenModeChangeEventArgs> FullscreenEvent = RoutedEvent.Register<WebView, FullscreenModeChangeEventArgs>("Fullscreen", RoutingStrategies.Bubble);

        public event EventHandler<FullscreenModeChangeEventArgs> Fullscreen
        {
            add { AddHandler(FullscreenEvent, value); }
            remove { RemoveHandler(FullscreenEvent, value); }
        }

        private TextBoxTextInputMethodClient _imClient = new TextBoxTextInputMethodClient();
        static CustomWebView()
        {
            TextInputMethodClientRequestedEvent.AddClassHandler<CustomWebView>((tb, e) =>
            {
                e.Client = tb._imClient;
            });
        }
        public CustomWebView()
        {

        }

        public CustomWebView(WebView opener)
            : base(opener)
        {
        }

        //protected override WebViewGlue CreateWebViewGlue()
        //{
        //	return new CustomWebViewGlue(this);
        //}



        internal void RaiseFullscreenModeChange(bool fullscreen)
        {
            RaiseCrossThreadEvent(OnFullScreenModeChange, new FullscreenModeChangeEventArgs(this, fullscreen), false);
        }

        private void OnFullScreenModeChange(FullscreenModeChangeEventArgs e)
        {
            RaiseEvent(e);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            foreach (char symbol in e.Text)
            {
                CefEventFlags modifiers = KeycodeConverter.IsShiftRequired(symbol) ? CefEventFlags.ShiftDown : CefEventFlags.None;

                if (symbol >= 0x4e00 && symbol <= 0x9fff)
                {
                    var k = new CefKeyEvent();
                    k.Type = CefKeyEventType.Char;
                    k.WindowsKeyCode = symbol;
                    k.Character = symbol;
                    k.UnmodifiedCharacter = symbol;
                    k.Modifiers = (uint)modifiers;
                    k.NativeKeyCode = symbol;
                    this.BrowserObject?.Host.SendKeyEvent(k);
                }
                else
                {
                    VirtualKeys key = KeycodeConverter.CharacterToVirtualKey(symbol);

                    var k = new CefKeyEvent();
                    k.Type = CefKeyEventType.Char;
                    k.WindowsKeyCode = PlatformInfo.IsLinux ? (int)key : symbol;
                    k.Character = symbol;
                    k.UnmodifiedCharacter = symbol;
                    k.Modifiers = (uint)modifiers;
                    k.NativeKeyCode = KeycodeConverter.VirtualKeyToNativeKeyCode(key, modifiers, false);
                    this.BrowserObject?.Host.SendKeyEvent(k);
                }
            }
            e.Handled = true;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            var mousePos = e.GetPosition(this);
            _imClient.SetPosition(mousePos);

            base.OnPointerPressed(e);
        }
    }
}
