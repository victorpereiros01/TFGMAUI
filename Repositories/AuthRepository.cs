using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using TFGMaui.Utils;
using TFGMaui.ViewModels;

namespace TFGMaui.Repositories
{
    /// <summary>
    /// Clase que contiene los metodos para hacer login y registro
    /// </summary>
    internal class AuthRepository : Repository
    {
        /// <summary>
        /// Metodo que evalua que el usuario exista o no
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UserExists(UsuarioModel user)
        {
            SetCmdQuery("SELECT 1 FROM Users WHERE Username = @Username or Email = @Email");

            AddCmdParameters(new() { { "@Username", user.Username }, { "@Email", user.Email } });

            Oconexion.Open();

            // Si devuelve null el usuario existe
            return Cmd.ExecuteScalar() != null;
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

                SetCmdQuery("UPDATE [dbo].[Users] SET [Avatar] = @Avatar WHERE Username = @Username");

                AddCmdParameters(new() { { "@Username", username }, { "@Avatar", dr.GetString(0) } });
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

        /// <summary>
        /// Devuelve el usuario de la base de datos
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UsuarioModel? Login(string username, string password)
        {
            SetCmdQuery("SELECT IdUser, Username, Email, Avatar, Password, Hobbie1, Hobbie2, Hobbie3, Hobbie4, Adult, Language FROM Users WHERE Username = @Username or Email = @Username AND Password = @Pass");

            AddCmdParameters(new() { { "@Username", username }, { "@Pass", password } });

            // Abre la conexion e inicializa el reader para obtener los datos
            Oconexion.Open();
            using SqlDataReader dr = Cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            var user = new UsuarioModel
            {
                Id = dr.GetInt32(0),
                Username = dr.GetString(1).Trim(),
                Email = dr.GetString(2).Trim(),
                Avatar = FileUtils.GetSource(dr.GetString(3)),
                Password = dr.GetString(4).Trim(),
                Hobbies = [dr.GetBoolean(5), dr.GetBoolean(6), dr.GetBoolean(7), dr.GetBoolean(8)],
                Adulto = dr.GetBoolean(9),
                Language = dr.GetString(10).Trim()
            };

            return user;
        }

        public bool Registrar(UsuarioModel user)
        {
            SetCmdQuery("INSERT INTO Users ([Username], [Email], [Password], [Hobbie1], [Hobbie2], [Hobbie3], [Hobbie4], [Adult], [Language]) \nVALUES (@Username, @Email, @Password, @Hobbie1, @Hobbie2, @Hobbie3, @Hobbie4, @Adulto, 'en-US')");

            AddCmdParameters(
                new()
                {
                    { "@Username", user.Username },
                    { "@Password", user.Password },
                    { "@Hobbie1", user.Hobbies[0] },
                    { "@Hobbie2", user.Hobbies[1] },
                    { "@Hobbie3", user.Hobbies[2] },
                    { "@Hobbie4", user.Hobbies[3] },
                    { "@Adulto", true },
                    { "@Email", user.Email }
                }
            );

            Oconexion.Open();

            // Check Error
            if (Cmd.ExecuteNonQuery() == 0)
            {
                return false;
            }

            return true;
        }

        public static string HashPasswordSHA256(string password)
        {
            byte[] data = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            StringBuilder sBuilder = new();

            foreach (byte v in data)
            {
                sBuilder.Append(v.ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashOfInput = HashPasswordSHA256(inputPassword);

            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, storedHash) == 0;
        }
    }
}
