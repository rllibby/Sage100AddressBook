/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Security.Authentication.Web;
using Windows.Security.Authentication.Web.Core;
using Windows.Storage;
using Windows.UI.Xaml;

#if DEBUG
using System.Diagnostics;
#endif

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Helper class for authentication.
    /// </summary>
    public class AuthenticationHelper : BindableBase
    {
        #region Private constants

        private const string AccountId = "AccountId";
        private const string DisplayName = "DisplayName";
        private const string TokenExpires = "TokenExpiresOn";
        private const string User = "User";

        #endregion

        #region Private properties

        private static AuthenticationHelper _instance = new AuthenticationHelper();
        private DateTime _tokenExpiration = DateTime.Now;
        private string _userName;
        private string _token;

        #endregion

        #region Private methods

#if DEBUG
        /// <summary>
        /// This is only used to get the application redirect to use for the application registration.
        /// </summary>
        /// <remarks>
        /// To register your application, the following must me done:
        /// 
        /// 1   -   Build the application and run it, making a call to this method.
        /// 2   -   Evaluate the uri and save the string value which will be used for application registration.
        /// 3   -   Create a new native app entry in Microsoft Graph; https://dev.office.com/app-registration
        /// 4   -   Paste the value from step 2 into the Redirect URI field.
        /// 5   -   Register the app and copy the Client ID.
        /// 6   -   Add the client ID to your application, which will be used during the authentication process.
        /// </remarks>
        private void GetAppRedirect()
        {
            var uri = string.Format("ms-appx-web://Microsoft.AAD.BrokerPlugIn/{0}", WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host.ToUpper());

            Debug.WriteLine(uri);
        }
#endif

        /// <summary>
        /// Makes the web request to authenticate.
        /// </summary>
        /// <param name="request">The web token request.</param>
        /// <param name="silent">True if this should be a silent (no UI) request.</param>
        /// <returns>The web token response.</returns>
        private async Task<WebTokenResponse> Authenticate(WebTokenRequest request, bool silent)
        {
            if (request == null) throw new ArgumentNullException("request");

            var requestResult = await (silent ? WebAuthenticationCoreManager.GetTokenSilentlyAsync(request) : WebAuthenticationCoreManager.RequestTokenAsync(request));

            if (requestResult.ResponseStatus == WebTokenRequestStatus.Success)
            {
                var tokenResponse = requestResult.ResponseData[0];

                foreach (var property in tokenResponse.Properties)
                {
                    if (property.Key.Equals(DisplayName)) UserName = property.Value;
                    if (property.Key.Equals(TokenExpires))
                    {
                        var expiresOnWin32Ticks = double.Parse(property.Value);
                        var epochStart = new DateTime(1601, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

                        _tokenExpiration = epochStart.AddSeconds(expiresOnWin32Ticks).ToLocalTime();
                    }
                }

                ApplicationData.Current.LocalSettings.Values.Remove(AccountId);
                ApplicationData.Current.LocalSettings.Values[AccountId] = requestResult.ResponseData[0].WebAccount.Id;

                return tokenResponse;
            }

            return null;
        }

        /// <summary>
        /// If the account id has previously been obtained, then attempts to silently get the access token. If this 
        /// fails, or accountId is null, then a prompted login will be used to obtain the access token.
        /// </summary>
        /// <returns>The access token for the Graph API.</returns>
        private async Task<string> GetAccessTokenForGraph()
        {
            var clientId = App.Current.Resources["ida:ClientID"].ToString();
            var accountId = (string)ApplicationData.Current.LocalSettings.Values[AccountId];
            var accountProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync("https://login.windows.net");
            var tokenRequest = new WebTokenRequest(accountProvider, string.Empty, clientId, (string.IsNullOrEmpty(accountId) ? WebTokenRequestPromptType.ForceAuthentication : WebTokenRequestPromptType.Default));

            tokenRequest.Properties.Add("authority", "https://login.windows.net");
            tokenRequest.Properties.Add("resource", "https://graph.microsoft.com/");

            if (tokenRequest.PromptType == WebTokenRequestPromptType.Default)
            {
                var silentResponse = await Authenticate(tokenRequest, true);

                if (silentResponse != null) return silentResponse.Token;
            }

            tokenRequest = new WebTokenRequest(accountProvider, string.Empty, clientId, WebTokenRequestPromptType.ForceAuthentication);
            tokenRequest.Properties.Add("authority", "https://login.windows.net");
            tokenRequest.Properties.Add("resource", "https://graph.microsoft.com/");

            var response = await Authenticate(tokenRequest, false);

            return (response == null) ? null : response.Token;
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
            _token = null;
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
                var token = await GetAccessTokenForGraph();

                if (Token != token)
                {
                    Token = token;
                    Notify();
                }
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
        public async Task SignOut()
        {
            Views.Busy.SetBusy(true, "Signing out...");

            try
            {
                if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(AccountId)) return;

                var account = (string)ApplicationData.Current.LocalSettings.Values[AccountId];

                ApplicationData.Current.LocalSettings.Values.Remove(AccountId);

                var providerToDelete = await WebAuthenticationCoreManager.FindAccountProviderAsync("https://login.windows.net");
                var accountToDelete = await WebAuthenticationCoreManager.FindAccountAsync(providerToDelete, account);

                if (accountToDelete != null) await accountToDelete.SignOutAsync(App.Current.Resources["ida:ClientID"].ToString());

                UserName = null;
                Token = null;

                Notify();
            }
            finally
            {
                Views.Busy.SetBusy(false);
            }
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
        /// The access token for Microsoft Graph.
        /// </summary>
        public string Token
        {
            get
            {
                if (!string.IsNullOrEmpty(_token) && (_tokenExpiration < DateTime.Now))
                {
                    _token = null;

                    Notify();
                }

                return _token;
            }
            set
            {
                _token = value;

                base.RaisePropertyChanged();
                RaisePropertyChanged("SignedIn");
            }
        }

        /// <summary>
        /// Returns true if the user is signed in, otherwise false.
        /// </summary>
        public bool SignedIn
        {
            get { return !string.IsNullOrEmpty(Token); }
        }

        /// <summary>
        /// Event for notifying listeners of authentication change.
        /// </summary>
        public RoutedEventHandler SignedInChanged;

        #endregion
    }
}
