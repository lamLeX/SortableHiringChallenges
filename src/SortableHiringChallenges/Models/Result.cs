using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SortableHiringChallenges
{
    public class Result
    {
        [JsonProperty("product_name")]
        public string ProductName { get; set; }
        public List<Listing> Listings { get; set; }
    }
}
