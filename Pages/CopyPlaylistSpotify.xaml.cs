
using System.Collections.ObjectModel;
using System.Text.Json;
using Mopups.Interfaces;
using TuneXtend.Interfaces;
using TuneXtend.Models.Spotify;
using TuneXtend.ViewModels;

namespace TuneXtend.Pages;

public partial class CopyPlaylistSpotify : ContentPage
{
    private readonly ISpotifyRestService _spotifyRestService;
    private readonly IPopupNavigation _popupNavigation;
    public CopyPlaylistSpotifyViewModel viewModel;
	public CopyPlaylistSpotify(ISpotifyRestService spotifyRestService, IPopupNavigation popupNavigation)
    {
        _spotifyRestService = spotifyRestService;
        _popupNavigation = popupNavigation;
        viewModel = new CopyPlaylistSpotifyViewModel();
        BindingContext = viewModel;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        PageRefreshView_OnRefreshing(new object(), EventArgs.Empty);
    }


    private void CollectionPlaylist_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PlaylistItem previousItem = e.PreviousSelection.Count > 0 ? (PlaylistItem)e.PreviousSelection[0] : null;
        var currentIndex = viewModel.Playlists.IndexOf(viewModel.SelectedPlaylist);
        var previousIndex = viewModel.Playlists.IndexOf(previousItem);
        if (currentIndex > -1)
        {
            var currentItem = viewModel.SelectedPlaylist;
            currentItem.collaborative = true;
            viewModel.Playlists[currentIndex] = currentItem;
        }
        if (previousIndex > -1)
        {
            previousItem!.collaborative = false;
            viewModel.Playlists[previousIndex] = previousItem;
        }
    }

    private void XButton_OnClicked(object sender, EventArgs e)
    {
        viewModel.SelectedPlaylist = null;
    }

    private async Task CopyTracks(string sourcePlaylistId, string destinationPlaylistId)
    {
        await _popupNavigation.PushAsync(new LoadingPopupPage());
        var sourceTracks = await _spotifyRestService.GetPlaylistTracksAsync(sourcePlaylistId);
        var destinationTracks = await _spotifyRestService.GetPlaylistTracksAsync(destinationPlaylistId);
        await _popupNavigation.PopAsync();
        viewModel.TotalLogCount = sourceTracks.Count;
        viewModel.CompletedLogCount = 0;
        viewModel.Logs.Add(new LogObject()
        {
            LogString = "Found " + sourceTracks.Count + " Tracks in the source Playlist",
            LogColor = Colors.LightSeaGreen
        });
        int success = 0;
        int failure = 0;
        int skipped = 0;
        foreach (var track in sourceTracks)
        {
            if (destinationTracks.FirstOrDefault(t => t.id == track.id) is not null)
            {
                viewModel.Logs.Add( new LogObject()
                    {
                     LogString = $"Skipping track [ {track.name} ] => Result : Skipped",
                     LogColor = Colors.DarkGrey
                    });
                skipped++;
                viewModel.CompletedLogCount++;
            }
            else
            {
                var l = new LogObject();
                l.LogString = $"Copying track [ {track.name} ] => Result : ";
                try
                {
                    var result = await _spotifyRestService.AddTrackToPlaylist(destinationPlaylistId, track.id);
                    if (result is not null)
                    {
                        l.LogString+=("Success");
                        l.LogColor = Colors.Green;
                        success++;
                        viewModel.CompletedLogCount++;
                    }
                    else
                    {
                        l.LogString +=("Failure");
                        l.LogColor = Colors.Red;
                        failure++;
                        viewModel.CompletedLogCount++;
                    }
                }
                catch (Exception ex)
                {
                    l.LogString +=("Failure");
                    l.LogColor = Colors.Red;
                    failure++;
                    viewModel.CompletedLogCount++;
                }

                viewModel.Logs.Add(l);
            }

        }
        viewModel.Logs.Add(new LogObject()
        {
            LogString = $"Copied {success} tracks, Failed : {failure}, Skipped : {skipped}",
            LogColor = Colors.LightSeaGreen
        });
        viewModel.TotalLogCount = null;
        viewModel.CompletedLogCount = null;
    }

    private async void BtnMain_OnClicked(object sender, EventArgs e)
    {
        viewModel.Logs = new ObservableCollection<LogObject>();
        if (viewModel.SourcePlaylistId is null)
        {
            await DisplayAlert("Warning", "Please Provide a correct Playlist Url", "Ok");
            return;
        }

        BtnMain.IsEnabled = false;
        if (viewModel.SelectedPlaylist is not null)
        {
            string sourcePlaylistId = viewModel.SourcePlaylistId;
            string destinationPlaylistId = viewModel.SelectedPlaylist.id;
            await CopyTracks(sourcePlaylistId, destinationPlaylistId);
        }
        else
        {
            string playlistName = await DisplayPromptAsync("Create Playlist",
                "Input new playlist name",
                "Create",
                "Cancel",
                new Guid().ToString(),
                10);
            if (playlistName is not null)
            {
                var newPlaylist = await _spotifyRestService.CreatePlaylistAsync(playlistName);
                await DisplayAlert("new playlist created", $"{JsonSerializer.Serialize(newPlaylist)}", "ok");
                string sourcePlaylistId = viewModel.SourcePlaylistId;
                string destinationPlaylistId = newPlaylist.id;
                await CopyTracks(sourcePlaylistId, destinationPlaylistId);
            }
        }
        BtnMain.IsEnabled  = true;
    }

    private async Task Refresh()
    {
        
        viewModel.Logs = new ObservableCollection<LogObject>();
        var playlists = await _spotifyRestService.GetUserPlaylistsAsync();
        if (playlists.Count != viewModel.Playlists.Count)
        {
            foreach (var item in playlists)
            {
                item.collaborative = false;
            }
            viewModel.Playlists = new ObservableCollection<PlaylistItem>(playlists);
        }

        
    }
    private async void PageRefreshView_OnRefreshing(object sender, EventArgs e)
    {
        PageRefreshView.IsRefreshing = true;
        await Refresh();
        PageRefreshView.IsRefreshing = false;
    }
}