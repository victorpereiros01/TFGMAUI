using Mopups.Pages;
using TFGMaui.ViewModels;
using TFGMaui.ViewModels.Mopup;

namespace TFGMaui.Views.Mopups;

public partial class GameMopup : PopupPage
{
    public GameMopup(MovieMopupViewModel movieMopupViewModel)
    {
        InitializeComponent();
        BindingContext = movieMopupViewModel;
    }
}