using Azure.Core;
using Microsoft.Data.SqlClient;
using System.Data;
using TFGMaui.Services;
using TFGMaui.Utils;
using TFGMaui.ViewModels;

namespace TFGMaui.Repositories
{
    internal class AuthCommandRepository
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

        public AuthCommandRepository()
        {
            Oconexion = new(IConstantes.ConnectionString);
            Cmd = new SqlCommand
            {
                Connection = Oconexion
            };
        }

        /// <summary>
        /// Devuelve el usuario de la base de datos
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UsuarioModel? Login(string username, string password)
        {
            SetCmdQuery("SELECT Username, Email, Avatar, Password, Hobbie1, Hobbie2, Hobbie3, Hobbie4, Adult FROM Users WHERE Username = @Username or Email = @Username AND Password = @Pass");

            AddCmdParameters(new()
            {
                { "@Username", username },
                { "@Pass", password}
            });

            // Abre la conexion e inicializa el reader para obtener los datos
            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return new UsuarioModel
            {
                Username = dr.GetString(0),
                Email = dr.GetString(1),
                Avatar = FileUtils.GetSource(dr.GetString(2)),
                Password = dr.GetString(3),
                Hobbies =
                [
                    dr.GetBoolean(4),
                    dr.GetBoolean(5),
                    dr.GetBoolean(6),
                    dr.GetBoolean(7)
                ],
                Adulto = dr.GetBoolean(8)
            };
        }

        /// <summary>
        /// Metodo que evalua que el usuario exista o no
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UserDoesntExists(UsuarioModel user)
        {
            SetCmdQuery("SELECT 1 FROM Users WHERE Username = @Username or Email = @Email");

            AddCmdParameters(new()
            {
                { "@Username", user.Username },
                { "@Email", user.Email}
            });

            Oconexion.Open();

            // Si devuelve null el usuario existe
            return Cmd.ExecuteScalar() == null;
        }

        /// <summary>
        /// Añade los parametros al comando
        /// </summary>
        /// <param name="p"></param>
        private void AddCmdParameters(Dictionary<string, object> p)
        {
            foreach (var item in p)
            {
                if (!p.TryGetValue(item.Key, out var value))
                    continue;
                Cmd.Parameters.AddWithValue(item.Key, value);
            }
        }

        public bool SetImageDefault(string username)
        {
            SetCmdQuery("SELECT Avatar FROM Users WHERE Username = 'admin'");

            // Abre la conexion e inicializa el reader para obtener los datos
            Oconexion.Open();
            using (SqlDataReader dr = Cmd.ExecuteReader())
            {
                if (!dr.Read())
                {
                    return false;
                }

                SetCmdQuery("UPDATE [dbo].[Users] SET [Avatar]= @Avatar WHERE Username = @Username");

                AddCmdParameters(new()
                {
                    { "@Username", username },
                    { "@Avatar", dr.GetString(0)}
                });
            }

            // A la conexion y ejecuta la query devolviendo las columnas afectadas
            Oconexion.Close();
            Oconexion.Open();

            // Si no hay columnas afectadas da error
            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        public bool Registrar(UsuarioModel user)
        {
            SetCmdQuery("INSERT INTO Users ([Username], [Email], [Password], [Hobbie1], [Hobbie2], [Hobbie3], [Hobbie4], [Adult]) \nVALUES (@Username, @Email, @Password, @Hobbie1, @Hobbie2, @Hobbie3, @Hobbie4, @Adulto)");

            AddCmdParameters(new()
            {
                { "@Username", user.Username },
                { "@Password", user.Password },
                { "@Hobbie1", user.Hobbies[0] },
                { "@Hobbie2", user.Hobbies[1] },
                { "@Hobbie3", user.Hobbies[2] },
                { "@Hobbie4", user.Hobbies[3] },
                { "@Adulto", true },
                { "@Email", user.Email}
            });

            Oconexion.Open();

            // Check Error
            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        // Placeholder for password hashing (replace with proper implementation)
        private string HashPassword(string password)
        {
            // Your password hashing implementation here
            return password; // Placeholder implementation
        }

        // Placeholder for password verification (replace with proper implementation)
        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Your password verification implementation here
            return password == hashedPassword; // Placeholder implementation
        }
    }
}
