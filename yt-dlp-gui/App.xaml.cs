using Libs.Yaml;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using yt_dlp_gui.Models;

namespace yt_dlp_gui {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static string CurrentVersion = "2023.03.28";
        public static Lang Lang { get; set; } = new();
        private void Application_Startup(object sender, StartupEventArgs e) {
            var args = e.Args.ToList();
            LoadPath();

            var langPath = ResolveLangPath();
            Lang = Yaml.Open<Lang>(langPath);
            new Views.Main().Show();
        }
        /// <summary>languages\{locale}\yt-dlp-gui.lang を優先し、無ければ exe 横の yt-dlp-gui.lang を返す。</summary>
        static string ResolveLangPath() {
            var root = AppPath ?? "";
            var langFile = AppName + ".lang";
            var culture = CultureInfo.CurrentUICulture;
            foreach (var name in new[] { culture.Name, culture.TwoLetterISOLanguageName }) {
                if (string.IsNullOrEmpty(name)) continue;
                var candidate = System.IO.Path.Combine(root, "languages", name, langFile);
                if (File.Exists(candidate)) return candidate;
            }
            return System.IO.Path.Combine(root, langFile);
        }
    }
}
