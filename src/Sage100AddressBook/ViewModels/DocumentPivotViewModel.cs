/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Microsoft.Graph;
using Sage100AddressBook.CustomControls;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.DocumentViewerServices;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// ViewModel for the document pivot in the detail page.
    /// </summary>
    public class DocumentPivotViewModel : BindableBase
    {
        #region Private constants

        private const int PivotIndex = 1;

        #endregion

        #region Private fields

        private ObservableCollectionEx<DocumentGroup> _documentGroups = new ObservableCollectionEx<DocumentGroup>();
        private ObservableCollectionEx<DocumentEntry> _documents = new ObservableCollectionEx<DocumentEntry>();
        private ObservableCollectionEx<DocumentFolder> _folders = new ObservableCollectionEx<DocumentFolder>();
        private CustomerDetailPageViewModel _owner;
        private DelegateCommand<SearchControl> _search;
        private DelegateCommand _closeSearch;
        private DocumentEntry _document;
        private DelegateCommand _delete;
        private DelegateCommand _upload;
        private DelegateCommand _open;
        private string _companyCode;
        private string _rootId;
        private int _index = (-1);
        private bool _isSearch;

        #endregion

        #region Private methods

        /// <summary>
        /// Callback for search execution.
        /// </summary>
        /// <param name="sender">The sender of the event, which is the search control.</param>
        /// <param name="arg">The search event arguments.</param>
        private async void OnSearchResults(object sender, SearchEventArgs arg)
        {
            if (string.IsNullOrEmpty(arg.SearchText)) return;

            var source = new ObservableCollectionEx<DocumentEntry>();
            var dest = new ObservableCollectionEx<DocumentEntry>();

            source.Set(await DocumentRetrievalService.Instance.RetrieveDocumentsAsync(_rootId, _companyCode));

            var found = await DocumentRetrievalService.Instance.FindDocumentsAsync(_rootId, arg.SearchText);

            foreach (var item in found)
            {
                var match = source.FirstOrDefault(e => e.Id.Equals(item.Id));

                if (match != null) dest.Add(match);
            }

            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    _documents.Set(dest);
                    BuildDocumentGroups();
                }
                finally
                {
                    _isSearch = true;
                }

                RaisePropertyChanged("DocumentCloseSearchVisible");
            });
        }

        /// <summary>
        /// Show the search control.
        /// </summary>
        private void ShowSearch(SearchControl arg)
        {
            arg?.ShowSearch(OnSearchResults);
        }

        /// <summary>
        /// Closes the search results and displays all documents.
        /// </summary>
        private async void CloseSearchResults()
        {
            _isSearch = false;

            Loading = true;

            try
            {
                _documents.Set(await DocumentRetrievalService.Instance.RetrieveDocumentsAsync(_rootId, _companyCode));
            }
            finally
            {
                BuildDocumentFolders();
                BuildDocumentGroups();

                Loading = false;
            }

            RaisePropertyChanged("DocumentCloseSearchVisible");
        }

        /// <summary>
        /// Ensure the app is not snapped.
        /// </summary>
        /// <returns>True if application is unsnapped.</returns>
        private async Task<bool> EnsureUnsnapped()
        {
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());

            if (!unsnapped)
            {
                await Dialogs.Show("Cannot unsnap the application.");
            }

            return unsnapped;
        }

        /// <summary>
        /// Builds the list of document folders for upload use.
        /// </summary>
        private void BuildDocumentFolders()
        {
            var folders = from document in _documents
                          group document by new { document.Folder, document.FolderId } into grp
                          orderby grp.Key.Folder descending
                          select new DocumentFolder(grp.Key.FolderId, grp.Key.Folder);

            _folders.Set(folders);
        }

        /// <summary>
        /// Builds the document groups from the list of documents.
        /// </summary>
        private void BuildDocumentGroups()
        {
            var grouped = from document in _documents group document by new { document.Folder, document.FolderId } into grp
                          orderby grp.Key.Folder descending
                          select new DocumentGroup
                          {
                              GroupName = grp.Key.Folder,
                              GroupId = grp.Key.FolderId,
                              DocumentEntries = grp.ToList()
                          };

            _documentGroups.Set(grouped.ToList());

            RaisePropertyChanged("IsEmpty");
        }

        /// <summary>
        /// Opens the document.
        /// </summary>
        /// <returns>The async task to wait on.</returns>
        private async Task DoDocumentOpen()
        {
            var service = DocumentViewerService.Instance;

            if (_document == null) return;

            Loading = true;

            var fileName = string.Empty;

            try
            {
                var client = AuthenticationHelper.GetClient();

                if (client == null) return;

                using (var stream = await service.GetFileStream(_document.Id))
                {
                    fileName = await service.SaveToFile(stream, _document.Name);
                    await service.LaunchFileAssociation(fileName);
                }
            }
            catch (Exception exception)
            {
                await Dialogs.ShowException(string.Format("Failed to launch the file '{0}'.", fileName), exception, false);
            }
            finally
            {
                Loading = false;
            }
        }

        /// <summary>
        /// Routes to the document open task.
        /// </summary>
        private async void OpenDocument()
        {
            await DoDocumentOpen();
        }

        /// <summary>
        /// Deletes the document from one drive.
        /// </summary>
        private async void DeleteDocument()
        {
            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                var client = await AuthenticationHelper.GetClient();

                if ((client == null) || (_document == null)) return;

                if (!(await Dialogs.ShowOkCancel(string.Format("Delete the document \"{0}\"?", _document.Name)))) return;

                Loading = true;

                try
                {
                    await client.Me.Drive.Items[_document.Id].Request().DeleteAsync();

                    _documents.Remove(_document);

                    BuildDocumentGroups();

                    _document = null;
                }
                finally
                {
                    Loading = false;
                }
            });
        }

        /// <summary>
        /// Uploads a document to one drive.
        /// </summary>
        private async void UploadDocument()
        {
            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                var client = await AuthenticationHelper.GetClient();

                if (client == null) return;

                var index = await Dialogs.ShowSelection("Select a group to upload to.", _folders.ToList<object>());

                if (index < 0) return;

                var folder = _folders[index];

                if (!(await EnsureUnsnapped())) return;

                var openPicker = new FileOpenPicker();

                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".pdf");
                openPicker.FileTypeFilter.Add(".doc");
                openPicker.FileTypeFilter.Add(".docx");
                openPicker.FileTypeFilter.Add(".xls");
                openPicker.FileTypeFilter.Add(".xlsx");
                openPicker.FileTypeFilter.Add(".ppt");
                openPicker.FileTypeFilter.Add(".pptx");

                var file = await openPicker.PickSingleFileAsync();

                if (file == null) return;

                Loading = true;

                try
                {
                    var fileName = file.Name;

                    using (var stream = await file.OpenReadAsync())
                    {
                        using (var upload = stream.AsStreamForRead())
                        {
                            var driveItem = await client.Me.Drive.Items[folder.Id].Children[fileName].Content.Request().PutAsync<DriveItem>(upload);

                            var entry = new DocumentEntry()
                            {
                                Folder = folder.Name,
                                FolderId = folder.Id,
                                Id = driveItem.Id,
                                Name = driveItem.Name,
                                LastModifiedDate = driveItem.LastModifiedDateTime?.DateTime.ToLocalTime()
                            };

                            _documents.Add(entry);

                            BuildDocumentGroups();
                        }
                    }
                }
                finally
                {
                    Loading = false;
                }
            });
        }

        /// <summary>
        /// Determines if we have a current document.
        /// </summary>
        /// <returns></returns>
        private bool HasDocument()
        {
            return (_document != null);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DocumentPivotViewModel(CustomerDetailPageViewModel owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
            _search = new DelegateCommand<SearchControl>(new Action<SearchControl>(ShowSearch));
            _closeSearch = new DelegateCommand(new Action(CloseSearchResults));
            _open = new DelegateCommand(new Action(OpenDocument), HasDocument);
            _upload = new DelegateCommand(new Action(UploadDocument));
            _delete = new DelegateCommand(new Action(DeleteDocument), HasDocument);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads the documents from the retrieval service.
        /// </summary>
        /// <param name="id">The id of the folder to load.</param>
        /// <param name="companyCode">The company code associated with the id.</param>
        /// <returns>The async task to wait on.</returns>
        public async Task Load(string id, string companyCode)
        {
            try
            {
                _rootId = id;
                _companyCode = companyCode;
                _documents.Set(await DocumentRetrievalService.Instance.RetrieveDocumentsAsync(id, companyCode));
            }
            finally
            {
                BuildDocumentFolders();
                BuildDocumentGroups();
            }
        }

        /// <summary>
        /// Sets the current pivot index.
        /// </summary>
        /// <param name="index">The new pivot index being maintained by the page.</param>
        public void SetPivotIndex(int index)
        {
            try
            {
                _index = index;
            }
            finally
            {
                RaisePropertyChanged("DocumentCommandsVisible");
                RaisePropertyChanged("DocumentCloseSearchVisible");
            }
        }

        /// <summary>
        /// Event that is triggered when the selected document changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void DocumentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _document = (sender as GridView)?.SelectedItem as DocumentEntry;
            }
            finally
            {
                _open.RaiseCanExecuteChanged();
                _delete.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Event that is triggered when the document is double tapped.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public async void DocumentDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            await _owner.Dispatcher.DispatchAsync(async () => 
            {
                if (_document == null) return;
                await DoDocumentOpen();
            });
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Collection of document groups.
        /// </summary>
        public ObservableCollectionEx<DocumentGroup> DocumentGroups
        {
            get { return _documentGroups; }
        }

        /// <summary>
        /// The command handler for invoking search.
        /// </summary>
        public DelegateCommand<SearchControl> Search
        {
            get { return _search; }
        }

        /// <summary>
        /// Closes the search results and reloads all documents.
        /// </summary>
        public DelegateCommand CloseSearch
        {
            get { return _closeSearch; }
        }

        /// <summary>
        /// Opens the current document.
        /// </summary>
        public DelegateCommand Open
        {
            get { return _open; }
        }

        /// <summary>
        /// Upoads a new document.
        /// </summary>
        public DelegateCommand Upload
        {
            get { return _upload; }
        }

        /// <summary>
        /// Deletes an existing document.
        /// </summary>
        public DelegateCommand Delete
        {
            get { return _delete; }
        }

        /// <summary>
        /// Determines if the view is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return (_documents.Count == 0); }
        }

        /// <summary>
        /// Determine if the close search command is visible.
        /// </summary>
        public bool DocumentCloseSearchVisible
        {
            get { return (DocumentCommandsVisible && _isSearch); }
        }

        /// <summary>
        /// Determines if the document commands are available.
        /// </summary>
        public bool DocumentCommandsVisible
        {
            get { return (_index == PivotIndex); }
        }

        /// <summary>
        /// True if loading.
        /// </summary>
        public bool Loading
        {
            get { return _owner.Loading; }
            set
            {
                _owner.Loading = value;

                RaisePropertyChanged("Loading");
            }
        }

        #endregion
    }
}
