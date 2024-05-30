using Mopups.Pages;
using TFGMaui.ViewModels.Mopup;

namespace TFGMaui.Views.Mopups;

public partial class BookMopup : PopupPage
{
    public BookMopup(BookMopupViewModel bookMopupViewModel)
    {
        InitializeComponent();
        BindingContext = bookMopupViewModel;
    }
}