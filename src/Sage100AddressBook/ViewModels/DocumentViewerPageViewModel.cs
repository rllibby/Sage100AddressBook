using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sage100AddressBook.Services.DocumentViewerServices;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using Sage100AddressBook.Models;

namespace Sage100AddressBook.ViewModels
{
    public class DocumentViewerPageViewModel : ViewModelBase
    {

        private DocumentViewerService _documentViewerService;

        public DocumentViewerPageViewModel()
        {

            _documentViewerService = new DocumentViewerService();
        }

        private ImageSource _documentImage;


        public ImageSource DocumentImage { get { return _documentImage; } set { Set(ref _documentImage, value); } }
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //Search = (suspensionState.ContainsKey(nameof(Search))) ? suspensionState[nameof(Search)]?.ToString() : parameter?.ToString();

            //_addresses = new ObservableCollection<AddressEntry>(await _searchService.ExecuteSearchAsync(Search));
            //Sage100AddressBook.Views.DocumentViewerPage.Browser.Navigate("http://espn.com");

            //to-do - eventually will want to obtain url from the parameter
            //var url = "https://swmsagedev-my.sharepoint.com/personal/steve_swmsagedev_onmicrosoft_com/_layouts/15/guestaccess.aspx?guestaccesstoken=5T9hTXjlxFe9qJQm1DUPHC9p%2bCUQEKHT8syowQa747E%3d&docid=0823727dd340d4ae884dbc9525a2932ed";

            var doc = (DocumentEntry)parameter;
            //get a stream for a pdf page
            var stream = await _documentViewerService.GetPDFStreamAsync(doc.Id);

            BitmapImage src = new BitmapImage();

            src.SetSource(stream);
            DocumentImage = src;
 

            await Task.CompletedTask;
        }
    }
}
