using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGMaui.Models
{
    public class QuoteModel
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        public string Content { get; set; }

        public string Author { get; set; }

        public List<string> Tags { get; set; }

        public int Maxlength { get; set; }
    }
}
