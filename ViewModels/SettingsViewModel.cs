using System.Collections.ObjectModel;
using System.Data;
using Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Mopups.PreBaked.PopupPages.SingleResponse;
using Mopups.PreBaked.Services;
using RestSharp;
using TFGMaui.Models;
using TFGMaui.Repositories;
using TFGMaui.Services;
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
        private string passP;

        [ObservableProperty]
        private bool isDark;
        [ObservableProperty]
        private string color;

        [ObservableProperty]
        private string nuevaPass;
        [ObservableProperty]
        private string nuevaPassRep;

        [ObservableProperty]
        private string nuevoNombre;

        [ObservableProperty]
        private ImageSource avatar;

        [ObservableProperty]
        private string base64;

        [ObservableProperty]
        private bool adulto;

        public SettingsViewModel()
        {
            Color = "Oscuro"; IsDark = true;
            Languages = [
                new LanguageModel() { Imagen= ImageSource.FromFile("user.png"), Value= "Spanish", Utf8= "es-ES"},
                new LanguageModel() { Imagen= ImageSource.FromFile("user.png"), Value= "English", Utf8= "en-US"},
                new LanguageModel() { Imagen= ImageSource.FromFile("user.png"), Value= "German", Utf8= "de"},
                new LanguageModel() { Imagen= ImageSource.FromFile("user.png"), Value= "Italian", Utf8= "it"},
                new LanguageModel() { Imagen= ImageSource.FromFile("user.png"), Value= "Portuguese", Utf8= "pt"},
                new LanguageModel() { Imagen= ImageSource.FromFile("user.png"), Value= "Japanese", Utf8= "ja"}
            ];
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
                new HobbieModel() { IsChecked = UsuarioActivo.Hobbies[0], NombreHobbie = "Cinema" },
                new HobbieModel() { IsChecked = UsuarioActivo.Hobbies[1], NombreHobbie = "Manganime" },
                new HobbieModel() { IsChecked = UsuarioActivo.Hobbies[2], NombreHobbie = "Games" },
                new HobbieModel() { IsChecked = UsuarioActivo.Hobbies[3], NombreHobbie = "Books & comics" }
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

            if (f == null)
            {
                return;
            }

            base64 = f.FileBase64;
            Avatar = f.ImageSource;
            UsuarioActivo.Avatar = Avatar;
        }

        /// <summary>
        /// Busca la imagen con el nombre de usuario, e inserta o actualiza la imagen
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ChangeAvatar()
        {
            if (new SettingsRepository().ChangeAvatar(base64, UsuarioActivo))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Avatar cambiado satisfactoriamente", "Aceptar");
            }
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
        public async Task LogOut(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = new UsuarioModel(),
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
            if (!NuevaPass.Equals(NuevaPassRep))
            {
                return;
            }

            if (!PassP.Equals(UsuarioActivo.Password.Trim()))
            {
                return;
            }

            if (new SettingsRepository().ChangePass(NuevaPass, UsuarioActivo))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Contraseña cambiada satisfactoriamente", "Aceptar");
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
                Color = "Claro";
                IsDark = false;
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                Color = "Oscuro";
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
            if (!Pass.Equals(UsuarioActivo.Password.Trim()))
            {
                return;
            }

            if (new SettingsRepository().ChangeUsername(NuevoNombre, UsuarioActivo))
            {
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
            UsuarioActivo.Hobbies = [];
            Items.ToList().ForEach(x => UsuarioActivo.Hobbies.Add(x.IsChecked));

            if (new SettingsRepository().ChangeHobbies(UsuarioActivo))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Hobbies cambiados satisfactoriamente", "Aceptar");
            }
        }
    }
}
