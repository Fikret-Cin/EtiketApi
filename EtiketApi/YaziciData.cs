using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EtiketApi
{
    public class YaziciData
    {
        [JsonProperty("yaziciAdi")]
        public string YaziciAdi { get; set; }
        [JsonProperty("etiketAdedi")]
        public int EtiketAdedi { get; set; }
        [JsonProperty("etiket")]
        public string Etiket { get; set; }

        [JsonProperty("kaynak")]
        public string Kaynak { get; set; }

        [JsonProperty("etiketData")]
        public List<EtiketData> EtiketData { get; set; }
    }
    public class EtiketData
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }



}