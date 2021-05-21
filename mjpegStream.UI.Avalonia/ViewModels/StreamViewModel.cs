namespace mjpegStream.UI.Avalonia.ViewModels
{
    using System;
    using System.IO;
    using global::Avalonia.Media.Imaging;
    using global::Avalonia.Visuals.Media.Imaging;
    using mjpegStream.UI.Avalonia.Models;
    using ReactiveUI;

    public class StreamViewModel : ViewModelBase, IDisposable
    {
        private readonly StreamModel _model;
        private bool _unexpectedError;
        private Bitmap _image;

        public bool UnexpectedError
        {
            get => _unexpectedError;
            set => this.RaiseAndSetIfChanged(ref _unexpectedError, true);
        }

        public Bitmap Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        public StreamViewModel()
        {
        }
        
        public StreamViewModel(string uri) : this()
        {
            _model = new StreamModel(uri);
            
            _model.OnUnexpectedError += model_OnUnexpectedError;
            _model.OnJpegBinaryUpdated += model_OnJpegBinaryUpdated;
        }

        private void model_OnJpegBinaryUpdated(object? sender, StreamModel.JpegBinaryUpdatedEventArgs e)
        {
            using MemoryStream memoryStream = new(e.Data);
            
            Bitmap newImage = Bitmap.DecodeToWidth(memoryStream, 800, BitmapInterpolationMode.Default);

            this.Image = newImage;
        }

        private void model_OnUnexpectedError(object? sender, EventArgs e)
        {
            this.UnexpectedError = true;
        }

        public void Dispose()
        {
            _model.Dispose();
            _image.Dispose();
        }
    }
}