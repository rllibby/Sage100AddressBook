/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sage100AddressBook.Views
{
    /// <summary>
    /// Splash screen view handler.
    /// </summary>
    public sealed partial class Splash : UserControl
    {
        #region Private methods

        /// <summary>
        /// Resize the image to fit the current display.
        /// </summary>
        /// <param name="splashScreen">The splash screen.</param>
        private void Resize(SplashScreen splashScreen)
        {
            if (splashScreen.ImageLocation.Top == 0)
            {
                splashImage.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                rootCanvas.Background = null;
                splashImage.Visibility = Visibility.Visible;
            }

            splashImage.Height = splashScreen.ImageLocation.Height;
            splashImage.Width = splashScreen.ImageLocation.Width;
            splashImage.SetValue(Canvas.TopProperty, splashScreen.ImageLocation.Top);
            splashImage.SetValue(Canvas.LeftProperty, splashScreen.ImageLocation.Left);
            ProgressTransform.TranslateY = splashImage.Height / 2;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="splashScreen">The splash screen to host.</param>
        public Splash(SplashScreen splashScreen)
        {
            InitializeComponent();
            Window.Current.SizeChanged += (s, e) => Resize(splashScreen);
            Resize(splashScreen);
        }

        #endregion
    }
}

