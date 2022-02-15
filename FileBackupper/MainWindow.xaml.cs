namespace FileBackupper;

public partial class MainWindow : MetroWindow
{

    public MainWindow()
    {
        InitializeComponent();
        //Passing Close To The ICloseWindow Interface.
        if (DataContext is ICloseWindow icw) icw.Close = Close;


    }


    //Used To Validate TextBox TO Use Only Numrical Values.
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

}
