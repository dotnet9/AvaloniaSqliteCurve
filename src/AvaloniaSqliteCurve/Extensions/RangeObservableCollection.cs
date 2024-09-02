using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AvaloniaSqliteCurve.Extensions;

public class RangeObservableCollection<T> : ObservableCollection<T>
{
    private bool SuppressNotificaction { get; set; }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!SuppressNotificaction) base.OnCollectionChanged(e);
    }

    public new void Clear()
    {
        Items.Clear();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public void Add(IEnumerable<T>? items)
    {
        if (items == null)
        {
            return;
        }

        SuppressNotificaction = true;

        foreach (var item in items)
        {
            Items.Add(item);
        }

        SuppressNotificaction = false;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}