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
    [QueryProperty("IsRememberMe", "IsRememberMe")]
    internal partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private Color colorAcc, txColor, colorAccCom, txColorCom;

        [ObservableProperty]
        private bool isPassword;

        [ObservableProperty]
        private bool isRememberMe;

        public LoginViewModel()
        {
            UsuarioActivo = new();
            IsPassword = true;
            CheckStoredCredentials();

            ColorAcc = Color.Parse(new UISettings().GetColorValue(UIColorType.Accent).ToString());
            TxColor = MiscellaneousUtils.ColorIsDarkOrLight(ColorAcc);

            ColorAccCom = ColorAcc.GetComplementary();
            TxColorCom = MiscellaneousUtils.ColorIsDarkOrLight(colorAccCom);
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            if (pagina.Equals("RegisterPage"))
            {
                UsuarioActivo = new();
            }
            else if (pagina.Equals("MainPage"))
            {
                UsuarioActivo.Guest = UsuarioActivo.Username.Equals("admin");
            }

            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["IsGuest"] = UsuarioActivo.Guest
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

                if (UsuarioActivo is null)
                {
                    return;
                }

                await SecureStorage.SetAsync("credentialsStored", IsRememberMe.ToString());
                await SecureStorage.SetAsync("username", IsRememberMe ? UsuarioActivo.Username : " ");
                await SecureStorage.SetAsync("password", IsRememberMe ? UsuarioActivo.Password : " ");

                await Navegar("MainPage");
                await Application.Current.MainPage.DisplayAlert("Saludos", "Bienvenid@ " + UsuarioActivo.Name, "Aceptar");                
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.ToString(), "Aceptar");
                UsuarioActivo = new();
            }
        }

        private async void CheckStoredCredentials()
        {
            if (!(IsRememberMe = await SecureStorage.GetAsync("credentialsStored") == bool.TrueString))
            {
                return;
            }

            string username = await SecureStorage.GetAsync("username");
            string password = await SecureStorage.GetAsync("password");

            UsuarioActivo = new() { Username = username, Password = password };

            await Login();
        }

        [RelayCommand]
        public async Task ChangeRemember()
        {
            IsRememberMe = !IsRememberMe;
        }

        [RelayCommand]
        public async Task LoginGuest()
        {
            UsuarioActivo = new() { Username = "admin", Password = "admin" };

            await Login();
        }

        [RelayCommand]
        private async Task ChangePassVis()
        {
            IsPassword = !IsPassword;
        }
    }
}
