/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Storage;
using Microsoft.Graph;
using Windows.UI.Xaml;

/*
 *  Application Registration:
 * 
 * https://github.com/microsoftgraph/msgraph-sdk-dotnet/blob/master/README.md
 * 
 */

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Helper class for authentication.
    /// </summary>
    public class AuthenticationHelper : BindableBase
    {
        #region Private constants

        private const string DisplayName = "DisplayName";
        private const string RefreshToken = "RefreshToken";
        private const string User = "User";

        #endregion

        #region Private properties

        private static AuthenticationHelper _instance = new AuthenticationHelper();
        private static GraphServiceClient _graphClient;
        private string _clientId = App.Current.Resources["ida:ClientID"].ToString();
        private string _redirectUrl = App.Current.Resources["ida:ReturnUrl"].ToString();
        private string _userName;

        #endregion

        #region Private methods

        /// <summary>
        /// Aquires a new authenticated instance of the GraphClient.
        /// </summary>
        /// <returns></returns>
        private async Task AquireGraphClient()
        {
            var authenticationProvider = new OAuth2AuthenticationProvider(_clientId, _redirectUrl, new string[]
                {
                    "offline_access",
                    "https://graph.microsoft.com/Directory.ReadWrite.All",
                    "https://graph.microsoft.com/Directory.AccessAsUser.All",
                });

            try
            {
                await authenticationProvider.AuthenticateAsync();

                Client = new GraphServiceClient(authenticationProvider);
            }
            catch
            {
                Client = null;
            }
        }

        /// <summary>
        /// Notifies listeners on the SignedInChanged event that state has changed.
        /// </summary>
        private void Notify()
        {
            var handler = SignedInChanged;

            if (handler == null) return;

            handler(this, new RoutedEventArgs());
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private AuthenticationHelper()
        {
            _userName = (string)ApplicationData.Current.LocalSettings.Values[User];
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Attempts to sign the user in and obtain the Microsoft Graph access token.
        /// </summary>
        /// <returns>The async task which can be awaited.</returns>
        public async Task SignIn()
        {
            Views.Busy.SetBusy(true, "Signing in...");

            try
            {
                if (_graphClient == null) await AquireGraphClient();
                if (_graphClient != null)
                {
                    var me = await _graphClient.Me.Request().GetAsync();

                    UserName = me.DisplayName;
                }

                Notify();
            }
            finally
            {
                Views.Busy.SetBusy(false);
            }
        }

        /// <summary>
        /// Attempts to sign the user out of Microsoft Graph.
        /// </summary>
        /// <returns>The async task which can be awaited.</returns>
        public void SignOut()
        {
            Views.Busy.SetBusy(true, "Signing out...");

            try
            {
                Client = null;
                Token = null;
                UserName = null;

                Notify();
            }
            finally
            {
                Views.Busy.SetBusy(false);
            }
        }

        /// <summary>
        /// If signedin, returns the Client from the static instance, otherwise a UI based authentication is 
        /// performed to sign the user in.
        /// </summary>
        /// <returns>The graph client.</returns>
        public async static Task<GraphServiceClient> GetClient()
        {
            if (_graphClient != null) return _graphClient;

            await Instance.SignIn();

            return _graphClient;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Singleton instance of the 
        /// </summary>
        public static AuthenticationHelper Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// The currently signed in user.
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set
            {
                ApplicationData.Current.LocalSettings.Values[User] = _userName = value;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The stored refresh token.
        /// </summary>
        public string Token
        {
            get { return (string)ApplicationData.Current.LocalSettings.Values[RefreshToken]; }
            set
            {
                ApplicationData.Current.LocalSettings.Values[RefreshToken] = value;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The Microsoft Graph service client.
        /// </summary>
        public GraphServiceClient Client
        {
            get { return _graphClient; }
            set
            {
                _graphClient = value;
                       
                base.RaisePropertyChanged();
                RaisePropertyChanged("SignedIn");
            }
        }

        /// <summary>
        /// Returns true if the user is signed in, otherwise false.
        /// </summary>
        public bool SignedIn
        {
            get { return (_graphClient != null); }
        }

        /// <summary>
        /// Event for notifying listeners of authentication change.
        /// </summary>
        public RoutedEventHandler SignedInChanged;

        #endregion
    }
}
