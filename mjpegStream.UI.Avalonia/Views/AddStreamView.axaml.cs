using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace mjpegStream.UI.Avalonia.Views
{
    using System;
    using global::Avalonia.ReactiveUI;
    using mjpegStream.UI.Avalonia.ViewModels;
    using ReactiveUI;

    public partial class AddStreamView : ReactiveWindow<AddStreamViewModel>
    {
        public AddStreamView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            
            this.WhenActivated(x => x(this.ViewModel.AddStreamCommand.Subscribe(Close)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}