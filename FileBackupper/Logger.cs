
namespace FileBackupper;
public static class Logger
{

    public static void LogException(Exception e)
    {

        var expection = new
        {
            Message = e.Message,
            StackTrace = e.StackTrace,
            Source = e.Source,
            Data = e.Data,
        };
        var ex = JsonSerializer.Serialize(expection);
        var logs = Directory.CreateDirectory("Logs");
        var logFullPath = Path.Combine(logs.FullName, $"{DateTime.Now.ToString("yyyy-MMM-dd HH-mm-ss-ff")}.json");
        using (StreamWriter sw = new(logFullPath, false))
            sw.Write(ex);

    }
}
