/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Models;
using Sage100AddressBook.Views;
using System;
using System.Collections.ObjectModel;
using Template10.Mvvm;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// Base class that can be used to share common logic between the Quote and Order pivot view models.
    /// </summary>
    public class OrderSummaryViewModel : ViewModelBase
    {
        #region Private fields

        private DelegateCommand<OrderSummary> _edit;
        private ViewModelLoading _owner;
        private OrderSummary _current;
        private OrderType _type;
        private string _companyCode;
        private string _rootId;
        private int _currentIndex = (-1);
        private int _index = (-1);

        #endregion

        #region Private methods

        /// <summary>
        /// Navigates to the quote/order page using the specified entry.
        /// </summary>
        /// <param name="entry">The order summary to navigate to.</param>
        private void NavigateEntry(OrderSummary entry)
        {
            if (entry == null) return;

            var args = new QuoteOrderArgs
            {
                Type = _type,
                Id = entry.Id,
                CustomerId = _rootId,
                CompanyId = _companyCode,
            };

            _owner.SessionState.Add(string.Format("{0}Args", _type), JsonConvert.SerializeObject(args));
            _owner.NavigationService.Navigate(typeof(QuoteOrderPage), args, new SuppressNavigationTransitionInfo());
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Performs the edit action on quotes.
        /// </summary>
        protected void EditAction(OrderSummary entry)
        {
            NavigateEntry(entry);
        }

        /// <summary>
        /// Determines if we have an active order summary.
        /// </summary>
        /// <returns>True if the order summary is not null.</returns>
        protected bool HasOrderSummary(OrderSummary entry)
        {
            return ((entry != null) || (_current != null));
        }

        /// <summary>
        /// Handles back state.
        /// </summary>
        protected void UpdateState(Collection<OrderSummary> collection)
        {
            _currentIndex = (-1);

            var key = string.Format("{0}Args", _type);

            if (!_owner.SessionState.ContainsKey(key)) return;

            var state = _owner.SessionState.Get<string>(key);

            _owner.SessionState.Remove(key);

            var args = JsonConvert.DeserializeObject<QuoteOrderArgs>(state);

            if (!_companyCode.Equals(args.CompanyId, StringComparison.OrdinalIgnoreCase)) return;
            if (!_rootId.Equals(args.CustomerId, StringComparison.OrdinalIgnoreCase)) return;

            for (var i = 0; i < collection.Count; i++)
            {
                if (collection[i].Id.Equals(args.Id, StringComparison.OrdinalIgnoreCase))
                {
                    _currentIndex = i;
                    return;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public OrderSummaryViewModel(ViewModelLoading owner, OrderType type)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
            _type = type;
            _edit = new DelegateCommand<OrderSummary>(new Action<OrderSummary>(EditAction), HasOrderSummary);
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
        /// Event that is triggered when the quote is double tapped.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            NavigateEntry(_current);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the current order summary entry.
        /// </summary>
        public OrderSummary Current
        {
            get { return _current; }
            set { Set(ref _current, value); }
        }

        /// <summary>
        /// The delegate handler for the edit action.
        /// </summary>
        public DelegateCommand<OrderSummary> Edit
        {
            get { return _edit; }
        }

        /// <summary>
        /// The company id.
        /// </summary>
        public string CompanyCode
        {
            get { return _companyCode; }
        }

        /// <summary>
        /// The customer id.
        /// </summary>
        public string RootId
        {
            get { return _rootId; }
        }

        /// <summary>
        /// Returns the owning view model.
        /// </summary>
        public ViewModelLoading Owner
        {
            get { return _owner; }
        }

        /// <summary>
        /// Returns the current quote summary entry.
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { Set(ref _currentIndex, value); }
        }

        #endregion
    }
}
