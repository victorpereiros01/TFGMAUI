using Microsoft.Data.SqlClient;
using TFGMaui.Models;
using TFGMaui.Services;

namespace TFGMaui.Repositories
{
    internal class Repository
    {
        protected readonly SqlConnection Oconexion;

        protected SqlCommand Cmd { get; set; }

        /// <summary>
        /// Añade la query
        /// </summary>
        /// <param name="query"></param>
        protected void SetCmdQuery(string query)
        {
            Cmd.CommandText = query;
        }

        /// <summary>
        /// Añade los parametros al comando
        /// </summary>
        /// <param name="p"></param>
        protected void AddCmdParameters(Dictionary<string, object> p)
        {
            foreach (var item in p)
            {
                if (p.TryGetValue(item.Key, out var value))
                    Cmd.Parameters.AddWithValue(item.Key, value);
            }
        }

        public Repository()
        {
            Oconexion = new(IConstantes.ConnectionString);
            Cmd = new SqlCommand { Connection = Oconexion };
        }

        public QuoteModel GetQuoteRandom()
        {
            SetCmdQuery("SELECT TOP 1 [Value], Source FROM[dbo].[Quotes] ORDER by newid()");

            Oconexion.Open();

            using SqlDataReader dr = Cmd.ExecuteReader();

            if (!dr.Read())
            {
                return null;
            }

            return new QuoteModel()
            {
                Value = dr.GetString(0),
                Source = dr.GetString(1)
            };
        }
    }
}
