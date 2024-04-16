namespace TFGMaui.ViewModels
{
    internal class UsuarioModel
    {
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public ImageSource Avatar { get; set; }
        public List<bool> Hobbies { get; set; }
    }
}