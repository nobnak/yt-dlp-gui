using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace yt_dlp_gui {
    using IoPath = System.IO.Path;
    public partial class App :Application {
        public static string? AppExe;
        public static string? AppPath;
        public static string? AppName;
        private void LoadPath() {
            var entryAssembly = Assembly.GetEntryAssembly();
            AppName = entryAssembly?.GetName().Name ?? "yt-dlp-gui";
            AppPath = AppContext.BaseDirectory.TrimEnd(IoPath.DirectorySeparatorChar, IoPath.AltDirectorySeparatorChar);
            AppExe = IoPath.Combine(AppPath, AppName + ".exe");
        }
        public static void Log(string message) {
            try {
                var logPath = IoPath.Combine(AppPath ?? "", "yt-dlp-gui.log");
                var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}";
                File.AppendAllText(logPath, line, Encoding.UTF8);
            } catch { }
        }
        public static string Path(Folders type, params string[] pathpart) {
            //var exe = Environment.ProcessPath;
            //var folder = IoPath.GetDirectoryName(exe);
            //var name = IoPath.GetFileNameWithoutExtension(exe);
            List<string> parmas = new() { AppPath };
            //新增預設
            parmas.AddRange(type switch {
                Folders.root => Array.Empty<string>(),
                Folders.bin => new[] { "bin" },
                Folders.configs => new[] { "configs" },
                Folders.temp => new[] { "temp" },
                _ => throw new NotImplementedException(),
            });
            //新增延伸
            parmas.AddRange(pathpart);
            var res = "";
            try {
                res = IoPath.Combine(parmas.ToArray());
            } catch (Exception) { }
            return res;
        }
        public enum Folders {
            root, bin, configs, temp
        }
    }
}
