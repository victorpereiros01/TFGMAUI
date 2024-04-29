using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFGMaui.Services;
using TFGMaui.ViewModels;

namespace TFGMaui.Repositories
{
    /// <summary>
    /// Clase que contiene los metodos para cambiar los datos del usuario
    /// </summary>
    internal class SettingsRepository
    {
        private readonly SqlConnection Oconexion;

        public SqlCommand Cmd { get; set; }

        /// <summary>
        /// Añade la query
        /// </summary>
        /// <param name="query"></param>
        public void SetCmdQuery(string query)
        {
            Cmd.CommandText = query;
        }

        /// <summary>
        /// Añade los parametros al comando
        /// </summary>
        /// <param name="p"></param>
        private void AddCmdParameters(Dictionary<string, object> p)
        {
            foreach (var item in p)
            {
                if (p.TryGetValue(item.Key, out var value))
                    Cmd.Parameters.AddWithValue(item.Key, value);
            }
        }

        public SettingsRepository()
        {
            Oconexion = new(IConstantes.ConnectionString);
            Cmd = new SqlCommand { Connection = Oconexion };
        }

        public bool ChangeAvatar(string base64, UsuarioModel user)
        {
            SetCmdQuery("UPDATE [dbo].[Users] SET [Avatar] = @Avatar WHERE IdUser = @IdUser");

            AddCmdParameters(new() { { "@IdUser", user.Id }, { "@Avatar", base64 } });

            Cmd.CommandType = CommandType.Text;
            Oconexion.Open();

            // Check Error
            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        public bool ChangePass(string nuevaPass, UsuarioModel user)
        {
            SetCmdQuery("UPDATE [dbo].[Users] SET [Password] = @Pass WHERE IdUser = @IdUser");

            AddCmdParameters(new() { { "@IdUser", user.Id }, { "@Pass", nuevaPass } });

            Cmd.CommandType = CommandType.Text;
            Oconexion.Open();

            // Check Error
            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        public bool ChangeParentalMode(UsuarioModel user)
        {
            SetCmdQuery("UPDATE [dbo].[Users] SET [Adult] = @Adulto WHERE IdUser = @IdUser");

            AddCmdParameters(new() { { "@Adulto", user.Adulto }, { "@IdUser", user.Id } });

            Cmd.CommandType = CommandType.Text;
            Oconexion.Open();

            // Check Error
            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        public bool ChangeUsername(string nuevoNombre, UsuarioModel user)
        {
            SetCmdQuery(
                "UPDATE [dbo].[Users] SET [Username] = @NuevoNombre  WHERE IdUser = @IdUser"
            );

            AddCmdParameters(new() { { "@IdUser", user.Id }, { "@NuevoNombre", nuevoNombre } });

            Oconexion.Open();

            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        public bool ChangeHobbies(UsuarioModel user)
        {
            SetCmdQuery(
                "UPDATE [dbo].[Users] SET [Hobbie1] = @Hobbie1, [Hobbie2] = @Hobbie2, [Hobbie3] = @Hobbie3, [Hobbie4] = @Hobbie4 WHERE IdUser = @IdUser"
            );

            AddCmdParameters(
                new()
                {
                    { "@IdUser", user.Id },
                    { "@Hobbie1", user.Hobbies[0] },
                    { "@Hobbie2", user.Hobbies[1] },
                    { "@Hobbie3", user.Hobbies[2] },
                    { "@Hobbie4", user.Hobbies[3] }
                }
            );

            Oconexion.Open();

            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }
    }
}
