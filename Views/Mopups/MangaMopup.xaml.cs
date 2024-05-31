using Mopups.Pages;
using TFGMaui.ViewModels;
using TFGMaui.ViewModels.Mopup;

namespace TFGMaui.Views.Mopups;

public partial class MangaMopup : PopupPage
{
    public MangaMopup(MangaMopupViewModel mangaMopupViewModel)
    {
        InitializeComponent();
        BindingContext = mangaMopupViewModel;
    }
}