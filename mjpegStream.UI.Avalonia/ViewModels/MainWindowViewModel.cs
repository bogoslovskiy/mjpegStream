namespace mjpegStream.UI.Avalonia.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using System.Windows.Input;
    using ReactiveUI;

    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand AddCommand { get; }
        
        public Interaction<AddStreamViewModel, StreamViewModel?> ShowAddStreamViewDialog { get; }
        
        public ObservableCollection<StreamViewModel> Streams { get; } = new();

        public MainWindowViewModel()
        {
            this.ShowAddStreamViewDialog = new Interaction<AddStreamViewModel, StreamViewModel?>();
            
            this.AddCommand = ReactiveCommand.Create(
                async () =>
                {
                    StreamViewModel? newStreamViewModel = await this.ShowAddStreamViewDialog.Handle(new AddStreamViewModel());

                    if (newStreamViewModel != null)
                    {
                        this.Streams.Add(newStreamViewModel);
                    }
                }
            );
        }
    }
}