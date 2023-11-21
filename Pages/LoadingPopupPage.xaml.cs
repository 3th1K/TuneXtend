namespace TuneXtend.Pages;

public partial class LoadingPopupPage
{
    public string LoadingMessage { get; set; }

    public LoadingPopupPage(string loadingMessage)
    {
        InitializeComponent();
        LoadingMessage = loadingMessage;
        BindingContext = this;
    }
    public LoadingPopupPage()
    {
        InitializeComponent();
        LoadingMessage = "Loading";
        BindingContext = this;
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}