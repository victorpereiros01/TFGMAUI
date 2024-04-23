using Mopups.Pages;
using TFGMaui.ViewModels;

namespace TFGMaui.Views.Mopups;

public partial class MovieMopup : PopupPage
{
    public MovieMopup(MovieMopupViewModel movieMopupViewModel)
    {
        InitializeComponent();
        BindingContext = movieMopupViewModel;
    }
}