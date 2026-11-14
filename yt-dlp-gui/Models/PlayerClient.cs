using System.ComponentModel;

namespace yt_dlp_gui.Models {
    public class PlayerClientItem : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Name { get; set; } = string.Empty;
        public bool IsChecked { get; set; } = false;
    }
}
