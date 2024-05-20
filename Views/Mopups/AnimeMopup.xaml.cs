using Mopups.Pages;
using TFGMaui.ViewModels;
using TFGMaui.ViewModels.Mopup;

namespace TFGMaui.Views.Mopups;

public partial class AnimeMopup : PopupPage
{
    public AnimeMopup(MovieMopupViewModel movieMopupViewModel)
    {
        InitializeComponent();
        BindingContext = movieMopupViewModel;
    }
}