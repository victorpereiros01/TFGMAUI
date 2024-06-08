namespace TFGMaui.Utils
{
    internal class RecursoModel
    {
        public string Id { get; set; }

        public string FileBase64 { get; set; }

        public ImageSource ImageSource { get; set; }
    }

    public static class FileUtils
    {
        /// <summary>
        /// Obtiene la imagen para colocarla en la vista
        /// </summary>
        /// <param name="base64Full"></param>
        /// <returns>Un source base64</returns>
        internal static ImageSource GetSource(string base64Full)
        {
            var s = base64Full.Split(",");

            return ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(s[1])));
        }

        /// <summary>
        /// Abre el explorador de archivos y obtiene una imagen
        /// </summary>
        /// <returns>Imagen en texto para subir a la base de datos</returns>
        internal static async Task<RecursoModel?> OpenFile()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona un archivo",
                FileTypes = FilePickerFileType.Images
            });

            if (result is null)
            {
                return null;
            }

            var stream = await result.OpenReadAsync();
            var image = ImageSource.FromStream(() => stream);

            var streamForImageBase64 = await result.OpenReadAsync();
            var msstream = new MemoryStream();
            streamForImageBase64.CopyTo(msstream);

            return new RecursoModel
            {
                FileBase64 = "data:" + result.ContentType + ";base64," + Convert.ToBase64String(msstream.ToArray()),
                ImageSource = image
            };
        }
    }
}
