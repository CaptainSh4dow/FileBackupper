namespace FileBackupper.Models;
public class Profile : ObservableObject
{
    #region Private
    private double duration = 1;
    private int selectedTimeUnitIndex = 1;
    private bool backupInLoop = false;
    private ObservableCollection<Pair<string, bool>> directories = new();
    #endregion
    #region Public
    public int SelectedTimeUnitIndex { get => selectedTimeUnitIndex; set => SetProperty(ref selectedTimeUnitIndex, value); }
    public bool BackupInLoop { get => backupInLoop; set => SetProperty(ref backupInLoop, value); }
    public double Duration
    {
        get => duration;
        set
        {

            if (value <= 0) throw new ArgumentException("Value Most Be Greater Than 0.");

            SetProperty(ref duration, value);
            return;

        }
    }
    public ObservableCollection<Pair<string, bool>> Directories { get => directories; set => SetProperty(ref directories, value); }

    #endregion
    public Profile(ObservableCollection<Pair<string, bool>> directories)
    {
        Directories = directories;
    }
}
