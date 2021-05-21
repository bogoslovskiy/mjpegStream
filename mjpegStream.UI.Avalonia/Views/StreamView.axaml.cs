using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace mjpegStream.UI.Avalonia.Views
{
    public partial class StreamView : UserControl
    {
        public StreamView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}