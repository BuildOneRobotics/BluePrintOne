using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace BluePrintOne
{
    public class Updater
    {
        private const string GITHUB_API = "https://api.github.com/repos/buildonerobotics/blueprint-one/releases/latest";
        private const string CURRENT_VERSION = "1.0.0";

        public static async Task<bool> CheckForUpdates()
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "BluePrintOne");
                var response = await client.GetStringAsync(GITHUB_API);
                var release = JsonSerializer.Deserialize<GitHubRelease>(response);
                
                if (release?.tag_name != null && release.tag_name.TrimStart('v') != CURRENT_VERSION)
                {
                    var result = MessageBox.Show($"New version {release.tag_name} available!\n\nCurrent: {CURRENT_VERSION}\nDownload update?", 
                        "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    
                    if (result == MessageBoxResult.Yes && release.assets?.Length > 0)
                    {
                        await DownloadAndInstall(release.assets[0].browser_download_url);
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private static async Task DownloadAndInstall(string url)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "BluePrintOne.exe");
            using var client = new HttpClient();
            var data = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(tempPath, data);
            Process.Start(new ProcessStartInfo { FileName = tempPath, UseShellExecute = true });
            Application.Current.Shutdown();
        }

        private class GitHubRelease
        {
            public string tag_name { get; set; }
            public Asset[] assets { get; set; }
        }

        private class Asset
        {
            public string browser_download_url { get; set; }
        }
    }
}
