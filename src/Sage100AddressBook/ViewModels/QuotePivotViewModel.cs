/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.Sage100Services;
using Sage100AddressBook.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

#pragma warning disable 4014

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model for the quotes pivot item.
    /// </summary>
    public class QuotePivotViewModel : ViewModelBase
    {
        #region Private constants

        private const int PivotIndex = 2;

        #endregion

        #region Private fields

        private ObservableCollectionEx<OrderSummary> _quotes = new ObservableCollectionEx<OrderSummary>();
        private ViewModelLoading _owner;
        private DelegateCommand<OrderSummary> _send;
        private DelegateCommand<OrderSummary> _delete;
        private DelegateCommand<OrderSummary> _edit;
        private DelegateCommand _quickQuote;
        private DelegateCommand _refresh;
        private OrderSummary _current;
        private string _companyCode;
        private string _rootId;
        private int _currentIndex = (-1);
        private int _index = (-1);
        private bool _loading;
        private bool _loaded;
        private bool _isLoading;

        #endregion

        #region Private methods

        /// <summary>
        /// Determines if we have a current quote.
        /// </summary>
        /// <returns>True if the quote is not null.</returns>
        private bool HasQuote(OrderSummary entry)
        {
            return ((entry != null) || (_current != null));
        }

        /// <summary>
        /// Sends the quote message.
        /// </summary>
        /// <returns>The async action</returns>
        private async void SendAction(OrderSummary entry)
        {
            if (entry == null) return;

            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var customer = (_owner as CustomerDetailPageViewModel)?.CurrentCustomer;
                var scope = new InputScope();
                var name = new InputScopeName();

                name.NameValue = InputScopeNameValue.EmailNameOrAddress;
                scope.Names.Add(name);

                var result = await Dialogs.Input(scope, "Recipient email address.", customer?.EmailAddress, "Email address...");

                if (result == null) return;

                Loading = true;

                try
                {
                    var quoteMessage = new SendQuoteMessage
                    {
                        CustomerId = _rootId,
                        EmailAddress = result,
                        OrderId = entry.Id
                    };

                    await OrderWebService.Instance.SendQuoteMessage(_companyCode, quoteMessage);
                }
                finally
                {
                    Loading = false;
                }
            });
        }

        /// <summary>
        /// Deletes the quote.
        /// </summary>
        /// <returns>The async action.</returns>
        private async void DeleteAction(OrderSummary entry)
        {
            if (entry == null) return;

            await _owner.Dispatcher.DispatchAsync(async () =>
            {
                if (!(await Dialogs.ShowOkCancel(string.Format("Delete the quote \"{0}\"?", entry.SalesOrderNo)))) return;

                Loading = true;

                try
                {
                    var deleted = await OrderWebService.Instance.DeleteQuote(_companyCode, entry.Id);

                    if (deleted) _quotes.Remove(entry);
                }
                finally
                {
                    Loading = false;
                }
            });
        }

        /// <summary>
        /// Performs the refresh on quotes.
        /// </summary>
        private async void RefreshAction()
        {
            if (_isLoading) return;

            _loaded = false;

            await Load();
        }

        /// <summary>
        /// Performs the edit action on quotes.
        /// </summary>
        private void EditAction(OrderSummary entry)
        {
            if (entry == null) return;

            var args = new QuoteOrderArgs
            {
                Type = OrderType.Quote,
                Id = entry.Id,
                CustomerId = _rootId,
                CompanyId = _companyCode
            };

             _owner.NavigationService.Navigate(typeof(QuoteOrderPage), args, new SuppressNavigationTransitionInfo());
        }

        /// <summary>
        /// Performs the quick quote action.
        /// </summary>
        private async void QuickQuoteAction()
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Loading = true;

                try
                {
                    var line = await Dialogs.GetQuickQuoteItem(_companyCode, _rootId);

                    if (line == null) return;

                    var quickQuote = new QuickQuote
                    {
                        CustomerId = _rootId,
                        ItemId = line.Id,
                        Quantity = line.Quantity
                    };

                    var result = await OrderWebService.Instance.PostQuickQuote(_companyCode, quickQuote);

                    if (result == null) return;

                    var quotes = await CustomerWebService.Instance.GetQuotesSummaryAsync(_rootId, _companyCode);

                    foreach (var quote in quotes)
                    {
                        if (_quotes.FirstOrDefault(q => q.Id.Equals(quote.Id)) == null)
                        {
                            quote.Delete = _delete;
                            quote.Send = _send;
                            quote.Edit = _edit;

                            _quotes.Add(quote);

                            CurrentIndex = _quotes.Count - 1;

                            return;
                        }
                    }
                }
                finally
                {
                    Loading = false;
                }
            });
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public QuotePivotViewModel(ViewModelLoading owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
            _refresh = new DelegateCommand(new Action(RefreshAction));
            _send = new DelegateCommand<OrderSummary>(new Action<OrderSummary>(SendAction), HasQuote);
            _edit = new DelegateCommand<OrderSummary>(new Action<OrderSummary>(EditAction), HasQuote);
            _delete = new DelegateCommand<OrderSummary>(new Action<OrderSummary>(DeleteAction), HasQuote);
            _quickQuote = new DelegateCommand(new Action(QuickQuoteAction));
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
            if ((_index != PivotIndex) || _loaded || _isLoading) return;

            _isLoading = true;

            var dispatcher = Window.Current.Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Loading = true;

                _quotes.Clear();
            });

            Task.Run(async () =>
            {
                return await CustomerWebService.Instance.GetQuotesSummaryAsync(_rootId, _companyCode);

            }).ContinueWith(async (t) =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        if (t.IsCompleted)
                        {
                            foreach (var entry in t.Result)
                            {
                                entry.Delete = _delete;
                                entry.Edit = _edit;
                                entry.Send = _send;
                            }

                            _quotes.Set(t.Result);
                            _loaded = true;
                        }
                        RaisePropertyChanged("IsEmpty");
                    }
                    finally
                    {
                        Loading = _isLoading = false;
                    }
                });
            });
        }

        /// <summary>
        /// Event that is triggered when the quote is double tapped.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void QuoteDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (_current == null) return;

            var args = new QuoteOrderArgs
            {
                Type = OrderType.Quote,
                Id = _current.Id,
                CustomerId = _rootId,
                CompanyId = _companyCode
            };

            _owner.NavigationService.Navigate(typeof(QuoteOrderPage), args, new SuppressNavigationTransitionInfo());
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

                _index = index;

                if (active)
                {
                    if (_isLoading)
                    {
                        Loading = true;
                        return;
                    }

                    await Load();
                }
                else if (wasActive)
                {
                    await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Loading = false; });
                }
            }
            finally
            {
                RaisePropertyChanged("QuoteCommandsVisible");
            }
        }

        /// <summary>
        /// Event that is triggered when the selected quote changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void QuoteSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var grid = (sender as GridView);

                Current = grid.SelectedItem as OrderSummary;
                RaisePropertyChanged("QuoteCommandsVisible");

                if ((Current != null) && (grid != null)) grid.ScrollIntoView(Current);
            }
            finally
            {
                _send.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the current quote summary entry.
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { Set(ref _currentIndex, value); }
        }

        /// <summary>
        /// Returns the current quote summary entry.
        /// </summary>
        public OrderSummary Current
        {
            get { return _current; }
            set { Set(ref _current, value); }
        }

        /// <summary>
        /// Collection of document groups.
        /// </summary>
        public ObservableCollectionEx<OrderSummary> Quotes
        {
            get { return _quotes; }
        }

        /// <summary>
        /// The action for the send quote.
        /// </summary>
        public DelegateCommand<OrderSummary> Send
        {
            get { return _send; }
        }

        /// <summary>
        /// The action for the send quote.
        /// </summary>
        public DelegateCommand<OrderSummary> Edit
        {
            get { return _edit; }
        }

        /// <summary>
        /// The action for deleting the quote.
        /// </summary>
        public DelegateCommand<OrderSummary> Delete
        {
            get { return _delete; }
        }

        /// <summary>
        /// The action for refreshing the quote list.
        /// </summary>
        public DelegateCommand Refresh
        {
            get { return _refresh; }
        }

        /// <summary>
        /// The action for the quick quote.
        /// </summary>
        public DelegateCommand QuickQuote
        {
            get { return _quickQuote; }
        }

        /// <summary>
        /// Determines if the quote commands are available.
        /// </summary>
        public bool QuoteCommandsVisible
        {
            get { return (_index == PivotIndex); }
        }

        /// <summary>
        /// Determines if the view is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return (_quotes.Count == 0); }
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
