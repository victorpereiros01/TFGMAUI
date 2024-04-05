using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Data.SqlClient;
using TFGMaui.Models;
using TFGMaui.Services;

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
                new HobbieModel() { IsChecked = false, NombreHobbie="Peliculas" },
                new HobbieModel() { IsChecked = false, NombreHobbie="Series" },
                new HobbieModel() { IsChecked = false, NombreHobbie="Manga" },
                new HobbieModel() { IsChecked = false, NombreHobbie="Anime" },
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
        public async Task SiguienteEstado()
        {
            FirstState = !FirstState;
        }

        [RelayCommand]
        public async Task Registrar()
        {
            string Mensaje = string.Empty;

            if (!RepContra.Equals(UsuarioReg.Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "Aceptar");
            }
            else
            {
                foreach (var item in Items)
                {
                    UsuarioReg.Hobbies.Add(item.IsChecked);
                }

                try
                {   // Conexion con la base de datos sql server
                    using SqlConnection oconexion = new(IConstantes.ConnectionString);

                    SqlCommand cmd = new("SP_REGISTRAR_USUARIO", oconexion);
                    // Añade valores de entrada del stored procedure sql server
                    cmd.Parameters.AddWithValue("NombreUsuario", UsuarioReg.NombreUsuario);
                    cmd.Parameters.AddWithValue("Password", UsuarioReg.Password);
                    cmd.Parameters.AddWithValue("Email", UsuarioReg.Email);

                    // Añade parametros de vuelta del procedimiento
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    // Pone el tipo del comando a procedimiento abre la conexion y la ejecuta
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    // Obtiene el mensaje de la base de datos
                    Mensaje = Convert.ToString(cmd.Parameters["Mensaje"].Value);
                }
                catch (Exception ex)
                {
                    Mensaje = ex.Message;
                }

                await App.Current.MainPage.DisplayAlert("Actualizacion", Mensaje, "Aceptar");
            }
        }
    }
}
