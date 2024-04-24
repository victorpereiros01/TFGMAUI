using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using System.Data;
using TFGMaui.Services;
using System.Diagnostics;
using TFGMaui.Utils;

namespace TFGMaui.ViewModels
{
    internal partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        public LoginViewModel()
        {
            UsuarioActivo = new() { NombreUsuario = "@admin", Password = "admin" };
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
        public async Task Login()
        {
            try
            {
                var user = new UsuarioModel
                {
                    Email = UsuarioActivo.NombreUsuario,
                    Password = UsuarioActivo.Password,
                    NombreUsuario = UsuarioActivo.NombreUsuario
                };

                using SqlConnection oconexion = new(IConstantes.ConnectionString);
                string query = "SELECT NombreUsuario, Email, f.ValorImagenEnc, Password FROM Usuarios u join Imagenes f on f.IdImagen=u.Avatar WHERE u.NombreUsuario = @Nombre or u.Email = @Nombre AND u.Password = @Pass";

                SqlCommand cmd = new(query, oconexion);
                cmd.Parameters.AddWithValue("@Nombre", user.NombreUsuario);
                cmd.Parameters.AddWithValue("@Pass", user.Password);
                cmd.CommandType = CommandType.Text;

                oconexion.Open();
                using SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    UsuarioActivo.NombreUsuario = dr.GetString(0).Trim();
                    UsuarioActivo.Email = dr.GetString(1).Trim();
                    UsuarioActivo.Avatar = FileUtils.GetSource(dr.GetString(2)).Result;
                    UsuarioActivo.Password = dr.GetString(3).Trim();

                    if (user.Equals(UsuarioActivo))
                    {
                        await Navegar("MainPage");

                        await Application.Current.MainPage.DisplayAlert("Saludos", "Bienvenid@ " + UsuarioActivo.NombreUsuario, "Aceptar");
                    }
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Creedenciales incorrectas", "Aceptar");
                Debug.WriteLine(e.Message);
            }

        }
    }
}
