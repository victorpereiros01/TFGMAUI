using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TFGMaui.Models;
using TFGMaui.Repositories;
using Windows.UI.ViewManagement;
using TFGMaui.Utils;
using Microsoft.UI.Windowing;
using Microsoft.UI;

namespace TFGMaui.ViewModels
{
    internal partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool firstPageReg;

        [ObservableProperty]
        private UsuarioModel usuarioReg;

        [ObservableProperty]
        private string repContra;

        [ObservableProperty]
        private Color colorAcc, txColor;

        [ObservableProperty]
        private ObservableCollection<HobbieModel> items;

        [ObservableProperty]
        private bool isPassword, isPasswordRep;

        public RegisterViewModel()
        {
            ColorAcc = Color.Parse(new UISettings().GetColorValue(UIColorType.Accent).ToString());
            TxColor = MiscellaneousUtils.ColorIsDarkOrLight(colorAcc);

            UsuarioReg = new();
            FirstPageReg = true;

            IsPassword = true;
            IsPasswordRep = true;

            Items =
            [
                new HobbieModel() { IsChecked = false, HobbieType = "Cinema" },
                new HobbieModel() { IsChecked = false, HobbieType = "Manganime" },
                new HobbieModel() { IsChecked = false, HobbieType = "Games" },
                new HobbieModel() { IsChecked = false, HobbieType = "Books & comics" }
            ];
            UsuarioReg.Hobbies = [];
        }

        [RelayCommand]
        private async Task ChangePassVis()
        {
            IsPassword = !IsPassword;
        }

        [RelayCommand]
        private async Task ChangePassRepVis()
        {
            IsPasswordRep = !IsPasswordRep;
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            FirstPageReg = true;
            RepContra = string.Empty;

            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioReg
            });
        }

        [RelayCommand]
        public async Task MinimizeApp()
        {
#if WINDOWS
            var nativeWindow = App.Current.Windows.First().Handler.PlatformView;
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

            AppWindow appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(windowHandle));

            if (appWindow.Presenter is OverlappedPresenter p)
            {
                p.Minimize();
            }
#endif
        }

        [RelayCommand]
        public async Task CloseApp()
        {
            Application.Current.Quit();
        }

        /// <summary>
        /// Cambia entre las dos paginas del registro
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task CambiarPagina()
        {
            // Esta en la primera pagina
            if (FirstPageReg)
            {
                try
                {
                    // Si no existe el usuario esta valido para crearlo
                    if (new AuthRepository().UserExists(UsuarioReg))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "El nombre de usuario está en uso", "Aceptar");
                    }
                    else
                    {
                        if (!RepContra.Equals(UsuarioReg.Password))
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "Aceptar");
                        }
                        else
                        {
                            // Cambia a la segunda pagina
                            FirstPageReg = false;
                        }
                    }
                }
                catch { }
            }
            else
            {
                // Esta en la segunda pagina y cambia a la primera
                FirstPageReg = true;
            }
        }

        /// <summary>
        /// Sube a la base de datos los datos del usuario, como los hobbies y la imagen
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task Registrar()
        {
            try
            {
                Items.ToList().ForEach(x => UsuarioReg.Hobbies.Add(x.IsChecked));

                if (new AuthRepository().Registrar(UsuarioReg))
                {
                    new AuthRepository().SetImageDefault(usuarioReg.Username);

                    await App.Current.MainPage.DisplayAlert("Usuario creado", "Se ha registrado el usuario correctamente", "Aceptar");
                    await Navegar("LoginPage");
                }
            }
            catch { }
        }
    }
}
