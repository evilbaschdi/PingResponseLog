using System;
using System.Windows.Forms;

namespace PingResponseLog.Core
{
    /// <summary>
    /// </summary>
    public class ApplicationBasics : IApplicationBasics
    {
        private readonly IApplicationSettings _applicationSettings;

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
        /// </summary>
        public ApplicationBasics(IApplicationSettings applicationSettings)
        {
            if(applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            _applicationSettings = applicationSettings;
        }

        /// <summary>
        /// </summary>
        public void BrowseLoggingFolder()
        {
            var folderDialog = new FolderBrowserDialog
            {
                SelectedPath = _applicationSettings.LoggingPath
            };

            var result = folderDialog.ShowDialog();
            if(result.ToString() != "OK")
            {
                return;
            }
            _applicationSettings.LoggingPath = folderDialog.SelectedPath;
        }

        /// <summary>
        /// </summary>
        public string GetLoggingPath()
        {
            return string.IsNullOrWhiteSpace(_applicationSettings.LoggingPath)
                ? ""
                : _applicationSettings.LoggingPath;
        }
    }
}