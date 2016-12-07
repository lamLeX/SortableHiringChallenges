using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SortableHiringChallenges
{
    public class Product
    {
        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("family")]
        public string Family { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("announced-date")]
        public DateTime AnnouncedDate { get; set; }
    }
}
