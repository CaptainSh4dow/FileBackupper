using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable disable
namespace FileBackupper.Models;
public class Pair<TKey, TValue> : ObservableObject
{
    private TKey key;
    private TValue value;
    public TKey Key { get => key; set => SetProperty(ref key, value); }

    public TValue Value { get => value; set => SetProperty(ref this.value, value); }

    public Pair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
