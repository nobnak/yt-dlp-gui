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
        private static string? _appPath;
        public static string AppPath {
            get {
                if (string.IsNullOrEmpty(_appPath)) {
                    var procPath = Environment.ProcessPath;
                    if (!string.IsNullOrEmpty(procPath)) {
                        _appPath = IoPath.GetDirectoryName(procPath);
                    }
                    if (string.IsNullOrEmpty(_appPath)) {
                        _appPath = AppContext.BaseDirectory.TrimEnd(IoPath.DirectorySeparatorChar, IoPath.AltDirectorySeparatorChar);
                    }
                }
                return _appPath ?? string.Empty;
            }
            set => _appPath = value;
        }

        private static string? _appName;
        public static string AppName {
            get {
                if (string.IsNullOrEmpty(_appName)) {
                    var entry = Assembly.GetEntryAssembly();
                    _appName = entry?.GetName().Name ?? "yt-dlp-gui";
                }
                return _appName;
            }
            set => _appName = value;
        }

        private static string? _appExe;
        public static string AppExe {
            get {
                if (string.IsNullOrEmpty(_appExe)) {
                    _appExe = IoPath.Combine(AppPath, AppName + ".exe");
                }
                return _appExe;
            }
            set => _appExe = value;
        }

        private void LoadPath() {
            _ = AppPath;
            _ = AppName;
            _ = AppExe;
        }
        public static void Log(string message) {
            try {
                var logPath = IoPath.Combine(AppPath, "yt-dlp-gui.log");
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
