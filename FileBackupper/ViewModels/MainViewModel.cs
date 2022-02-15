namespace FileBackupper.ViewModels;
public partial class MainViewModel : ViewModelBase, ICloseWindow
{
    #region Private
    private Profile mainProfile;
    private CancellationTokenSource cancellationToken;
    private bool saveOnClose = true;

    private double progress;
    #endregion
    #region Public 
    public double Progress { get => progress; set => SetProperty(ref progress, value); }
    public Profile MainProfile { get => mainProfile; set => SetProperty(ref mainProfile, value); }
    public ObservableCollection<Pair<string, TimeSpan>> TimeUnits { get; } = new()
    {
        new Pair<string, TimeSpan>("Second", TimeSpan.FromSeconds(1)),
        new Pair<string, TimeSpan>("Minute", TimeSpan.FromMinutes(1)),
        new Pair<string, TimeSpan>("Hour", TimeSpan.FromHours(1)),
        new Pair<string, TimeSpan>("Day", TimeSpan.FromDays(1)),

    };
    #endregion
    #region Commands
    public IRelayCommand ClosedCommand { get; }
    public IRelayCommand AddDirectoryCommand { get; }

    public IRelayCommand SelectAllCommand { get; }
    public IRelayCommand DeselectAllCommand { get; }
    public IRelayCommand InvertSelectionCommand { get; }

    public IRelayCommand DeleteSelectedCommand { get; }
    public IRelayCommand CancelBackupingCommand { get; }

    public IAsyncRelayCommand LoaddedCommand { get; }
    public IAsyncRelayCommand StartBackupingCommand { get; }
    #endregion
    #region ICloseWindow
    public Action Close { get; set; }

    #endregion

    public MainViewModel()
    {
        #region Init Commands 
        AddDirectoryCommand = new RelayCommand(BrowseDirs, () => IsNotBusy);
        SelectAllCommand = new RelayCommand(SelectAll, () => IsNotBusy);
        DeselectAllCommand = new RelayCommand(DeselectAll, () => IsNotBusy);
        DeleteSelectedCommand = new RelayCommand(DeleteSelected, () => IsNotBusy);
        InvertSelectionCommand = new RelayCommand(InvertSelection, () => IsNotBusy);
        StartBackupingCommand = new AsyncRelayCommand(StartBackuping, () => IsNotBusy);
        CancelBackupingCommand = new RelayCommand(StopBackuping, () => IsBusy);

        LoaddedCommand = new AsyncRelayCommand(Loadded);
        ClosedCommand = new RelayCommand(Closed);


        #endregion


        //Init The Profile
        MainProfile = new(new());
        //Notify Commands That's Attached to IsBusy
        IsBusyChanged += (s, e) =>
        {
            AddDirectoryCommand.NotifyCanExecuteChanged();
            SelectAllCommand.NotifyCanExecuteChanged();
            DeselectAllCommand.NotifyCanExecuteChanged();
            DeleteSelectedCommand.NotifyCanExecuteChanged();
            InvertSelectionCommand.NotifyCanExecuteChanged();
            StartBackupingCommand.NotifyCanExecuteChanged();
            CancelBackupingCommand.NotifyCanExecuteChanged();


        };

    }


