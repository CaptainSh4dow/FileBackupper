using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;


namespace FileBackupper.Models;
public class Profile : ObservableObject
{
    private double duration = 1;
    private int selectedTimeUnitIndex = 1;
    private ObservableCollection<Pair<string, bool>> directories = new();

    public int SelectedTimeUnitIndex { get => selectedTimeUnitIndex; set => SetProperty(ref selectedTimeUnitIndex, value); }
    public bool BackupInLoop { get => backupInLoop; set => SetProperty(ref backupInLoop, value); }
    private bool backupInLoop = false;
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
    public Profile(ObservableCollection<Pair<string, bool>> directories)
    {
        Directories = directories;
    }
}
