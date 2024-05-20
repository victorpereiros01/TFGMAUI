using Mopups.Pages;
using TFGMaui.ViewModels.Mopup;

namespace TFGMaui.Views.Mopups;

public partial class BookMopup : PopupPage
{
    public BookMopup(MovieMopupViewModel movieMopupViewModel)
    {
        InitializeComponent();
        BindingContext = movieMopupViewModel;
    }
}