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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

#pragma warning disable 4014

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
        private List<DocumentEntry> _documents = new List<DocumentEntry>();
        private List<DocumentFolder> _folders = new List<DocumentFolder>();
        private ViewModelLoading _owner;
        private SearchControl _searchControl;
        private DataTransferManager _dataTransferManager;
        private DelegateCommand<SearchControl> _search;
        private DelegateCommand<SearchControl> _closeSearch;
        private DocumentEntry _current;
        private DataPackage _shareData;
        private DelegateCommand<DocumentEntry> _open;
        private DelegateCommand<DocumentEntry> _share;
        private DelegateCommand<DocumentEntry> _move;
        private DelegateCommand<DocumentEntry> _delete;
        private DelegateCommand<DocumentEntry> _rename;
        private DelegateCommand _upload;
        private string _searchText;
        private string _companyCode;
        private string _rootId;
        private int _index = (-1);
        private bool _isSearch;
        private bool _loading;

        #endregion

        #region Private methods

        /// <summary>
        /// Used to pre-process image files so that they are scaled down to a reasonable size.
        /// </summary>
        /// <param name="file">The file to process.</param>
        /// <returns>The existing or new storage file.</returns>
        private async Task<FileContent> PreProcessFile(StorageFile file)
        {
            if (file == null) throw new ArgumentNullException("file");

            var results = new FileContent(file);

            if (!(string.Equals(Path.GetExtension(file.Name), ".jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(Path.GetExtension(file.Name), ".png", StringComparison.OrdinalIgnoreCase))) return results;

            WriteableBitmap writeable = null;

            var origHeight = (uint)0;
            var origWidth = (uint)0;

            using (var stream = await file.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);

                var bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                if ((bitmap.PixelWidth < OcrEngine.MaxImageDimension) && (bitmap.PixelHeight < OcrEngine.MaxImageDimension))
                {
                    var temp = new StringBuilder(4096);
                    var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();

                    if (ocrEngine != null)
                    {
                        var ocrResult = await ocrEngine.RecognizeAsync(bitmap);

                        foreach (var line in ocrResult.Lines)
                        {
                            foreach (var word in line.Words)
                            {
                                temp.Append(word.Text.Trim(new[] { ' ', ',', '.', '/', '\\', ':', '-', ';', '<', '>', '(', ')', '"', '\'' }));
                                temp.Append(" ");
                            }
                        }
                    }

                    results.Content = temp.ToString();
                }

                var newHeight = origHeight = decoder.PixelHeight;
                var newWidth = origWidth = decoder.PixelWidth;

                if ((origWidth > 1000) || (origHeight > 1000))
                {
                    var ratioX = 1000 / (double)origWidth;
                    var ratioY = 1000 / (double)origHeight;
                    var ratio = Math.Min(ratioX, ratioY);

                    newHeight = (uint)(origHeight * ratio);
                    newWidth = (uint)(origWidth * ratio);
                }

                writeable = new WriteableBitmap((int)newWidth, (int)newHeight);

                using (var encoderStream = new InMemoryRandomAccessStream())
                {
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(encoderStream, decoder);

                    encoder.BitmapTransform.ScaledHeight = newHeight;
                    encoder.BitmapTransform.ScaledWidth = newWidth;

                    await encoder.FlushAsync();

                    writeable.SetSource(encoderStream);
                }
            }

            var newFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Path.ChangeExtension(file.Name, "png"), CreationCollisionOption.ReplaceExisting);

            using (var encoderStream = await newFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, encoderStream);
                var pixelStream = writeable.PixelBuffer.AsStream();
                var pixels = new byte[pixelStream.Length];

                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)writeable.PixelWidth, (uint)writeable.PixelHeight, 96.0, 96.0, pixels);

                await encoder.FlushAsync();
            }

            results.File = newFile;

            return results;
        }

        /// <summary>
        /// Performs metadata processing for .png and .jpg files.
        /// </summary>
        /// <param name="client">The graph client to use for uploading metadata.</param>
        /// <param name="item">The graph drive item that we will create metadata for.</param>
        /// <param name="content">The metadata file content.</param>
        /// <returns>The metadata drive item that was created.</returns>
        private async Task<DriveItem> ProcessMetadata(GraphServiceClient client, DriveItem item, string content)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (item == null) throw new ArgumentNullException("item");
            if (string.IsNullOrEmpty(content)) throw new ArgumentNullException("content");

            var metadata = new ImageMetadata()
            {
                ImageId = item.Id,
                Content = content
            };

            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(metadata))))
            {
                return await client.Me.Drive.Items[item.ParentReference.Id].Children[item.Name + MetadataExtension].Content.Request().PutAsync<DriveItem>(memoryStream);
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
            var dispatcher = _owner.Dispatcher;

            await dispatcher.DispatchAsync(async() =>
            {
                try
                {
                    Loading = true;
                    SearchText = search;

                    _documents.Clear();
                    BuildDocumentGroups();

                    Task.Run<IEnumerable<DocumentEntry>>(async () =>
                    {
                        var source = new List<DocumentEntry>();

                        _documents.Clear();
                        source.AddRange(await DocumentRetrievalService.Instance.RetrieveDocumentsAsync(_rootId, _companyCode, _folders));

                        var found = await DocumentRetrievalService.Instance.FindDocumentsAsync(_rootId, search);

                        foreach (var item in found)
                        {
                            var match = source.FirstOrDefault(e => e.Id.Equals(item.Id));

                            if (match != null) _documents.Add(match);
                        }

                        return _documents;

                    }).ContinueWith(async (t) =>
                    {
                        await dispatcher.DispatchAsync(async () =>
                        {
                            try
                            {
                                if (t.IsFaulted)
                                {
                                    if (t.Exception != null) await Dialogs.ShowException(string.Format("Failed to search the documents for '{0}'.", search), t.Exception, false);
                                }
                                if (t.IsCompleted) BuildDocumentGroups();
                            }
                            finally
                            {
                                Loading = false;
                            }
                        });
                    });
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
            var dispatcher = _owner.Dispatcher;

            await dispatcher.DispatchAsync(() =>
            {
                arg?.CloseSearch();

                SearchText = string.Empty;
               
                _searchControl = null;
                _documents.Clear();

                BuildDocumentGroups();

                if (_index == PivotIndex) Load();
            });
        }

        /// <summary>
        /// Builds the document groups from the list of documents.
        /// </summary>
        private void BuildDocumentGroups()
        {
            foreach (var document in _documents)
            {
                document.Open = _open;
                document.Share = _share;
                document.MoveTo = _move;
                document.Delete = _delete;
                document.Rename = _rename;
            }

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
        /// <param name="entry">The document the action should execute on.</param>
        /// <returns>The async task to wait on.</returns>
        private async Task DoDocumentOpen(DocumentEntry entry)
        {
            var service = DocumentViewerService.Instance;

            if (entry == null) return;

            Loading = true;

            var fileName = string.Empty;

            try
            {
                var client = AuthenticationHelper.GetClient();

                if (client == null) return;

                using (var stream = await service.GetFileStream(entry.Id))
                {
                    fileName = await service.SaveToFile(stream, entry.Name);

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
        /// <param name="entry">The document the action should execute on.</param>
        private async void ShareDocument(DocumentEntry entry)
        {
            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                Loading = true;

                try
                {
                    var client = await AuthenticationHelper.GetClient();

                    if ((client == null) || (entry == null)) return;

                    var list = new List<string> { "View only link", "Edit link" };
                    var index = await Dialogs.SelectLink();

                    if (index < 0) return;

                    var link = await client.Me.Drive.Items[entry.Id].CreateLink((index == 0) ? "view" : "edit").Request().PostAsync();

                    _shareData = new DataPackage();
                    _shareData.Properties.Title = entry.Name;
                    _shareData.Properties.Description = string.Format("{0} link for document '{1}',", (index == 0) ? "View only" : "Edit", entry.Name);
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
        /// <param name="entry">The document the action should execute on.</param>
        private async void OpenDocument(DocumentEntry entry)
        {
            await DoDocumentOpen(entry);
        }

        /// <summary>
        /// Renames the document.
        /// </summary>
        /// <param name="entry">The document the action should execute on.</param>
        private async void RenameDocument(DocumentEntry entry)
        {
            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                try
                {
                    var client = await AuthenticationHelper.GetClient();

                    if ((client == null) || (entry == null)) return;

                    var rename = await Dialogs.Rename(entry.Name);

                    if (string.IsNullOrEmpty(rename)) return;
                    if (string.IsNullOrEmpty(Path.GetExtension(rename))) rename += Path.GetExtension(entry.Name) ?? string.Empty;

                    Loading = true;

                    try
                    {
                        var driveItem = new DriveItem()
                        {
                            Id = entry.Id,
                            Name = rename,
                            ParentReference = new ItemReference { Id = entry.FolderId }
                        };

                        driveItem = await client.Me.Drive.Items[entry.Id].Request().UpdateAsync(driveItem);

                        entry.Id = driveItem.Id;
                        entry.Name = driveItem.Name;

                        if (!string.IsNullOrEmpty(entry.MetadataId))
                        {
                            var metadataItem = new DriveItem()
                            {
                                Id = entry.MetadataId,
                                Name = rename + MetadataExtension,
                                ParentReference = new ItemReference { Id = entry.FolderId }
                            };

                            await client.Me.Drive.Items[entry.MetadataId].Request().UpdateAsync(metadataItem);
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
                    await Dialogs.ShowException(string.Format("Failed to rename the document '{0}'.", entry.Name), exception, false);
                }
            });
        }

        /// <summary>
        /// Moves the document to a new folder.
        /// </summary>
        /// <param name="entry">The document the action should execute on.</param>
        private async void MoveDocument(DocumentEntry entry)
        {
            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                try
                {
                    var client = await AuthenticationHelper.GetClient();

                    if ((client == null) || (entry == null)) return;

                    var index = await Dialogs.SelectGroup(GroupOperation.Move, _folders, _rootId);

                    Loading = true;

                    try
                    {
                        if (index < 0) return;

                        var folder = _folders[index];

                        if (folder.Id.Equals(entry.FolderId)) return;

                        var driveItem = new DriveItem()
                        {
                            Id = entry.Id,
                            Name = entry.Name,
                            ParentReference = new ItemReference { Id = folder.Id }
                        };

                        driveItem = await client.Me.Drive.Items[entry.Id].Request().UpdateAsync(driveItem);

                        entry.Id = driveItem.Id;
                        entry.Folder = folder.Name;
                        entry.FolderId = folder.Id;

                        if (!string.IsNullOrEmpty(entry.MetadataId))
                        {
                            var metadataItem = new DriveItem()
                            {
                                Id = entry.MetadataId,
                                Name = entry.Name + MetadataExtension,
                                ParentReference = new ItemReference { Id = folder.Id }
                            };

                            await client.Me.Drive.Items[entry.MetadataId].Request().UpdateAsync(metadataItem);
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
                    await Dialogs.ShowException(string.Format("Failed to move the document '{0}'.", entry.Name), exception, false);
                }
            });
        }

        /// <summary>
        /// Deletes the document from one drive.
        /// </summary>
        /// <param name="entry">The document the action should execute on.</param>
        private async void DeleteDocument(DocumentEntry entry)
        {
            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                var client = await AuthenticationHelper.GetClient();

                if ((client == null) || (entry == null)) return;

                if (!(await Dialogs.ShowOkCancel(string.Format("Delete the document \"{0}\"?", entry.Name)))) return;

                Loading = true;

                try
                {
                    await client.Me.Drive.Items[entry.Id].Request().DeleteAsync();

                    if (!string.IsNullOrEmpty(entry.MetadataId))
                    {
                        await client.Me.Drive.Items[entry.MetadataId].Request().DeleteAsync();
                    }

                    _documents.Remove(entry);

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
                    var fileContent = await PreProcessFile(file);

                    using (var stream = await fileContent.File.OpenReadAsync())
                    {
                        if (stream.Size > 4096000)
                        {
                            await Dialogs.Show("Upload content must be less than 4 MB in size!");
                            return;
                        }

                        using (var upload = stream.AsStreamForRead())
                        {
                            driveItem = await client.Me.Drive.Items[folder.Id].Children[fileContent.File.Name].Content.Request().PutAsync<DriveItem>(upload);

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

                    if (!string.IsNullOrEmpty(fileContent.Content))
                    {
                        var metadataItem = await ProcessMetadata(client, driveItem, fileContent.Content);

                        entry.MetadataId = metadataItem.Id;
                    }

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
        /// <returns>True if the document is not null.</returns>
        private bool HasDocument(DocumentEntry entry)
        {
            return ((entry != null) || (_current != null));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DocumentPivotViewModel(ViewModelLoading owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;

            _dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferManager.DataRequested += OnDataRequested;
            _search = new DelegateCommand<SearchControl>(new Action<SearchControl>(ShowSearch));
            _closeSearch = new DelegateCommand<SearchControl>(new Action<SearchControl>(CloseSearchAction));
            _upload = new DelegateCommand(new Action(UploadDocument));
            _open = new DelegateCommand<DocumentEntry>(new Action<DocumentEntry>(OpenDocument), HasDocument);
            _move = new DelegateCommand<DocumentEntry>(new Action<DocumentEntry>(MoveDocument), HasDocument);
            _share = new DelegateCommand<DocumentEntry>(new Action<DocumentEntry>(ShareDocument), HasDocument);
            _delete = new DelegateCommand<DocumentEntry>(new Action<DocumentEntry>(DeleteDocument), HasDocument);
            _rename = new DelegateCommand<DocumentEntry>(new Action<DocumentEntry>(RenameDocument), HasDocument);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saves the id and company code.
        /// </summary>
        /// <param name="id">The id for the entity.</param>
        /// <param name="companyCode">The company code.</param>
        public void SetArguments(string id, string companyCode)
        {
            _rootId = id;
            _companyCode = companyCode.ToLower();
        }

        /// <summary>
        /// Loads the documents from the retrieval service.
        /// </summary>
        /// <returns>The async task to wait on.</returns>
        public async Task Load()
        {
            if (_index != PivotIndex) return;

            var dispatcher = Window.Current.Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
            {
                Loading = true;
                _documents.Clear();
                BuildDocumentGroups();
            });

            Task.Run(async () =>
            {
                return await DocumentRetrievalService.Instance.RetrieveDocumentsAsync(_rootId, _companyCode, _folders);
            }).ContinueWith(async (t) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        if (t.IsCompleted) _documents.AddRange(t.Result);
                        BuildDocumentGroups();
                    }
                    finally
                    {
                        Loading = false;
                    }
                });
            });
        }

        /// <summary>
        /// Sets the current pivot index.
        /// </summary>
        /// <param name="index">The new pivot index being maintained by the page.</param>
        public async void SetPivotIndex(int index)
        {
            try
            {
                var active = ((_index != PivotIndex) && (index == PivotIndex));
                var wasActive = ((_index == PivotIndex) && (index != PivotIndex));
                var closeSearch = (wasActive && (_searchControl != null));

                _index = index;

                if (active)
                {
                    await Load();
                }
                else if (wasActive)
                {
                    await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Loading = false; });
                }

                if (closeSearch) await CloseSearchResults(_searchControl);
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
                Current = (sender as GridView)?.SelectedItem as DocumentEntry;

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
                if (_current == null) return;

                await DoDocumentOpen(_current);
            });
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the current document entry.
        /// </summary>
        public DocumentEntry Current
        {
            get { return _current; }
            set { Set(ref _current, value); }
        }

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
        public DelegateCommand<DocumentEntry> Share
        {
            get { return _share; }
        }

        /// <summary>
        /// Opens the current document.
        /// </summary>
        public DelegateCommand<DocumentEntry> Open
        {
            get { return _open; }
        }

        /// <summary>
        /// Moves the current document.
        /// </summary>
        public DelegateCommand<DocumentEntry> Move
        {
            get { return _move; }
        }

        /// <summary>
        /// Renames the current document.
        /// </summary>
        public DelegateCommand<DocumentEntry> Rename
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
        public DelegateCommand<DocumentEntry> Delete
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
            get { return _loading; }
            set
            {
                if (_loading != value)
                {
                    _loading = value;
                    _owner.Loading = value;

                    RaisePropertyChanged("Loading");
                }
            }
        }

        #endregion
    }
}
