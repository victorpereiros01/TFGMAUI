using Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Mopups.PreBaked.PopupPages.SingleResponse;
using Mopups.PreBaked.Services;
using RestSharp;
using System.Collections.ObjectModel;
using System.Data;
using TFGMaui.Models;
using TFGMaui.Services;
using TFGMaui.Utils;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    [QueryProperty("Items", "Items")]
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

        [RelayCommand]
        private async Task ChangeAvatar()
        {
            using SqlConnection connection = new(IConstantes.ConnectionString);

            connection.Open();

            SqlCommand command = connection.CreateCommand();
            SqlTransaction transaction;

            // Start a local transaction.
            transaction = connection.BeginTransaction();

            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = connection;
            command.Transaction = transaction;

            try
            {
                // Obtiene la imagen en base64
                string base64 = (await FileUtils.OpenFile()).FileBase64;

                string existsQuery = "SELECT COUNT(1) FROM [Imagenes] WHERE Imagenes.NombreCol= @NombreCol";

                string insert = "INSERT INTO [dbo].[Imagenes] ( [ValorImagenEnc], [NombreCol] ) VALUES  ( @Avatar, @NombreCol )";
                string update = "UPDATE [dbo].[Imagenes] SET [ValorImagenEnc] = @Avatar WHERE Imagenes.NombreCol = @NombreCol";
                command.Parameters.AddWithValue("@Avatar", base64);
                command.Parameters.AddWithValue("@NombreCol", UsuarioActivo.NombreUsuario);
                command.CommandText = existsQuery;

                // Hace el primer comando para poner la imagen en la base de datos y lo ejecuta
                int exists = (int)command.ExecuteScalar();

                command.CommandText = exists > 0 ? update : insert;
                var lastNum = command.ExecuteNonQuery();

                // Conecta el avatar del usuario con la imagen de la tabla de imagenes y lo ejecuta
                command.CommandText = "UPDATE [dbo].[Usuarios] SET Avatar = f.IdImagen from Usuarios RIGHT join Imagenes f on f.NombreCol= @NombreCol WHERE Usuarios.NombreUsuario=@NombreCol";
                command.ExecuteNonQuery();

                // Attempt to commit the transaction.
                transaction.Commit();

                await App.Current.MainPage.DisplayAlert("Exito", "Foto cambiada satisfactoriamente", "Aceptar");
            }
            catch (Exception ex)
            {
                // Attempt to roll back the transaction.
                try
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Error al intentar cambiar la imagen", "Aceptar");
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    // This catch block will handle any errors that may have occurred
                    // on the server that would cause the rollback to fail, such as
                    // a closed connection.
                    await App.Current.MainPage.DisplayAlert("Error", ex2.Message, "Aceptar");
                }
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

        [RelayCommand]
        public async Task ChangeLanguage() { }

        [RelayCommand]
        public async Task ChangePass()
        {
            Pass = await App.Current.MainPage.DisplayPromptAsync("Alerta", "Introduce tu contraseña");

            if (!CheckCreedentials())
            {
                return;
            }

            try
            {
                using SqlConnection connection = new(IConstantes.ConnectionString);
                string query = "UPDATE [dbo].[Usuarios] SET [Password]=@Pass WHERE NombreUsuario=@Nombre";

                using SqlCommand command = new(query, connection);

                command.Parameters.AddWithValue("@Nombre", UsuarioActivo.NombreUsuario);
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

        [RelayCommand]
        public async Task ChangeUsername()
        {
            Pass = await App.Current.MainPage.DisplayPromptAsync("Alerta", "Introduce tu contraseña");

            if (!CheckCreedentials())
            {
                return;
            }

            try
            {
                using SqlConnection connection = new(IConstantes.ConnectionString);
                SqlCommand command = connection.CreateCommand();
                command.Connection = connection;
                string query = "SELECT IdUsuario FROM Usuarios WHERE NombreUsuario=@Nombre";
                command.CommandText = query;
                command.Parameters.AddWithValue("@Nombre", UsuarioActivo.NombreUsuario);

                connection.Open();
                int idUsuario = (int)command.ExecuteScalar();

                if (idUsuario == 0)
                {
                    return;
                }

                string query2 = "UPDATE [dbo].[Usuarios] SET [NombreUsuario]=@NuevoNombre  WHERE IdUsuario=@IdUsuario";
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

        public bool CheckCreedentials()
        {
            //using SqlConnection connection = new(IConstantes.ConnectionString);
            //string query = "SELECT COUNT(*) FROM Usuarios WHERE Nombre=@Nombre Password = @Password";

            //using SqlCommand command = new(query, connection);

            //command.Parameters.AddWithValue("@Nombre", UsuarioActivo.NombreUsuario);
            //command.Parameters.AddWithValue("@Password", Pass);

            //command.CommandType = CommandType.Text;
            //connection.Open();

            //return (int)command.ExecuteScalar() == 1;

            return Pass.Equals(UsuarioActivo.Password);
        }

        [RelayCommand]
        public async Task ChangeHobbies()
        {
            UsuarioActivo.Hobbies = [];
            Items.ToList().ForEach(x => UsuarioActivo.Hobbies.Add(x.IsChecked));

            try
            {
                using SqlConnection connection = new(IConstantes.ConnectionString);
                string query = "UPDATE [dbo].[Usuarios] SET [Hobbie1]=@Hobbie1, [Hobbie2]=@Hobbie2, [Hobbie3]=@Hobbie3, [Hobbie4]=@Hobbie4 WHERE NombreUsuario=@Nombre";

                using SqlCommand command = new(query, connection);

                command.Parameters.AddWithValue("@Nombre", UsuarioActivo.NombreUsuario);
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

        /// <summary>
        /// Ask the password to change the adult mode
        /// </summary>
        [RelayCommand]
        public async Task EnableParentalMode()
        {
            try
            {
                using SqlConnection connection = new(IConstantes.ConnectionString);
                string query = "UPDATE [dbo].[Usuarios] SET [Adult]=@Adulto WHERE NombreUsuario=@Nombre";

                using SqlCommand command = new(query, connection);

                command.Parameters.AddWithValue("@Adulto", UsuarioActivo.Adulto);
                command.Parameters.AddWithValue("@Nombre", UsuarioActivo.NombreUsuario);

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
    }
}
