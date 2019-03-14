﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ConferenceTracker;
using ConferenceTracker.Droid;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(ExtendedListView), typeof(ExtendedListViewRenderer))]

namespace ConferenceTracker.Droid
{
    public class ExtendedListViewRenderer : ListViewRenderer
    {
        private bool _attached;
        private int _prevFirstVisibleElement;
        private ExtendedListView _myListView;

        public ExtendedListViewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == _myListView)
            {
                _myListView = null;
            }
            if (e.NewElement is ExtendedListView)
            {
                _myListView = Element as ExtendedListView;

                _myListView.LastScrollDirection = ExtendedListView.NoPreviousScroll;
                _myListView.AtStartOfList = true;

                _attached = true;
                Control.Scroll += Control_Scroll;
            }
        }

        // Handle initial setting of AtEndOfList, as well as updating for orientation changes.
        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);

            if (changed && _myListView != null && _myListView.ItemsSource != null)
            {
                // The Android renderer does not seem to expose the fully-enumerated list to us in this interface,
                // or anything related to total size.  If the ItemsSource on the Forms side of things happens to be
                // an ICollection, we can use its Count property. However it doesn't have to be an ICollection
                // (though typically it will be, as both List<T> and ObservableCollection<T> are), and we don't want
                // to force a full enumeration of the collection. So we just iterate through from the beginning, just
                // far enough to prove whether the LastVisibleElement is the last in the collection.
                if (_myListView.ItemsSource is System.Collections.ICollection)
                {
                    var collectionSize = ((System.Collections.ICollection)_myListView.ItemsSource).Count;
                    _myListView.AtEndOfList = (Control.LastVisiblePosition >= collectionSize);
                }
                else
                {
                    var counter = 0;
                    var atEnd = true;
                    foreach (var item in _myListView.ItemsSource)
                    {
                        if (++counter > Control.LastVisiblePosition)
                        {
                            atEnd = false;
                            break;
                        }
                    }
                    _myListView.AtEndOfList = atEnd;
                }
            }
        }

        private void Control_Scroll(object sender, AbsListView.ScrollEventArgs e)
        {
            if (e.FirstVisibleItem != _prevFirstVisibleElement && _myListView != null)
            {
                var direction = e.FirstVisibleItem > _prevFirstVisibleElement ? ExtendedListView.ScrollToEnd : ExtendedListView.ScrollToStart;
                _myListView.LastScrollDirection = direction;
                _myListView.AtStartOfList = (e.FirstVisibleItem == 0);
                _myListView.AtEndOfList = (e.FirstVisibleItem + 1 + e.VisibleItemCount >= e.TotalItemCount);
                _prevFirstVisibleElement = e.FirstVisibleItem;
            }
            var items = _myListView.ItemsSource as IEnumerable<object>;
            if (items != null && items.Count() > e.FirstVisibleItem)
            {
                _myListView.FirstVisibleItem = items.ElementAt(e.FirstVisibleItem);
            }
            _myListView.FirstVisibleItemIndex = e.FirstVisibleItem;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && _attached)
            {
                _attached = false;
                Control.Scroll -= Control_Scroll;
            }
        }
    }
}