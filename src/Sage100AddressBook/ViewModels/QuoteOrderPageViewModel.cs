/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using Sage100AddressBook.Services.Sage100Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Template10.Mvvm;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model for the quote order page.
    /// </summary>
    public class QuoteOrderPageViewModel : ViewModelLoading
    {
        #region Private fields

        private ObservableCollectionEx<OrderDetail> _lines = new ObservableCollectionEx<OrderDetail>();
        private RadDataGrid _grid;
        private OrderDetail _selected;
        private DelegateCommand _addLine;
        private DelegateCommand _deleteLine;
        private DelegateCommand _editLine;
        private Order _order = new Order();
        private QuoteOrderArgs _args;

        #endregion

        #region Private methods

        /// <summary>
        /// Clear the current selection.
        /// </summary>
        private void ClearSelection()
        {
            _grid = null;
            _selected = null;

            _editLine.RaiseCanExecuteChanged();
            _deleteLine.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Event that is fired when editing a line,.
        /// </summary>
        private async void EditLineAction()
        {
            if (_selected == null) return;

            var selected = _selected;

            await Dispatcher.DispatchAsync(async () =>
            {
                Loading = true;

                try
                {
                    var result = await Dialogs.NumericInput(selected.ItemCodeDesc, Convert.ToInt32(selected.QuantityOrdered));

                    if ((result == selected.QuantityOrdered) || (result < 0)) return;
                    if (result == 0)
                    {
                        await OrderWebService.Instance.DeleteLine(_args.CompanyId, _args.Id, selected.Id);
                    }
                    else
                    {
                        selected.QuantityOrdered = result;
                        await OrderWebService.Instance.UpdateLine(_args.CompanyId, _args.Id, selected.Id, selected);
                    }

                    await LoadOrder();

                    RaisePropertyChanged("Item");
                }
                finally
                {
                    Loading = false;
                }
            });
        }

        /// <summary>
        /// Loads the order and copies the lines to our observable collection.
        /// </summary>
        /// <returns>The async task to wait on.</returns>
        private async Task LoadOrder()
        {
            var order = await OrderWebService.Instance.GetOrderAsync(_args.Id, _args.CompanyId);
            var temp = new List<OrderDetail>();

            Item = order;

            foreach (var line in order.Details)
            {
                var copy = line.Copy();

                copy.Persisted = true;
                copy.Modified = false;

                temp.Add(copy);
            }

            _lines.Set(temp);

            ClearSelection();
        }

        /// <summary>
        /// Performs the delete on the line collection.
        /// </summary>
        private async void DeleteLineAction()
        {
            var line = _selected;

            if (line == null) return;

            await Dispatcher.DispatchAsync(async () =>
            {
                Loading = true;

                try
                {
                    await OrderWebService.Instance.DeleteLine(_args.CompanyId, _args.Id, line.Id);
                    await LoadOrder();
                }
                finally
                {
                    Loading = false;
                }
            });
        }

        /// <summary>
        /// Performs the add on the line collection.
        /// </summary>
        private async void AddLineAction()
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                Loading = true;

                try
                {
                    var line = await Dialogs.GetQuickQuoteItem(_args.CompanyId, _args.CustomerId, "Add Line");

                    if (line == null) return;

                    var detailLine = new OrderDetail()
                    {
                        ItemId = line.Id,
                        QuantityOrdered = line.Quantity,
                        ItemCodeDesc = line.Description,
                        Persisted = false,
                        Modified = true
                    };

                    await OrderWebService.Instance.AddLine(_args.CompanyId, _args.Id, detailLine);
                    await LoadOrder();

                    RaisePropertyChanged("Item");
                }
                finally
                {
                    Loading = false;
                }
            });
        }

        /// <summary>
        /// Determines if a line can be deleted.
        /// </summary>
        /// <returns>True if the line can be deleted, otherwise false.</returns>
        private bool CanEditDeleteLine()
        {
            return ((_grid != null) && (_selected != null));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public QuoteOrderPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) { }

            _addLine = new DelegateCommand(new Action(AddLineAction));
            _deleteLine = new DelegateCommand(new Action(DeleteLineAction), CanEditDeleteLine);
            _editLine = new DelegateCommand(new Action(EditLineAction), CanEditDeleteLine);
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
                _args = (parameter as QuoteOrderArgs);

                try
                {
                    await LoadOrder();
                }
                catch
                {
                    NavigationService.GoBack();
                }
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
            try
            {
                if (_args.Type == OrderType.Order)
                {
                    var orders = GlobalCache.OrderCache.Get(_args.CompanyId, _args.CustomerId);

                    if (orders == null) return;

                    foreach (var item in orders)
                    {
                        if (item.Id.Equals(_order.Id))
                        {
                            item.Assign(_order);
                            GlobalCache.OrderCache.Set(_args.CompanyId, _args.CustomerId, orders);

                            return;
                        }
                    }
                }

                var quotes = GlobalCache.QuoteCache.Get(_args.CompanyId, _args.CustomerId);

                if (quotes == null) return;

                foreach (var item in quotes)
                {
                    if (item.Id.Equals(_order.Id))
                    {
                        item.Assign(_order);
                        GlobalCache.QuoteCache.Set(_args.CompanyId, _args.CustomerId, quotes);

                        return;
                    }
                }
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// Event that is triggered when the selection changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public async void OnSelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            await Dispatcher.DispatchAsync(() =>
            {
                _grid = (sender as RadDataGrid);
                _selected = _grid?.SelectedItem as OrderDetail;

                _editLine.RaiseCanExecuteChanged();
                _deleteLine.RaiseCanExecuteChanged();
            });
        }

        /// <summary>
        /// Event that is triggered on grid double tap.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">THe event arguments.</param>
        public async void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            await Dispatcher.DispatchAsync(() =>
            {
                _grid = (sender as RadDataGrid);

                var pt = e.GetPosition(_grid);
                var row = _grid.HitTestService.RowItemFromPoint(pt);

                if (row == null) return;

                EditLineAction();
            });
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The add line command.
        /// </summary>
        public DelegateCommand AddLine
        {
            get { return _addLine; }
        }

        /// <summary>
        /// The delete line command.
        /// </summary>
        public DelegateCommand DeleteLine
        {
            get { return _deleteLine; }
        }

        /// <summary>
        /// The edit line command.
        /// </summary>
        public DelegateCommand EditLine
        {
            get { return _editLine; }
        }

        /// <summary>
        /// The collection of detail lines.
        /// </summary>
        public ObservableCollectionEx<OrderDetail> Lines
        {
            get { return _lines; }
        }

        /// <summary>
        /// The order item that we are working on.
        /// </summary>
        public Order Item
        {
            get { return _order; }
            set
            {
                Set(ref _order, value);
                RaisePropertyChanged("Title");
            }
        }

        /// <summary>
        /// Returns the title.
        /// </summary>
        public string Title
        {
            get
            {
                return ((_order == null) || (_args == null)) ? string.Empty : string.Format("{0} {1}", (_args.Type == OrderType.Order) ? "Order" : "Quote", _order.SalesOrderNo); 
            }
        }

        #endregion
    }
}
