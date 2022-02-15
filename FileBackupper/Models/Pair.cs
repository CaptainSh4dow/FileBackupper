using CommunityToolkit.Mvvm.ComponentModel;
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
