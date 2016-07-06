/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.Sage100Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#pragma warning disable 4014

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model for the quotes pivot item.
    /// </summary>
    public class QuotePivotViewModel : BindableBase
    {
        #region Private constants

        private const int PivotIndex = 2;

        #endregion

        #region Private fields

        private ObservableCollectionEx<OrderSummary> _quotes = new ObservableCollectionEx<OrderSummary>();
        private ViewModelLoading _owner;
        private DelegateCommand<OrderSummary> _send;
        private DelegateCommand _quickQuote;
        private OrderSummary _current;
        private string _companyCode;
        private string _rootId;
        private int _index = (-1);
        private bool _loading;

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
        /// <returns>True if the quote is not null.</returns>
        private async void SendAction(OrderSummary entry)
        {
            /*
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {




            });
            */

            await Task.CompletedTask;
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
                        if (_quotes.FirstOrDefault(q => q.Id.Equals(quote.Id)) == null) _quotes.Add(quote);
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
            _send = new DelegateCommand<OrderSummary>(new Action<OrderSummary>(SendAction), HasQuote);
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
            if (_index != PivotIndex) return;

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
                        if (t.IsCompleted) _quotes.Set(t.Result);
                        RaisePropertyChanged("IsEmpty");
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

                _index = index;

                if (active)
                {
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
                Current = (sender as GridView)?.SelectedItem as OrderSummary;
            }
            finally
            {
                RaisePropertyChanged("QuoteCommandsVisible");
            }
        }

        #endregion

        #region Public properties

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
