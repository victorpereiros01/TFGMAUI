using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFGMaui.Services;
using ABI.System;
using System.Diagnostics;

namespace TFGMaui.ViewModels
{
    internal partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private UsuarioModel usuarioActivo;

        public LoginViewModel()
        {
            UsuarioActivo = new() { NombreUsuario = "admin", Password = "admin" };
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
                using SqlConnection oconexion = new(IConstantes.ConnectionString);
                String query = "SELECT NombreUsuario, Email, Avatar FROM Usuarios u WHERE u.NombreUsuario = @Nombre or u.Email = @Nombre AND u.Password = @Pass";

                SqlCommand cmd = new SqlCommand(query, oconexion);
                cmd.Parameters.AddWithValue("@Nombre", UsuarioActivo.NombreUsuario);
                cmd.Parameters.AddWithValue("@Pass", UsuarioActivo.Password);
                cmd.CommandType = CommandType.Text;

                oconexion.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        UsuarioActivo.NombreUsuario = dr.GetString(0);
                        UsuarioActivo.Email = dr.GetString(1);
                        //UsuarioActivo.Avatar = dr.GetByte(2);
                    }
                }

                oconexion.Close();
                Navegar("MainPage");

                await Application.Current.MainPage.DisplayAlert("Saludos", "Bienvenid@ " + UsuarioActivo.NombreUsuario, "Aceptar");
            }
            catch (System.Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Creedenciales incorrectas", "Aceptar");
                Debug.WriteLine(e.Message);
            }

        }
    }
}
