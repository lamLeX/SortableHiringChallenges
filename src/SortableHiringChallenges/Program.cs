using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SortableHiringChallenges
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var productInputRaw = File.ReadLines(args[0]);
            var listingInputRaw = File.ReadLines(args[1]);

            var productsInput = productInputRaw.Select(JsonConvert.DeserializeObject<Product>);
            var listingsInput = listingInputRaw.Select(JsonConvert.DeserializeObject<Listing>);

            var productManufacturers = productsInput.GroupBy(prod => prod.Manufacturer).ToList();

            var prodGrouping = from prod in productsInput
                               group prod by prod.Manufacturer into groupByManufacturer
                               from prodInGroupByManufacturer in (from prod in groupByManufacturer
                                                                  group prod by prod.Family ?? "NoFamily")
                               group prodInGroupByManufacturer by groupByManufacturer.Key;
            var productLookup = prodGrouping.ToDictionary(groupByManufacturer => groupByManufacturer.Key,
                prodByManufacturer => prodByManufacturer.ToDictionary(groupByFamily => groupByFamily.Key, prod => prod.ToList()));

            var listingGroupedByManufacturers =
                listingsInput.GroupBy(
                        listing =>
                            productManufacturers.FirstOrDefault(m => listing.Manufacturer.ToLower().Replace(" ", "")
                                                                           .Contains(m.Key.ToLower().Replace(" ", ""))
                            )?.Key ?? "Other")
                .Where(group => group.Key != "Other");


            var results = (from productGroup in productLookup
                           let listingByCurrentManufacturer = listingGroupedByManufacturers.Where(listingGroup => listingGroup.Key == productGroup.Key).SelectMany(prod => prod).ToList()
                           from productGroupByFamily in productGroup.Value
                           let familyRegexKey = productGroupByFamily.Key.Replace("-", "-*").Replace(" ", " *")
                           let listingForThisFamily = listingByCurrentManufacturer.Where(listing => Regex.IsMatch(listing.Title, familyRegexKey, RegexOptions.IgnoreCase)).ToList()
                           from product in productGroupByFamily.Value
                           let modelRegexKey = product.Model.Replace("-", "-*").Replace(" ", " *")
                           let listingForThisProduct = listingForThisFamily.Where(listing => Regex.IsMatch(listing.Title, modelRegexKey, RegexOptions.IgnoreCase)).ToList()
                           select new Result()
                           {
                               ProductName = product.ProductName,
                               Listings = listingForThisProduct
                           }).ToList();

            File.WriteAllLines("results.txt", results.Select(JsonConvert.SerializeObject));
        }
    }
}
