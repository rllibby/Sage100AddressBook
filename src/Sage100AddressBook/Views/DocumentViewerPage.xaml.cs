using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

namespace Sage100AddressBook.Views
{
    public sealed partial class DocumentViewerPage : Page
    {

        public DocumentViewerPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    //Browser.NavigationCompleted += Browser_NavigationCompleted;

        //    ////Browser.Navigate(new Uri("http://www.sage.com/sage-summit"));
        //    //var googleDocsViewer = "http://drive.google.com/viewerng/viewer?embedded=true&url=";
        //    //var pdf = "https://swmsagedev-my.sharepoint.com/personal/steve_swmsagedev_onmicrosoft_com/_layouts/15/guestaccess.aspx?guestaccesstoken=5T9hTXjlxFe9qJQm1DUPHC9p%2bCUQEKHT8syowQa747E%3d&docid=0823727dd340d4ae884dbc9525a2932ed";
        //    //Browser.Navigate(new Uri(googleDocsViewer + pdf));


        //}

        //private void Browser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        //{
        //    Debug.WriteLine(args.IsSuccess.ToString());
        //}
    }
}
