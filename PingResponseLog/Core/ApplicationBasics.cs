using System.Windows.Forms;

namespace PingResponseLog.Core
{
    public class ApplicationBasics : IApplicationBasics
    {
        public void BrowseLoggingFolder()
        {
            var folderDialog = new FolderBrowserDialog
            {
                SelectedPath = GetLoggingPath()
            };

            var result = folderDialog.ShowDialog();
            if(result.ToString() != "OK")
            {
                return;
            }

            Properties.Settings.Default.LoggingPath = folderDialog.SelectedPath;
            Properties.Settings.Default.Save();
        }

        public string GetLoggingPath()
        {
            return string.IsNullOrWhiteSpace(Properties.Settings.Default.LoggingPath)
                ? ""
                : Properties.Settings.Default.LoggingPath;
        }
    }
}