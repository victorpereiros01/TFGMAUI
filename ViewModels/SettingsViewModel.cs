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

            try
            {
                using SqlConnection connection = new(IConstantes.ConnectionString);
                string query = "UPDATE [dbo].[Users] SET [Avatar]=@Avatar WHERE Username=@Nombre";

                using SqlCommand command = new(query, connection);

                command.Parameters.AddWithValue("@Nombre", UsuarioActivo.Username);
                command.Parameters.AddWithValue("@Avatar", base64);

                command.CommandType = CommandType.Text;
                connection.Open();

                // Check Error
                if (command.ExecuteNonQuery() < 1)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Error al cambiar la contraseña", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Exito", "Contraseña cambiada satisfactoriamente", "Aceptar");
                }
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Actualizacion", e.Message, "Aceptar");
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
                ["UsuarioActivo"] = new()
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

            if (!Pass.Equals(UsuarioActivo.Password))
            {
                return;
            }

            try
            {
                using SqlConnection connection = new(IConstantes.ConnectionString);
                string query = "UPDATE [dbo].[Users] SET [Password]=@Pass WHERE Username=@Nombre";

                using SqlCommand command = new(query, connection);

                command.Parameters.AddWithValue("@Nombre", UsuarioActivo.Username);
                command.Parameters.AddWithValue("@Pass", NuevaPass);

                command.CommandType = CommandType.Text;
                connection.Open();

                // Check Error
                if (command.ExecuteNonQuery() < 1)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Error al cambiar la contraseña", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Exito", "Contraseña cambiada satisfactoriamente", "Aceptar");
                }
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Actualizacion", e.Message, "Aceptar");
            }
        }

        /// <summary>
        /// Si la contraseña es igual cambia el modo parental
        /// </summary>
        [RelayCommand]
        public async Task EnableParentalMode()
        {
            Pass = await App.Current.MainPage.DisplayPromptAsync("Alerta", "Introduce tu contraseña");

            if (!Pass.Equals(UsuarioActivo.Password))
            {
                return;
            }

            try
            {
                using SqlConnection connection = new(IConstantes.ConnectionString);
                string query = "UPDATE [dbo].[Users] SET [Adult]=@Adulto WHERE Username=@Nombre";

                using SqlCommand command = new(query, connection);

                command.Parameters.AddWithValue("@Adulto", UsuarioActivo.Adulto);
                command.Parameters.AddWithValue("@Nombre", UsuarioActivo.Username);

                command.CommandType = CommandType.Text;
                connection.Open();

                // Check Error
                if (command.ExecuteNonQuery() < 1)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Error al cambiar el modo parental", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Exito", "Modo parental cambiado satisfactoriamente", "Aceptar");
                }
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Actualizacion", e.Message, "Aceptar");
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

            if (!Pass.Equals(UsuarioActivo.Password))
            {
                return;
            }

            try
            {
                using SqlConnection connection = new(IConstantes.ConnectionString);
                SqlCommand command = connection.CreateCommand();
                command.Connection = connection;
                string query = "SELECT IdUsuario FROM Users WHERE Username=@Nombre";
                command.CommandText = query;
                command.Parameters.AddWithValue("@Nombre", UsuarioActivo.Username);

                connection.Open();
                int idUsuario = (int)command.ExecuteScalar();

                if (idUsuario == 0)
                {
                    return;
                }

                string query2 = "UPDATE [dbo].[Users] SET [Username]=@NuevoNombre  WHERE IdUsuario=@IdUsuario";
                command.CommandText = query2;
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.AddWithValue("@NuevoNombre", NuevoNombre);

                // Check Error
                if (command.ExecuteNonQuery() < 1)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Error el nombre de usuario", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Exito", "Nombre de usuario cambiado satisfactoriamente", "Aceptar");
                }
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Actualizacion", e.Message, "Aceptar");
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

            try
            {
                using SqlConnection connection = new(IConstantes.ConnectionString);
                string query = "UPDATE [dbo].[Users] SET [Hobbie1]=@Hobbie1, [Hobbie2]=@Hobbie2, [Hobbie3]=@Hobbie3, [Hobbie4]=@Hobbie4 WHERE Username=@Nombre";

                using SqlCommand command = new(query, connection);

                command.Parameters.AddWithValue("@Nombre", UsuarioActivo.Username);
                command.Parameters.AddWithValue("@Hobbie1", UsuarioActivo.Hobbies[0]);
                command.Parameters.AddWithValue("@Hobbie2", UsuarioActivo.Hobbies[1]);
                command.Parameters.AddWithValue("@Hobbie3", UsuarioActivo.Hobbies[2]);
                command.Parameters.AddWithValue("@Hobbie4", UsuarioActivo.Hobbies[3]);

                command.CommandType = CommandType.Text;
                connection.Open();

                // Check Error
                if (command.ExecuteNonQuery() < 1)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Error al cambiar los hobbies", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Exito", "Hobbies cambiados satisfactoriamente", "Aceptar");
                }
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Actualizacion", e.Message, "Aceptar");
            }
        }
    }
}
