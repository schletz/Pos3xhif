using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ObservablesDemo.Model
{
    /// <summary>
    /// Klasse für die Synchronisation der ObservableCollection mit einer darunterliegenden
    /// Basiscollection. Das Hinzufügen und Löschen von Elementen wird dann mit Add oder Remove
    /// weitergegeben.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SynchronizedObservable<T> : ObservableCollection<T>
    {
        private readonly ICollection<T> sourceCollection;
        /// <summary>
        /// Lädt die Collection, in der die hinzugefügten Werte zurückgeschrieben werden sollen.
        /// </summary>
        /// <param name="sourceCollection"></param>
        public SynchronizedObservable(ICollection<T> sourceCollection) : base(sourceCollection)
        {
            this.sourceCollection = sourceCollection;
            CollectionChanged += SynchronizedObservable_CollectionChanged;
        }

        /// <summary>
        /// Bei einer Änderung der ObservableCollection wird die Änderung in der sourceCollection
        /// nachgezogen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SynchronizedObservable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // NewItems ist eine nicht generische IList und kann null sein. Deswegen wird sie
            // mit Cast in einen typisierten Enumerator geändert. Ist sie null, wird foreach
            // durch den leeren Enumerator als Standardwert nicht durchlaufen.
            // if (e.NewItems != null) geht natürlich auch.
            foreach (T p in e.NewItems?.Cast<T>() ?? Enumerable.Empty<T>())
            {
                sourceCollection.Add(p);
            }
            foreach (T p in e.OldItems?.Cast<T>() ?? Enumerable.Empty<T>())
            {
                sourceCollection.Remove(p);
            }
        }
    }
}
