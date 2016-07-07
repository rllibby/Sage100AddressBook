/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model for the documents page.
    /// </summary>
    public class DocumentsPageViewModel : ViewModelLoading
    {
        #region Private constants

        private const string Documents = "Documents";
        private const string CompanyCode = "abc";

        #endregion

        #region Private fields

        private DocumentPivotViewModel _documentModel;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DocumentsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { }

            _documentModel = new DocumentPivotViewModel(this);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Called when the page is being navigated to.
        /// </summary>
        /// <param name="parameter">The parameter passed during navigation.</param>
        /// <param name="mode">The navigation mode.</param>
        /// <param name="suspensionState">The saved state.</param>
        /// <returns>The async task to wait on.</returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Loading = true;

            try
            {
                _documentModel.SetPivotIndex(1);
                _documentModel.SetArguments(Documents, CompanyCode);
            }
            finally
            {
                Loading = false;
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Called when this view model is navigated from.
        /// </summary>
        /// <param name="suspensionState">The dictionary of application state.</param>
        /// <param name="suspending">True if application is suspending.</param>
        /// <returns>The async task.</returns>
        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            await Task.CompletedTask;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The model handling the document pivot page.
        /// </summary>
        public DocumentPivotViewModel DocumentModel
        {
            get { return _documentModel; }
        }

        #endregion
    }
}

