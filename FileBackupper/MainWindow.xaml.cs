using FileBackupper.ViewModels;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace FileBackupper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {
            InitializeComponent();

            if (DataContext is ICloseWindow icw) icw.Close = Close;


        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
