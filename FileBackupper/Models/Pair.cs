namespace FileBackupper.Models;
public class Pair<TKey, TValue> : ObservableObject
{

    #region Private
    private TKey key;
    private TValue value;
    #endregion
    #region Public
    public TKey Key { get => key; set => SetProperty(ref key, value); }

    public TValue Value { get => value; set => SetProperty(ref this.value, value); }
    #endregion
    public Pair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
