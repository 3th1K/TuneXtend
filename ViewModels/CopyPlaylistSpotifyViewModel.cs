using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TuneXtend.Models.Spotify;

namespace TuneXtend.ViewModels
{
    public class LogObject
    {
        public string LogString { get; set; }
        public Color LogColor { get; set; }
    }

    public partial class CopyPlaylistSpotifyViewModel : ObservableObject
    {
        [ObservableProperty] private string sourcePlaylistId = null;

        private string sourceUri;

        public string SourceUri
        {
            get => sourceUri;
            set
            {
                SetProperty(ref sourceUri, value);
                try
                {
                    var a = value.Split("/")[4].Split("?")[0];
                    SetProperty(ref sourcePlaylistId, a.Length>1?a:null);
                }
                catch
                {
                    SetProperty(ref sourcePlaylistId, null);
                }
            }
        }

        [ObservableProperty] private ObservableCollection<PlaylistItem> playlists = new();
        [ObservableProperty] private ObservableCollection<LogObject> logs = new();


        

        [ObservableProperty] private int? totalLogCount = null;
        private int? completedLogCount = null;

        public int? CompletedLogCount
        {
            get => completedLogCount;
            set
            {
                SetProperty(ref completedLogCount, value);
                if (value is not null && value < TotalLogCount)
                {
                    BtnMainText = $"Copy in progress : {value} / {TotalLogCount}";
                }
                else if (value is null)
                {
                    BtnMainText = $"Copy Done";
                }
            }
        }
        private PlaylistItem selectedPlaylist = null;
        public PlaylistItem SelectedPlaylist
        {
            get => selectedPlaylist;
            set
            {
                SetProperty(ref selectedPlaylist, value);
                if (value is not null)
                    BtnMainText = "Copy To This Playlist";
                else
                    BtnMainText = "+ Create A New Playlist";
            }
        }
        [ObservableProperty] private string btnMainText = "+ Create A New Playlist";
    }
}
