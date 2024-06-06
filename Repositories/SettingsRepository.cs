using System.Data;
using TFGMaui.ViewModels;

namespace TFGMaui.Repositories
{
    /// <summary>
    /// Clase que contiene los metodos para cambiar los datos del usuario
    /// </summary>
    internal class SettingsRepository : Repository
    {
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

        public bool ChangeEmail(string nuevoEmail, UsuarioModel user)
        {
            SetCmdQuery("UPDATE [dbo].[Users] SET [Password] = @Email WHERE IdUser = @IdUser");

            AddCmdParameters(new() { { "@IdUser", user.Id }, { "@Email", nuevoEmail } });

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

        public bool ChangeLanguage(UsuarioModel user)
        {
            SetCmdQuery("UPDATE [dbo].[Users] SET [Language] = @Language WHERE IdUser = @IdUser");

            AddCmdParameters(
                new()
                {
                    { "@Language", user.Language },
                    { "@IdUser", user.Id }
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
