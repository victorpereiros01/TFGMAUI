using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Utils;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    internal partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [ObservableProperty]
        private ObservableCollection<HobbieModel> items;

        [ObservableProperty]
        private ObservableCollection<LanguageModel> languages;

        [ObservableProperty]
        private LanguageModel language;

        [ObservableProperty]
        private string pass;

        [ObservableProperty]
        private bool isDark;
        [ObservableProperty]
        private string colorStr;

        [ObservableProperty]
        private string nuevaPass;
        [ObservableProperty]
        private string nuevoNombre;
        [ObservableProperty]
        private string nuevoEmail;

        [ObservableProperty]
        private ImageSource avatar;

        [ObservableProperty]
        private string base64;

        [ObservableProperty]
        private bool adulto;

        public SettingsViewModel()
        {
            ColorStr = "Oscuro"; IsDark = true;
            Languages = [
                new LanguageModel() { Imagen= ImageSource.FromFile("spanish.png"), Value= "ESPAÑOL", Utf8= "es-ES"},
                new LanguageModel() { Imagen= ImageSource.FromFile("english.png"), Value= "INGLES", Utf8= "en-US"},
                new LanguageModel() { Imagen= ImageSource.FromFile("german.png"), Value= "ALEMAN", Utf8= "de"},
                new LanguageModel() { Imagen= ImageSource.FromFile("italy.png"), Value= "ITALIANO", Utf8= "it"},
                new LanguageModel() { Imagen= ImageSource.FromFile("portuguese.png"), Value= "PORTUGUES", Utf8= "pt"},
                new LanguageModel() { Imagen= ImageSource.FromFile("japanese.png"), Value= "JAPONES", Utf8= "ja"}
            ];
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

        [RelayCommand]
        public async Task ChangeLanguage(string utf8)
        {
            UsuarioActivo.Language = utf8;

            if (new SettingsRepository().ChangeLanguage(UsuarioActivo))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Idioma cambiado satisfactoriamente", "Aceptar");
            }
        }

        /// <summary>
        /// Establece los hobbies para que en la vista funcionen
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task SetHobbies()
        {
            Items =
            [
                new HobbieModel() { IsChecked = UsuarioActivo.Hobbies[0], HobbieType = "CINE" },
                new HobbieModel() { IsChecked = UsuarioActivo.Hobbies[1], HobbieType = "MANGANIME" },
                new HobbieModel() { IsChecked = UsuarioActivo.Hobbies[2], HobbieType = "VIDEOJUEGOS" },
                new HobbieModel() { IsChecked = UsuarioActivo.Hobbies[3], HobbieType = "LIBROS" }
            ];

            Avatar = UsuarioActivo.Avatar;
            Adulto = UsuarioActivo.Adulto;
        }

        [RelayCommand]
        private async void BrowserOpen_Clicked()
        {
            Uri uri = new("https://www.microsoft.com");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }

        [RelayCommand]
        private async Task EditAvatar()
        {
            var f = await FileUtils.OpenFile();

            if (f is null)
            {
                return;
            }

            Base64 = f.FileBase64;
            Avatar = f.ImageSource;
        }

        /// <summary>
        /// Busca la imagen con el nombre de usuario, e inserta o actualiza la imagen
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ChangeAvatar()
        {
            if (Base64.IsNullOrEmpty())
            {
                return;
            }

            if (new SettingsRepository().ChangeAvatar(Base64, UsuarioActivo))
            {
                UsuarioActivo.Avatar = Avatar;
                await App.Current.MainPage.DisplayAlert("Exito", "Avatar cambiado satisfactoriamente", "Aceptar");
            }
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            if (pagina.Equals("LoginPage"))
            {
                await SecureStorage.SetAsync("credentialsStored", false.ToString());
                await SecureStorage.SetAsync("username", " ");
                await SecureStorage.SetAsync("password", " ");
                UsuarioActivo = new();
            }

            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioActivo,
                ["IsRememberMe"] = false
            });
        }

        /// <summary>
        /// Si la contraseña es igual cambia la contraseña del usuario
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task ChangePass()
        {
            if (NuevaPass.IsNullOrEmpty())
            {
                return;
            }

            Pass = await App.Current.MainPage.DisplayPromptAsync("Alerta", "Introduce tu contraseña");

            if (Pass is null)
            {
                return;
            }

            if (!Pass.Equals(UsuarioActivo.Password.Trim()))
            {
                return;
            }

            if (new SettingsRepository().ChangePass(NuevaPass, UsuarioActivo))
            {
                UsuarioActivo.Password = NuevaPass;
                if (SecureStorage.GetAsync("credentialsStored").Equals(true.ToString()))
                {
                    await SecureStorage.SetAsync("password", UsuarioActivo.Password);
                }

                NuevaPass = "";
                await App.Current.MainPage.DisplayAlert("Exito", "Contraseña cambiada satisfactoriamente", "Aceptar");
            }
        }

        /// <summary>
        /// Si la contraseña es igual cambia la contraseña del usuario
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task ChangeEmail()
        {
            if (NuevoEmail.IsNullOrEmpty())
            {
                return;
            }

            Pass = await App.Current.MainPage.DisplayPromptAsync("Alerta", "Introduce tu contraseña");

            if (Pass is null)
            {
                return;
            }

            if (!Pass.Equals(UsuarioActivo.Password.Trim()))
            {
                return;
            }

            if (new SettingsRepository().ChangeEmail(NuevoEmail, UsuarioActivo))
            {
                UsuarioActivo.Email = NuevoEmail;
                NuevoEmail = "";
                await App.Current.MainPage.DisplayAlert("Exito", "Email cambiado satisfactoriamente", "Aceptar");
            }
        }

        /// <summary>
        /// Si la contraseña es igual cambia el modo parental
        /// </summary>
        [RelayCommand]
        public async Task EnableParentalMode()
        {
            Adulto = UsuarioActivo.Adulto;

            Pass = await App.Current.MainPage.DisplayPromptAsync("Alerta", "Introduce tu contraseña");

            if (!Pass.Equals(UsuarioActivo.Password.Trim()))
            {
                return;
            }

            UsuarioActivo.Adulto = !UsuarioActivo.Adulto;
            if (new SettingsRepository().ChangeParentalMode(UsuarioActivo))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Modo parental cambiado satisfactoriamente", "Aceptar");
            }
        }

        [RelayCommand]
        public async Task ChangeDarkLight()
        {
            if (IsDark)
            {
                ColorStr = "Claro";
                IsDark = false;
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                ColorStr = "Oscuro";
                IsDark = true;
                Application.Current.UserAppTheme = AppTheme.Light;
            }
        }

        /// <summary>
        /// Si la contraseña es igual cambia el nombre de usuario(si no está cogido) con el id de usuario
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task ChangeUsername()
        {
            if (NuevoNombre.IsNullOrEmpty())
            {
                return;
            }

            Pass = await App.Current.MainPage.DisplayPromptAsync("Alerta", "Introduce tu contraseña");

            if (Pass is null)
            {
                return;
            }

            if (!Pass.Equals(UsuarioActivo.Password.Trim()))
            {
                return;
            }

            if (new SettingsRepository().ChangeUsername(NuevoNombre, UsuarioActivo))
            {
                UsuarioActivo.Username = NuevoNombre;
                if (SecureStorage.GetAsync("credentialsStored").Equals(true.ToString()))
                {
                    await SecureStorage.SetAsync("username", UsuarioActivo.Username);
                }

                NuevoNombre = "";
                await App.Current.MainPage.DisplayAlert("Exito", "Nombre de usuario cambiado satisfactoriamente", "Aceptar");
            }
        }

        /// <summary>
        /// Actualiza los hobbies del usuario por su nombre
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task ChangeHobbies()
        {
            if (Items.Count == 0)
            {
                return;
            }

            UsuarioActivo.Hobbies = [];
            Items.ToList().ForEach(x => UsuarioActivo.Hobbies.Add(x.IsChecked));

            if (new SettingsRepository().ChangeHobbies(UsuarioActivo))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbies cambiados satisfactoriamente", "Aceptar");
            }
        }
    }
}