    #region Selection
    private void SelectAll()
    {
        for (int i = 0; i < MainProfile.Directories.Count; i++)
            MainProfile.Directories[i].Value = true;
    }
    private void DeselectAll()
    {
        for (int i = 0; i < MainProfile.Directories.Count; i++)
            MainProfile.Directories[i].Value = false;
    }
    private void InvertSelection()
    {
        for (int i = 0; i < MainProfile.Directories.Count; i++)
            MainProfile.Directories[i].Value = !MainProfile.Directories[i].Value;
    }
    private void DeleteSelected()
    {
        int count = MainProfile.Directories.Count(x => x.Value == true);
        if (count == 0) return;
        var res = MessageBox.Show($"Are You Sure You Want To Delete {count} Path?",
            "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (res == MessageBoxResult.No) return;
        MainProfile.Directories = new ObservableCollection<Pair<string, bool>>(MainProfile.Directories.Where(x => x.Value == false));
    }
    #endregion
    #region Backuping
    private async Task StartBackuping()
    {

        if (MainProfile.Directories.Count(x => x.Value == true) == 0)
        {
            MessageBox.Show("There's Nothing to Backup! Please Add And Select Paths To Backup", "Empty.");
            return;
        }

        cancellationToken = new();
        IsBusy = true;
        TimeSpan onePercent = MainProfile.Duration * TimeUnits[MainProfile.SelectedTimeUnitIndex].Value * 0.01;
        do
        {

            try
            {
                for (Progress = 0; Progress <= 100; Progress++)
                {
                    await Task.Delay(onePercent, cancellationToken.Token);
                    if (cancellationToken.IsCancellationRequested) break;
                }
                await Backup();

            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                break;
            }


        } while (MainProfile.BackupInLoop);
        Progress = 0;

        IsBusy = false;
        cancellationToken.Dispose();
    }
    private void StopBackuping()
    {
        cancellationToken?.Cancel();
    }
    private async Task Backup()
    {
        var selectedDirs =
            from selectedDir in MainProfile.Directories
            where selectedDir.Value == true
            select selectedDir.Key;

        var files = from file in selectedDirs select new FileInfo(file);
        var tasks = from file in files select BackupFile(file);


        await Task.WhenAll(tasks);


        async Task BackupFile(FileInfo file)
        {

            try
            {
                Directory.CreateDirectory("Backups");
                var backupDir = Directory.CreateDirectory(Path.Combine("Backups", file.Name));
                var zipFileName = Path.Combine(backupDir.FullName, $"{DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss-ff")}.zip");
                await Task.Run(() => ZipFile.CreateFromDirectory(file.FullName, zipFileName)).AsAsyncAction();

            }
            catch (Exception e)
            {
                Logger.LogException(e);
                MessageBox.Show($"UnHandelded Exception.\nException Message:{e.Message}", "Exception Occured");
            }

        }

    }
    private void BrowseDirs()
    {
        var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
        dialog.Multiselect = false;
        dialog.ShowDialog();
        switch (dialog.SelectedPath)
        {
            case var i when string.IsNullOrEmpty(i):
                break;

            case var i when MainProfile.Directories.Any(x => x.Key == i):
                MessageBox.Show("The Path You Selected Already has Been Added, Please Try Unqiue Path.", "Path Already Added", MessageBoxButton.OK, MessageBoxImage.Hand);
                break;

            default:
                MainProfile.Directories.Add(new Pair<string, bool>(dialog.SelectedPath, false));
                break;
        }

    }
    #endregion
    #region Load & Close Handlers
    private void Closed()
    {
        if (saveOnClose)
        {
            string i = JsonSerializer.Serialize(mainProfile);
            using (StreamWriter sw = new("Profile.json", false))
                sw.Write(i);
        }

    }

    private async Task Loadded()
    {
        await GenerateData();
    }

    private async Task GenerateData()
    {
        if (File.Exists("Profile.json"))
        {

            try
            {
                using (FileStream stream = new("Profile.json", FileMode.Open, FileAccess.Read))
                    MainProfile = await JsonSerializer.DeserializeAsync<Profile>(stream) ?? new(new()); ;

            }
            catch (Exception)
            {

                var res = MessageBox.Show("It Appear That Your Profile Json File has Been Corrupted. Press Yes To Create New Profile Or Press No To Open Your Profile Json File and menually Fix it", "Error Loading Profile", MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
                switch (res)
                {


                    case MessageBoxResult.Cancel:
                        saveOnClose = false;
                        Close();
                        break;
                    case MessageBoxResult.Yes:
                        File.Delete("Profile.Json");
                        break;
                    case MessageBoxResult.No:
                        Process.Start("explorer.exe", "Profile.Json");
                        saveOnClose = false;
                        Close();
                        break;
                    default:
                        break;
                }

            }

        }
    }
    #endregion





    
}
