using Mopups.Services;
using System.Threading.Tasks;

namespace TuneXtend.Pages;

public partial class UserLoginPopupPage
{
    public string Url { get; set; }
    public string Result { get; set; }

    TaskCompletionSource<string> _taskCompletionSource;
    public Task<string> PopupDismissedTask => _taskCompletionSource.Task;
    public UserLoginPopupPage(string url)
    {
        Url = url;
        BindingContext = this;
        InitializeComponent();
    }

    private void LoginView_OnNavigated(object sender, WebNavigatedEventArgs e)
    {
        if (e.Url.Contains(SpotifyConstants.REDIRECT_URI))
        {
            Result = e.Url;
            MopupService.Instance.PopAsync();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _taskCompletionSource = new TaskCompletionSource<string>();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _taskCompletionSource.SetResult(Result);
    }
}