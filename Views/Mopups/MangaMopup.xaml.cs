using Mopups.Pages;
using TFGMaui.ViewModels;
using TFGMaui.ViewModels.Mopup;

namespace TFGMaui.Views.Mopups;

public partial class MangaMopup : PopupPage
{
    public MangaMopup(MovieMopupViewModel movieMopupViewModel)
    {
        InitializeComponent();
        BindingContext = movieMopupViewModel;
    }
}