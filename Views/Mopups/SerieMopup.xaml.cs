using Mopups.Pages;
using TFGMaui.ViewModels.Mopup;

namespace TFGMaui.Views.Mopups;

public partial class SerieMopup : PopupPage
{
    public SerieMopup(SerieMopupViewModel serieMopupViewModel)
    {
        InitializeComponent();
        BindingContext = serieMopupViewModel;
    }
}