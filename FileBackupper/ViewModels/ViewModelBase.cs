
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileBackupper.ViewModels;
public class ViewModelBase : ObservableObject
{
    private bool _isBusy = false;
    public event EventHandler? IsBusyChanged;
    public bool IsBusy
    {
        get => _isBusy; set
        {
            SetProperty(ref _isBusy, value);
            OnPropertyChanged(nameof(IsNotBusy));
            IsBusyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public bool IsNotBusy { get => !_isBusy; }
}
