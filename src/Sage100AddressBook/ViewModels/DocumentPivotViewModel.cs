/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Microsoft.Graph;
using Newtonsoft.Json;
using Sage100AddressBook.CustomControls;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.DocumentViewerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// ViewModel for the document pivot in the detail page.
    /// </summary>
    public class DocumentPivotViewModel : BindableBase
    {
        #region Private constants

        private const string MetadataExtension = ".json.txt";
        private const int PivotIndex = 1;

        #endregion

        #region Private fields

        private ObservableCollectionEx<DocumentGroup> _documentGroups = new ObservableCollectionEx<DocumentGroup>();
        private ObservableCollectionEx<DocumentEntry> _documents = new ObservableCollectionEx<DocumentEntry>();
        private ObservableCollectionEx<DocumentFolder> _folders = new ObservableCollectionEx<DocumentFolder>();
        private CustomerDetailPageViewModel _owner;
        private SearchControl _searchControl;
        private DataTransferManager _dataTransferManager;
        private DelegateCommand<SearchControl> _search;
        private DelegateCommand<SearchControl> _closeSearch;
        private DocumentEntry _document;
        private DataPackage _shareData;
        private DelegateCommand _share;
        private DelegateCommand _delete;
        private DelegateCommand _rename;
        private DelegateCommand _move;
        private DelegateCommand _upload;
        private DelegateCommand _open;
        private string _searchText;
        private string _companyCode;
        private string _rootId;
        private int _index = (-1);
        private bool _isSearch;

        #endregion

        #region Private methods

        /// <summary>
        /// Used to pre-process image files so that they are scaled down to a reasonable size.
        /// </summary>
        /// <param name="file">The file to process.</param>
        /// <returns>The existing or new storage file.</returns>
        private async Task<StorageFile> PreProcessFile(StorageFile file)
        {
            if (file == null) throw new ArgumentNullException("file");

            if (!(string.Equals(Path.GetExtension(file.Name), ".jpg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Path.GetExtension(file.Name), ".png", StringComparison.OrdinalIgnoreCase))) return file;

            using (var stream = await file.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);

                var origHeight = decoder.PixelHeight;
                var origWidth = decoder.PixelWidth;

                var ratioX = 1000 / (double)origWidth;
                var ratioY = 1000 / (double)origHeight;
                var ratio = Math.Min(ratioX, ratioY);

                var newHeight = (uint)(origHeight * ratio);
                var newWidth = (uint)(origWidth * ratio);

                using (var encoderStream = new InMemoryRandomAccessStream())
                {
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(encoderStream, decoder);
                    encoder.BitmapTransform.ScaledHeight = newHeight;
                    encoder.BitmapTransform.ScaledWidth = newWidth;

                    await encoder.FlushAsync();

                    var pixels = new byte[newWidth * newHeight * 4];

                    await encoderStream.ReadAsync(pixels.AsBuffer(), (uint)pixels.Length, InputStreamOptions.None);

                    var writeable = new 
                    using (var memstream = new MemoryStream())
                    {
                        await memstream.WriteAsync(pixels, 0, pixels.Length);

                        var folder = ApplicationData.Current.TemporaryFolder;
                        var newFile = await folder.CreateFileAsync(file.Name, CreationCollisionOption.GenerateUniqueName);

                        using (var filestream = await file.OpenStreamForWriteAsync())
                        {
                            await memstream.CopyToAsync(filestream);
                        }

                        file = newFile;
                    }
                }
            }

            return file;
        }

        /// <summary>
        /// Extracts the text from the image to use for metadata.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>The text that was extracted from the image.</returns>
        private async Task<string> ProcessBitmap(SoftwareBitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException("bitmap");

            var results = new StringBuilder(4096);

            if ((bitmap.PixelWidth > OcrEngine.MaxImageDimension) || (bitmap.PixelHeight > OcrEngine.MaxImageDimension)) return string.Empty;

            var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();

            if (ocrEngine == null) return string.Empty;

            var ocrResult = await ocrEngine.RecognizeAsync(bitmap);

            foreach (var line in ocrResult.Lines)
            {
                foreach (var word in line.Words)
                {
                    results.Append(word.Text.Trim(new[] { ' ', ',', '.', '/', '\\', ':', '-', ';', '<', '>', '(', ')', '"', '\'' }));
                    results.Append(" ");
                }
            }

            return results.ToString().Trim();
        }

        /// <summary>
        /// Performs metadata processing for .png and .jpg files.
        /// </summary>
        /// <param name="client">The graph client to use for uploading metadata.</param>
        /// <param name="file">The file that was uploaded.</param>
        /// <param name="folderId">The graph folder id where the file was uploaded.</param>
        /// <param name="id">The graph id for the uploaded file.</param>
        /// <returns></returns>
        private async Task<DriveItem> ProcessMetadata(GraphServiceClient client, StorageFile file, string folderId, string id)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (file == null) throw new ArgumentNullException("file");
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

            if (!(string.Equals(Path.GetExtension(file.Name), ".jpg", StringComparison.OrdinalIgnoreCase) 
                || string.Equals(Path.GetExtension(file.Name), ".png", StringComparison.OrdinalIgnoreCase))) return null;

            using (var imageStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(imageStream);
                var bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                var text = await ProcessBitmap(bitmap);

                if (string.IsNullOrEmpty(text)) return null;

                var metadata = new ImageMetadata()
                {
                    ImageId = id,
                    Content = text
                };

                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(metadata))))
                {
                    return await client.Me.Drive.Items[folderId].Children[file.Name + MetadataExtension].Content.Request().PutAsync<DriveItem>(memoryStream);
                }
            }
        }

        /// <summary>
        /// Callback for search execution.
        /// </summary>
        /// <param name="sender">The sender of the event, which is the search control.</param>
        /// <param name="arg">The search event arguments.</param>
        private async void OnSearchResults(object sender, SearchEventArgs arg)
        {
            if (string.IsNullOrEmpty(arg.SearchText)) return;

            var search = arg.SearchText;

            await _owner.Dispatcher.DispatchAsync(async() =>
            {
                try
                {
                    Loading = true;
                    SearchText = search;

                    try
                    {
                        var source = new ObservableCollectionEx<DocumentEntry>();
                        var dest = new ObservableCollectionEx<DocumentEntry>();

                        source.Set(await DocumentRetrievalService.Instance.RetrieveDocumentsAsync(_rootId, _companyCode, _folders));

                        var found = await DocumentRetrievalService.Instance.FindDocumentsAsync(_rootId, search);

                        foreach (var item in found)
                        {
                            var match = source.FirstOrDefault(e => e.Id.Equals(item.Id));

                            if (match != null) dest.Add(match);
                        }

                        _documents.Set(dest);
                        BuildDocumentGroups();
                    }
                    finally
                    {
                        Loading = false;
                    }
                }
                catch (Exception exception)
                {
                    await Dialogs.ShowException(string.Format("Failed to search the documents for '{0}'.", search), exception, false);
                }
            });
        }

        /// <summary>
        /// Event that is called when data is going to be shared.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">The event arguments.</param>
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            args.Request.Data = _shareData;
        }

        /// <summary>
        /// Show the search control.
        /// </summary>
        private void ShowSearch(SearchControl arg)
        {
            _searchControl = arg;
            _searchControl?.ShowSearch(OnSearchResults);
        }

        /// <summary>
        /// Action to close search results.
        /// </summary>
        /// <param name="arg">The search control.</param>
        private async void CloseSearchAction(SearchControl arg)
        {
            await CloseSearchResults(arg);
        }

        /// <summary>
        /// Closes the search results and displays all documents.
        /// </summary>
        private async Task CloseSearchResults(SearchControl arg)
        {
            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                Loading = true;
                SearchText = string.Empty;

                try
                {
                    arg?.CloseSearch();

                    _searchControl = null;
                    _documentGroups.Clear();
                    _documents.Set(await DocumentRetrievalService.Instance.RetrieveDocumentsAsync(_rootId, _companyCode, _folders));

                    BuildDocumentGroups();
                }
                finally
                {
                    Loading = false;
                }
            });
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
        /// Shares the current document.
        /// </summary>
        private async void ShareDocument()
        {
            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                Loading = true;

                try
                {
                    var client = await AuthenticationHelper.GetClient();

                    if ((client == null) || (_document == null)) return;

                    var list = new List<string> { "View only link", "Edit link" };
                    var index = await Dialogs.SelectLink();

                    if (index < 0) return;

                    var link = await client.Me.Drive.Items[_document.Id].CreateLink((index == 0) ? "view" : "edit").Request().PostAsync();

                    _shareData = new DataPackage();
                    _shareData.Properties.Title = _document.Name;
                    _shareData.Properties.Description = string.Format("{0} link for document '{1}',", (index == 0) ? "View only" : "Edit", _document.Name);
                    _shareData.SetWebLink(new Uri(link.Link.WebUrl));

                    DataTransferManager.ShowShareUI();
                }
                finally
                {
                    Loading = false;
                }
            });
        }

        /// <summary>
        /// Routes to the document open task.
        /// </summary>
        private async void OpenDocument()
        {
            await DoDocumentOpen();
        }

        /// <summary>
        /// Renames the document.
        /// </summary>
        private async void RenameDocument()
        {
            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                try
                {
                    var client = await AuthenticationHelper.GetClient();

                    if ((client == null) || (_document == null)) return;

                    var rename = await Dialogs.Rename(_document.Name);

                    if (string.IsNullOrEmpty(rename)) return;
                    if (string.IsNullOrEmpty(Path.GetExtension(rename)) )rename += Path.GetExtension(_document.Name) ?? string.Empty;

                    Loading = true;

                    try
                    {
                        var driveItem = new DriveItem()
                        {
                            Id = _document.Id,
                            Name = rename,
                            ParentReference = new ItemReference { Id = _document.FolderId }
                        };

                        driveItem = await client.Me.Drive.Items[_document.Id].Request().UpdateAsync(driveItem);

                        _document.Id = driveItem.Id;
                        _document.Name = driveItem.Name;

                        if (!string.IsNullOrEmpty(_document.MetadataId))
                        {
                            var metadataItem = new DriveItem()
                            {
                                Id = _document.MetadataId,
                                Name = rename + MetadataExtension,
                                ParentReference = new ItemReference { Id = _document.FolderId }
                            };

                            await client.Me.Drive.Items[_document.MetadataId].Request().UpdateAsync(metadataItem);
                        }

                        BuildDocumentGroups();
                    }
                    finally
                    {
                        Loading = false;
                    }
                }
                catch (Exception exception)
                {
                    await Dialogs.ShowException(string.Format("Failed to rename the document '{0}'.", _document.Name), exception, false);
                }
            });
        }               

        /// <summary>
        /// Moves the document to a new folder.
        /// </summary>
        private async void MoveDocument()
        {
            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                try
                {
                    var client = await AuthenticationHelper.GetClient();

                    if ((client == null) || (_document == null)) return;

                    var index = await Dialogs.SelectGroup(GroupOperation.Move, _folders, _rootId);

                    Loading = true;

                    try
                    {
                        if (index < 0) return;

                        var folder = _folders[index];

                        if (folder.Id.Equals(_document.FolderId)) return;

                        var driveItem = new DriveItem()
                        {
                            Id = _document.Id,
                            Name = _document.Name,
                            ParentReference = new ItemReference { Id = folder.Id }
                        };

                        driveItem = await client.Me.Drive.Items[_document.Id].Request().UpdateAsync(driveItem);

                        _document.Id = driveItem.Id;
                        _document.Folder = folder.Name;
                        _document.FolderId = folder.Id;

                        if (!string.IsNullOrEmpty(_document.MetadataId))
                        {
                            var metadataItem = new DriveItem()
                            {
                                Id = _document.MetadataId,
                                Name = _document.Name + MetadataExtension,
                                ParentReference = new ItemReference { Id = folder.Id }
                            };

                            await client.Me.Drive.Items[_document.MetadataId].Request().UpdateAsync(metadataItem);
                        }

                        BuildDocumentGroups();
                    }
                    finally
                    {
                        Loading = false;
                    }
                }
                catch (Exception exception)
                {
                    await Dialogs.ShowException(string.Format("Failed to move the document '{0}'.", _document.Name), exception, false);
                }
            });
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

                    if (!string.IsNullOrEmpty(_document.MetadataId))
                    {
                        await client.Me.Drive.Items[_document.MetadataId].Request().DeleteAsync();
                    }

                    _documents.Remove(_document);
                    _document = null;

                    BuildDocumentGroups();
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

                var index = await Dialogs.SelectGroup(GroupOperation.Upload, _folders, _rootId);

                if (index < 0) return;

                var folder = _folders[index];

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
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".png");

                DriveItem driveItem = null;
                DocumentEntry entry = null;

                var file = await openPicker.PickSingleFileAsync();

                if (file == null) return;

                Loading = true;

                try
                {
                    file = await PreProcessFile(file);

                    var fileName = file.Name;

                    using (var stream = await file.OpenReadAsync())
                    {
                        if (stream.Size > 4096000)
                        {
                            await Dialogs.Show("Upload content must be less than 4 MB in size!");
                            return;
                        }

                        using (var upload = stream.AsStreamForRead())
                        {
                            driveItem = await client.Me.Drive.Items[folder.Id].Children[fileName].Content.Request().PutAsync<DriveItem>(upload);

                            entry = new DocumentEntry()
                            {
                                Folder = folder.Name,
                                FolderId = folder.Id,
                                Id = driveItem.Id,
                                Name = driveItem.Name,
                                LastModifiedDate = driveItem.LastModifiedDateTime?.DateTime.ToLocalTime()
                            };

                            _documents.Add(entry);
                        }
                    }

                    var metadataItem = await ProcessMetadata(client, file, folder.Id, driveItem.Id);

                    entry.MetadataId = (metadataItem == null) ? null : metadataItem.Id;

                    BuildDocumentGroups();
                }
                catch (Exception exception)
                {
                    await Dialogs.ShowException("Failed to upload file", exception, false);
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

            _dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferManager.DataRequested += OnDataRequested;
            _search = new DelegateCommand<SearchControl>(new Action<SearchControl>(ShowSearch));
            _closeSearch = new DelegateCommand<SearchControl>(new Action<SearchControl>(CloseSearchAction));
            _open = new DelegateCommand(new Action(OpenDocument), HasDocument);
            _move = new DelegateCommand(new Action(MoveDocument), HasDocument);
            _share = new DelegateCommand(new Action(ShareDocument), HasDocument);
            _upload = new DelegateCommand(new Action(UploadDocument));
            _delete = new DelegateCommand(new Action(DeleteDocument), HasDocument);
            _rename = new DelegateCommand(new Action(RenameDocument), HasDocument);
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
                _documents.Set(await DocumentRetrievalService.Instance.RetrieveDocumentsAsync(id, companyCode, _folders));
            }
            finally
            {
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
                _share.RaiseCanExecuteChanged();
                _open.RaiseCanExecuteChanged();
                _delete.RaiseCanExecuteChanged();
                _move.RaiseCanExecuteChanged();
                _rename.RaiseCanExecuteChanged();
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
        public DelegateCommand<SearchControl> CloseSearch
        {
            get { return _closeSearch; }
        }

        /// <summary>
        /// Shares the current document.
        /// </summary>
        public DelegateCommand Share
        {
            get { return _share; }
        }

        /// <summary>
        /// Opens the current document.
        /// </summary>
        public DelegateCommand Open
        {
            get { return _open; }
        }

        /// <summary>
        /// Moves the current document.
        /// </summary>
        public DelegateCommand Move
        {
            get { return _move; }
        }

        /// <summary>
        /// Renames the current document.
        /// </summary>
        public DelegateCommand Rename
        {
            get { return _rename; }
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
        /// Determines if search is active.
        /// </summary>
        public bool IsSearch
        {
            get { return _isSearch; }
            set { Set(ref _isSearch, value); }
        }

        /// <summary>
        /// The currently active search text.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                IsSearch = !string.IsNullOrEmpty(_searchText);
                if (IsSearch) _searchText = string.Format("Search: '{0}'", _searchText);
                RaisePropertyChanged("DocumentCloseSearchVisible");

                base.RaisePropertyChanged();
            }
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
