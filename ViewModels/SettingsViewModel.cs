using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFGMaui.Services;
using TFGMaui.Utils;

namespace TFGMaui.ViewModels
{
    [QueryProperty("UsuarioActivo", "UsuarioActivo")]
    internal partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        [RelayCommand]
        private async void BrowserOpen_Clicked()
        {
            Uri uri = new Uri("https://www.microsoft.com");
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
    }
}
