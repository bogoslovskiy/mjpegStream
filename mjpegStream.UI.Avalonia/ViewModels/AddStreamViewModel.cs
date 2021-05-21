namespace mjpegStream.UI.Avalonia.ViewModels
{
    using System.Reactive;
    using System.Windows.Input;
    using ReactiveUI;

    public class AddStreamViewModel : ViewModelBase
    {
        private string? _uri = "http://47.206.111.174/mjpg/video.mjpg";
        private StreamViewModel _streamViewModel;
        
        public string? Uri
        {
            get => _uri;
            set => this.RaiseAndSetIfChanged(ref _uri, value);
        }

        public StreamViewModel Stream
        {
            get => _streamViewModel;
            set => this.RaiseAndSetIfChanged(ref _streamViewModel, value);
        }
        
        public ICommand TestCommand { get; }
        
        public ReactiveCommand<Unit, StreamViewModel?> AddStreamCommand { get; }

        public AddStreamViewModel()
        {
            this.AddStreamCommand = ReactiveCommand.Create(
                () =>
                {
                    this.Stream?.Dispose();
                    
                    return new StreamViewModel(this.Uri);
                }
            );
            
            this.TestCommand = ReactiveCommand.Create(
                () =>
                {
                    this.Stream?.Dispose();
                    
                    this.Stream = new StreamViewModel(this.Uri);
                }
            );
        }
    }
}