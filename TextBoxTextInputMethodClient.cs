using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Input.TextInput;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApp
{
    internal class TextBoxTextInputMethodClient : ITextInputMethodClient
    {
        private TextPresenter _presenter;
        private IDisposable _subscription;
        public Rect CursorRectangle => new Rect(new Point(x, y), new Point(x, y + 5)); //_presenter?.GetCursorRectangle() ?? default;
        public event EventHandler CursorRectangleChanged;
        public IVisual TextViewVisual => _presenter;
        public event EventHandler TextViewVisualChanged;
        public bool SupportsPreedit => false;
        public void SetPreeditText(string text) => throw new NotSupportedException();

        public bool SupportsSurroundingText => false;
        public TextInputMethodSurroundingText SurroundingText => throw new NotSupportedException();
        public event EventHandler SurroundingTextChanged;
        public string TextBeforeCursor => null;
        public string TextAfterCursor => null;

        private void OnCaretIndexChanged(int index) => CursorRectangleChanged?.Invoke(this, EventArgs.Empty);

        public void SetPresenter(TextPresenter presenter)
        {
            _subscription?.Dispose();
            _subscription = null;
            _presenter = presenter;
            if (_presenter != null)
            {
                _subscription = _presenter.GetObservable(TextPresenter.CaretIndexProperty)
                    .Subscribe(OnCaretIndexChanged);
            }
            TextViewVisualChanged?.Invoke(this, EventArgs.Empty);
            CursorRectangleChanged?.Invoke(this, EventArgs.Empty);
        }

        //设置输入框位置
        private double x, y;
        public void SetPosition(Point pos)
        {
            x = pos.X;
            y = pos.Y;
            CursorRectangleChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
