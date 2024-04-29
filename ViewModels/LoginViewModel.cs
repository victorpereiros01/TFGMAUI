using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using System.Data;
using TFGMaui.Services;
using System.Diagnostics;
using TFGMaui.Utils;
using TFGMaui.Repositories;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    internal partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        public LoginViewModel()
        {
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
