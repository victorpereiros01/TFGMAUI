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
        private string pass;

        [ObservableProperty]
        private string nuevaPass;

        [ObservableProperty]
        private string nuevoNombre;

        [RelayCommand]
        public async Task ChangeLanguage() { }

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
        }

        [RelayCommand]
        private async void BrowserOpen_Clicked()
        {
            Uri uri = new("https://www.microsoft.com");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }

        /// <summary>
        /// Busca la imagen con el nombre de usuario, e inserta o actualiza la imagen
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ChangeAvatar()
        {
            string base64 = (await FileUtils.OpenFile()).FileBase64;

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
                ["UsuarioActivo"] = new UsuarioModel()
            });
        }

        /// <summary>
        /// Si la contraseña es igual cambia la contraseña del usuario
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task ChangePass()
        {
            Pass = await App.Current.MainPage.DisplayPromptAsync("Alerta", "Introduce tu contraseña");

            if (!Pass.Equals(UsuarioActivo.Password.Trim()))
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
            Pass = await App.Current.MainPage.DisplayPromptAsync("Alerta", "Introduce tu contraseña");

            if (!Pass.Equals(UsuarioActivo.Password.Trim()))
            {
                return;
            }

            if (new SettingsRepository().ChangeParentalMode(UsuarioActivo))
            {
                await App.Current.MainPage.DisplayAlert("Exito", "Modo parental cambiado satisfactoriamente", "Aceptar");
            }
        }

        /// <summary>
        /// Si la contraseña es igual cambia el nombre de usuario(si no está cogido) con el id de usuario
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task ChangeUsername()
        {
            Pass = await App.Current.MainPage.DisplayPromptAsync("Alerta", "Introduce tu contraseña");

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
