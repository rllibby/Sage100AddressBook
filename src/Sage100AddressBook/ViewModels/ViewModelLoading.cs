/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using Template10.Mvvm;

namespace Sage100AddressBook.ViewModels
{
    /// <summary>
    /// View model descendant class that exposes a loading state.
    /// </summary>
    public class ViewModelLoading : ViewModelBase
    {
        #region Private fields

        private int _loading;

        #endregion

        #region Public properties

        /// <summary>
        /// True if loading, otherwise false.
        /// </summary>
        public bool Loading
        {
            get { return (_loading > 0); }
            set
            {
                _loading += (value ? 1 : (-1));
                _loading = Math.Max(0, _loading);

                base.RaisePropertyChanged();
            }
        }

        #endregion
    }
}
