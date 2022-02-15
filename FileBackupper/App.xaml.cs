namespace FileBackupper;
public partial class App : Application
{
    //Log Exceptions
    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        Logger.LogException(e.Exception);
    }
}
