/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.DocumentViewerServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// ViewModel for the document viewer page.
    /// </summary>
    public class DocumentViewerPageViewModel : ViewModelBase
    {
        #region Private fields

        private DocumentViewerService _documentViewerService;
        private ImageSource _documentImage;
        private DocumentEntry _document;
        private DelegateCommand _open;
        private string _documentName = "Document";
        private bool _loading;

        #endregion

        #region Private methods

        /// <summary>
        /// Opens the current document in the associated application.
        /// </summary>
        /// <returns></returns>
        private async Task OpenDocument()
        {
            if (_document == null) return;

            Loading = true;

            try
            {
                using (var stream = await _documentViewerService.GetFileStream(_document.Id))
                {
                    await _documentViewerService.SaveToFile(stream, _document.Name);
                    await _documentViewerService.LaunchFileAssociation(_document.Name);
                }
            }
            catch (Exception exception)
            {
                await Dialogs.ShowException(string.Format("Failed to launch the file '{0}'.", _document.Name), exception, false);
            }
            finally
            {
                Loading = false;
            }
        }

        /// <summary>
        /// Command for Open, which allows the routine to be shared by both await Task and await void.
        /// </summary>
        private async void DoOpen()
        {
            await OpenDocument();
        }

        /// <summary>
        /// Determines if the document can be opened.
        /// </summary>
        /// <returns></returns>
        private bool CanOpenDocument()
        {
            return (!string.IsNullOrEmpty(_documentName));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DocumentViewerPageViewModel()
        {
            _documentViewerService = DocumentViewerService.Instance;
            _open = new DelegateCommand(new Action(DoOpen), CanOpenDocument);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Called when the page is being naviagted to.
        /// </summary>
        /// <param name="parameter">The parameter passed during navigation.</param>
        /// <param name="mode">The navigation mode.</param>
        /// <param name="suspensionState">The saved state.</param>
        /// <returns>The async task to wait on.</returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var doc = (DocumentEntry)parameter;

            Loading = true;

            try
            {
                if (doc != null)
                {
                    _document = doc;

                    DocumentName = _document.Name;

                    using (var stream = await _documentViewerService.GetPdfStream(_document.Id))
                    {
                        var image = new BitmapImage();

                        image.SetSource(stream);
                        DocumentImage = image;
                    }
                }
            }
            finally
            {
                Loading = false;
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Event that is triggered when the image is tapped.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event arguments.</param>
        public async void OnImageDoubleTapped(object sender, DoubleTappedRoutedEventArgs args)
        {
            var scrollViewer = VisualTree.GetParent<ScrollViewer>((DependencyObject)sender);
            var doubleTapPoint = args.GetPosition(scrollViewer);

            if (scrollViewer.ZoomFactor != 1)
            {
                scrollViewer.ZoomToFactor(1);
                return;
            }

            if (scrollViewer.ZoomFactor == 1)
            {
                scrollViewer.ZoomToFactor(2);

                await Dispatcher.DispatchAsync(() =>
                {
                    scrollViewer.ScrollToHorizontalOffset(doubleTapPoint.X);
                    scrollViewer.ScrollToVerticalOffset(doubleTapPoint.Y);
                });
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Opens the current document.
        /// </summary>
        public DelegateCommand Open
        {
            get { return _open; }
        }

        /// <summary>
        /// The document image source.
        /// </summary>
        public ImageSource DocumentImage
        {
            get { return _documentImage; }
            set { Set(ref _documentImage, value); }
        }

        /// <summary>
        /// The document name
        /// </summary>
        public string DocumentName
        {
            get { return _documentName; }
            set { Set(ref _documentName, value); }
        }

        /// <summary>
        /// True if loading.
        /// </summary>
        public bool Loading
        {
            get { return _loading; }
            set { Set(ref _loading, value); }
        }

        #endregion
    }
}
