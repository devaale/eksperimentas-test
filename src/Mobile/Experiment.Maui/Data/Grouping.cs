using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Experiment.Maui.Data{
    /// <summary>
    /// Taken from https://devdreamz.com/question/846811-xamarin-listview-grouping
    /// 
    /// Groups sort of working with additional nested collections where everything is grouped in them, what is ridiculous, IMHO
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class Grouping<K, T> : ObservableCollection<T>
    {
        // NB: This is the GroupDisplayBinding above for displaying the header
        public K GroupKey { get; private set; }

        public Grouping(K key, IEnumerable<T> items)
        {
            GroupKey = key;
            foreach (var item in items)
                this.Items.Add(item);
        }
}
}
