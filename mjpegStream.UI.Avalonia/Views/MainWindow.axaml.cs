using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace mjpegStream.UI.Avalonia.Views
{
    using System.Threading.Tasks;
    using global::Avalonia.ReactiveUI;
    using mjpegStream.UI.Avalonia.ViewModels;
    using ReactiveUI;

    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            
            this.WhenActivated(x => x(this.ViewModel.ShowAddStreamViewDialog.RegisterHandler(DoShowAddStreamViewDialogAsync)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private async Task DoShowAddStreamViewDialogAsync(InteractionContext<AddStreamViewModel, StreamViewModel?> interaction)
        {
            AddStreamView dialog = new() {DataContext = interaction.Input};

            StreamViewModel? result = await dialog.ShowDialog<StreamViewModel?>(this);
            
            interaction.SetOutput(result);
        }
    }
}