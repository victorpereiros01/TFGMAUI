﻿using Newtonsoft.Json;
using RestSharp;
using System.Reflection;
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

            if (requestModel.Parameters is not null)
            {
                foreach (var item in requestModel.Parameters)
                {
                    if (!requestModel.Parameters.TryGetValue(item.Key, out var value))
                        continue;
                    request.AddParameter(item.Key, value);
                }
            }

            // Ejecuta la request y si da error lanza una excepcion
            var response = await client.ExecuteGetAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception("Request incorrecta");
            }

            var model = JsonConvert.DeserializeObject<T>(response.Content!)!;

            if (!(model.GetType() == typeof(BookModel)))
            {
                return model;
            }

            foreach (PropertyInfo property in model.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(string) && property.GetValue(model) == null)
                {
                    property.SetValue(model, "no data");
                }
            }

            return model;
        }
    }
}
