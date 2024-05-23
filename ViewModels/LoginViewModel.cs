using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI;
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
        private Color colorAcc, txColor, colorAccCom, txColorCom;

        [ObservableProperty]
        private bool isPassword;

        public LoginViewModel()
        {
            IsPassword = true;

            ColorAcc = Color.Parse(new UISettings().GetColorValue(UIColorType.Accent).ToString());
            TxColor = MiscellaneousUtils.ColorIsDarkOrLight(ColorAcc);

            ColorAccCom = ColorAcc.GetComplementary();
            TxColorCom = MiscellaneousUtils.ColorIsDarkOrLight(colorAccCom);

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
        /// Logea con el usuario o email
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task Login()
        {
            try
            {
                UsuarioActivo = new AuthRepository().Login(UsuarioActivo.Username, UsuarioActivo.Password)!;

                if (UsuarioActivo == null)
                {
                    return;
                }

                await Navegar("MainPage");
                await Application.Current.MainPage.DisplayAlert("Saludos", "Bienvenid@ " + UsuarioActivo.Username, "Aceptar");
            }
            catch { }
        }

        [RelayCommand]
        public async Task LoginGuest()
        {
            try
            {
                UsuarioActivo = new AuthRepository().Login(UsuarioActivo.Username, UsuarioActivo.Password)!;

                if (UsuarioActivo == null)
                {
                    return;
                }

                await Navegar("MainPage");
                await Application.Current.MainPage.DisplayAlert("Saludos", "Bienvenid@ " + UsuarioActivo.Username, "Aceptar");
            }
            catch { }
        }

        [RelayCommand]
        private async Task ChangePassVis()
        {
            IsPassword = !IsPassword;
        }
    }
}
