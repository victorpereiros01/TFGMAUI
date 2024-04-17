namespace TFGMaui.Utils
{
    public class RecursoModel
    {
        public string Id { get; set; }
        public string FileBase64 { get; set; }
        public string Name { get; set; }
    }

    public static class FileUtils
    {
        internal static async Task<ImageSource> GetSource(string base64Full)
        {
            var s = base64Full.Split(",");

            return ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(s[1])));
        }

        internal static async Task<RecursoModel> OpenFile()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona un archivo"
            });

            if (result != null)
            {
                var recurso = new RecursoModel();

                var streamForImageBase64 = await result.OpenReadAsync();

                // Fileinfo ayuda a obtener el tipo de archivo del q obtiene por el filepicker
                FileInfo fi = new FileInfo(result.FileName);
                recurso.Name = fi.Name;

                var msstream = new MemoryStream();
                streamForImageBase64.CopyTo(msstream);

                string convert = Convert.ToBase64String(msstream.ToArray());

                recurso.FileBase64 = "data:" + result.ContentType + ";base64," + convert;

                return recurso;
            }

            return null;
        }
    }
}
