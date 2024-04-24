﻿using Azure;
using Newtonsoft.Json;
using RestSharp;
using TFGMaui.Models;

namespace TFGMaui.Services
{
    internal class HttpService
    {
        // Método para ejecutar una solicitud HTTP y obtener una respuesta asincrónica
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
                throw new Exception();
            }

            return JsonConvert.DeserializeObject<T>(response.Content!)!;
        }
    }
}
