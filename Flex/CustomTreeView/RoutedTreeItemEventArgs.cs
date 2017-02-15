using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Flex.CustomTreeView
{
    /// <summary>
    /// Event handler signature for the
    /// <see cref="TreeViewBase{T}.SelectedItemChangedEvent"/>
    /// routed event.
    /// </summary>
    /// <typeparam name="T">The type of the tree's items.</typeparam>
    /// <param name="sender">The event source.</param>
    /// <param name="e">Provides both new and old processed item.</param>
    public delegate void RoutedTreeItemEventHandler<T>(object sender, RoutedTreeItemEventArgs<T> e) where T : INotifyPropertyChanged;

    /// <summary>
    /// Event arguments for the <see cref="TreeViewBase{T}.SelectedItemChangedEvent"/>
    /// routed event.
    /// </summary>
    /// <typeparam name="T">The type of the tree's items.</typeparam>
    public class RoutedTreeItemEventArgs<T> : RoutedEventArgs where T : INotifyPropertyChanged
    {
        private readonly T newItem;
        private readonly T oldItem;

        /// <summary>
        /// The currently selected item that caused the event. If
        /// the tree's <see cref="TreeViewBase{T}.SelectedItem"/>
        /// property is null, so is this parameter.
        /// </summary>
        public T NewItem
        {
            get { return newItem; }
        }


        /// <summary>
        /// The previously selected item, if any. Might be null
        /// if no item was selected before.
        /// </summary>
        public T OldItem
        {
            get { return oldItem; }
        }


        /// <summary>
        /// Creates the event args.
        /// </summary>
        /// <param name="newItem">The selected item, if any.</param>
        /// <param name="oldItem">The previously selected item, if any.</param>
        public RoutedTreeItemEventArgs(T newItem, T oldItem)
          : base(TreeViewBase<T>.SelectedItemChangedEvent)
        {
            this.newItem = newItem;
            this.oldItem = oldItem;
        }

    }
}
