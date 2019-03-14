using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ConferenceTracker
{
    public class ExtendedListView : ListView
    {
        public const string NoPreviousScroll = "None";
        public const string ScrollToStart = "Towards start";
        public const string ScrollToEnd = "Towards end";

        public static readonly BindableProperty LastScrollDirectionProperty =
            BindableProperty.Create(nameof(LastScrollDirection), typeof(string), typeof(ExtendedListView), null);
        public static readonly BindableProperty AtStartOfListProperty =
            BindableProperty.Create(nameof(AtStartOfList), typeof(bool), typeof(ExtendedListView), false);
        public static readonly BindableProperty AtEndOfListProperty =
            BindableProperty.Create(nameof(AtEndOfList), typeof(bool), typeof(ExtendedListView), false);

        public static readonly BindableProperty FirstVisibleItemIndexProperty =
                BindableProperty.Create(nameof(FirstVisibleItemIndex), typeof(int), typeof(ExtendedListView), 0);

        public static readonly BindableProperty FirstVisibleItemProperty =
                BindableProperty.Create(nameof(FirstVisibleItem), typeof(object), typeof(ExtendedListView), null);

        // Keep track of previous state and when the next time the reported value might change.
        // _nextShowChange is stored to allow a delay before a change in the value is allowed to avoid a jittery look.
        private bool? _lastShowValue;
        private DateTime _nextShowChange;
        public bool ShouldShowSearchBar
        {
            get
            {
                var newValue = AtStartOfList || LastScrollDirection == ScrollToStart;
                var shouldChange = (!_lastShowValue.HasValue) || (_lastShowValue.Value != newValue && _nextShowChange < DateTime.Now);

                if (shouldChange)
                {
                    _lastShowValue = newValue;
                    _nextShowChange = DateTime.Now.AddMilliseconds(250);
                }

                return shouldChange ? newValue : _lastShowValue.Value;
            }
        }

        public string LastScrollDirection
        {
            get { return (string)GetValue(LastScrollDirectionProperty); }
            set
            {
                SetValue(LastScrollDirectionProperty, value);
                OnPropertyChanged(nameof(ShouldShowSearchBar));
            }
        }

        public bool AtStartOfList
        {
            get { return (bool)GetValue(AtStartOfListProperty); }
            set
            {
                SetValue(AtStartOfListProperty, value);
                OnPropertyChanged(nameof(ShouldShowSearchBar));
            }
        }

        public bool AtEndOfList
        {
            get { return (bool)GetValue(AtEndOfListProperty); }
            set
            {
                SetValue(AtEndOfListProperty, value);
            }
        }

        public int FirstVisibleItemIndex
        {
            get { return (int)GetValue(FirstVisibleItemIndexProperty); }
            set
            {
                SetValue(FirstVisibleItemIndexProperty, value);
            }
        }

        public object FirstVisibleItem
        {
            get { return (object)GetValue(FirstVisibleItemProperty); }
            set
            {
                SetValue(FirstVisibleItemProperty, value);
            }
        }
    }
}
