using Azure;
using Newtonsoft.Json;
using RestSharp;
using TFGMaui.Models;

namespace TFGMaui.Services
{
    internal class HttpService
    {
        /// <summary>
        /// Método para ejecutar una solicitud HTTP y obtener una respuesta asincrónica
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<object> ExecuteRequestAsync<T>(HttpRequestModel requestModel)
        {
            // Establece la pagina, la extension, los encabezados y los parametros
            var client = new RestClient(requestModel.Url);
            var request = new RestRequest(requestModel.Endpoint);
            request.AddHeaders(requestModel.Headers);
            foreach (var item in requestModel.Parameters)
            {
                if (!requestModel.Parameters.TryGetValue(item.Key, out var value))
                    continue;
                request.AddParameter(item.Key, value);
            }

            // Ejecuta la request y si da error lanza una excepcion
            var response = await client.ExecuteGetAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception("Request incorrecta");
            }

            return JsonConvert.DeserializeObject<T>(response.Content!)!;
        }
    }
}
