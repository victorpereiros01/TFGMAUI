using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TFGMaui.Repositories;
using TFGMaui.Utils;
using Windows.UI.ViewManagement;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    internal partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private Color colorAcc, txColor;

        public LoginViewModel()
        {
            ColorAcc = Color.Parse(new UISettings().GetColorValue(UIColorType.Accent).ToString());
            //ColorAcc = Color.FromArgb("#f5ee8e");   // Claro
            //ColorAcc = Color.FromArgb("#143261");   // Oscuro
            TxColor = MiscellaneousUtils.ColorIsDarkOrLight(colorAcc);
            UsuarioActivo = new() { Username = "@admin", Password = "admin" };
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo
            });
        }

        /// <summary>
        /// Logea con el usuario o email
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task Login()
        {
            UsuarioActivo = new AuthRepository().Login(UsuarioActivo.Username, UsuarioActivo.Password)!;

            if (UsuarioActivo == null)
            {
                return;
            }

            await Navegar("MainPage");
            await Application.Current.MainPage.DisplayAlert("Saludos", "Bienvenid@ " + UsuarioActivo.Username, "Aceptar");
        }
    }
}
