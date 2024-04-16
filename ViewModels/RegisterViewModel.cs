using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using TFGMaui.Models;
using TFGMaui.Services;
using TFGMaui.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TFGMaui.ViewModels
{
    internal partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool firstState;

        [ObservableProperty]
        private UsuarioModel usuarioReg;

        [ObservableProperty]
        private string repContra;

        [ObservableProperty]
        private ObservableCollection<HobbieModel> items;

        public RegisterViewModel()
        {
            UsuarioReg = new();
            FirstState = true;
            Items =
            [
                new HobbieModel() { IsChecked = false, NombreHobbie="Cinema" },
                new HobbieModel() { IsChecked = false, NombreHobbie="Manganime" },
                new HobbieModel() { IsChecked = false, NombreHobbie="Games" },
                new HobbieModel() { IsChecked = false, NombreHobbie="Books & comics" }
            ];
            UsuarioReg.Hobbies = [];
        }

        [RelayCommand]
        public async Task Navegar(string pagina)
        {
            await Shell.Current.GoToAsync("//" + pagina, new Dictionary<string, object>()
            {
                ["UsuarioActivo"] = UsuarioReg
            });
        }

        [RelayCommand]
        public async Task CambiarEstado()
        {
            if (FirstState)
            {
                if (!RepContra.Equals(UsuarioReg.Password))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "Aceptar");
                }
                else
                {
                    FirstState = !CheckUserDoesntExists();
                }
            }
            else
            {
                FirstState = true;
            }
        }

        /// <summary>
        /// Metodo que evalua que el usuario exista o no
        /// </summary>
        /// <returns>True si no existe</returns>
        public bool CheckUserDoesntExists()
        {
            // Conexion con la base de datos sql server
            using SqlConnection oconexion = new(IConstantes.ConnectionString);

            SqlCommand cmd = new("SELECT 1 FROM Usuarios u WHERE @NombreUsuario = u.NombreUsuario or @Email = u.Email", oconexion);

            // Añade valores de entrada del stored procedure sql server
            cmd.Parameters.AddWithValue("@NombreUsuario", UsuarioReg.NombreUsuario);
            cmd.Parameters.AddWithValue("@Password", UsuarioReg.Password);
            cmd.Parameters.AddWithValue("@Email", UsuarioReg.Email);

            cmd.CommandType = CommandType.Text;
            oconexion.Open();

            // Checkea que si devuelve algo el usuario existe, por lo que da error
            return cmd.ExecuteScalar() == null;
        }

        public async Task ActualizarImagenDef()
        {
            using SqlConnection connection = new(IConstantes.ConnectionString);
            string query = "UPDATE [dbo].[Usuarios] SET Avatar = f.IdImagen from Usuarios RIGHT join Imagenes f on f.NombreCol = 'default' WHERE Usuarios.NombreUsuario = @Nombre";

            using SqlCommand command = new(query, connection);

            command.Parameters.AddWithValue("@Nombre", UsuarioReg.NombreUsuario);

            command.CommandType = CommandType.Text;
            connection.Open();

            // Check Error
            if (command.ExecuteNonQuery() < 1)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Error al actualizar el avatar", "Aceptar");
            }

            await App.Current.MainPage.DisplayAlert("Actualizacion", "Imagen actualizada", "Aceptar");
        }

        [RelayCommand]
        public async Task Registrar()
        {
            foreach (var item in Items)
            {
                UsuarioReg.Hobbies.Add(item.IsChecked);
            }

            try
            {
                using SqlConnection connection = new(IConstantes.ConnectionString);
                string query = "INSERT INTO Usuarios  ( [NombreUsuario], [Email], [Password], [Hobbie1], [Hobbie2], [Hobbie3], [Hobbie4]) \nVALUES (@Nombre, @Email, @Password, @Hobbie1, @Hobbie2, @Hobbie3, @Hobbie4)";

                using SqlCommand command = new(query, connection);

                command.Parameters.AddWithValue("@Nombre", UsuarioReg.NombreUsuario);
                command.Parameters.AddWithValue("@Email", UsuarioReg.Email);
                command.Parameters.AddWithValue("@Password", UsuarioReg.Password);
                command.Parameters.AddWithValue("@Hobbie1", UsuarioReg.Hobbies[0]);
                command.Parameters.AddWithValue("@Hobbie2", UsuarioReg.Hobbies[1]);
                command.Parameters.AddWithValue("@Hobbie3", UsuarioReg.Hobbies[2]);
                command.Parameters.AddWithValue("@Hobbie4", UsuarioReg.Hobbies[3]);

                command.CommandType = CommandType.Text;
                connection.Open();

                // Check Error
                if (command.ExecuteNonQuery() < 1)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Error al registrar", "Aceptar");
                }
                else
                {
                    await ActualizarImagenDef();

                    await App.Current.MainPage.DisplayAlert("Usuario creado", "Se ha registrado el usuario correctamente", "Aceptar");
                }
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Actualizacion", e.Message, "Aceptar");
            }
        }
    }
}
