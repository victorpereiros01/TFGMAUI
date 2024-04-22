namespace TFGMaui.Models
{
    internal partial class HttpRequestModel
    {
        // Ruta a la que accederá dependiendo si quiere ver peliculas, libros, etc
        public string Url { get; set; }

        // Como: elemento por id o las peliculas mejor valoradas
        public string Endpoint { get; set; }

        // Parametros como el lenguaje o la api_key
        public Dictionary<string, string> Parameters { get; set; }

        // Headers como authorization o que acepte 'MIME types (text/html, application/json, etc.)' 
        public Dictionary<string, string> Headers { get; set; }

        // Cuerpo para filtrar los resultados
        public string? Body { get; set; }

        public HttpRequestModel(string url, string endpoint, Dictionary<string, string> parameters, Dictionary<string, string> headers, string body)
        {
            Url = url;
            Endpoint = endpoint;
            Parameters = parameters;
            Headers = headers;
            Body = body;
        }

        public HttpRequestModel() { }

        public HttpRequestModel(string url, string endpoint, Dictionary<string, string> parameters, Dictionary<string, string> headers)
        {
            Url = url;
            Endpoint = endpoint;
            Parameters = parameters;
            Headers = headers;
        }
    }
}
