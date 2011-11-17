using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Rest.Aurant.Client
{
    class Program
    {
        private const string REL_RESTAURANT_LIST = "http://rest.aurant.org/restaurant-list";
        static void Main(string[] args)
        {
            var restaurantListUri = DiscoverRestaurantListUri();
            Console.WriteLine("Using " + restaurantListUri);

            var selectedRestaurant = SelectRestaurantUri(restaurantListUri);
            Console.WriteLine("Using " + selectedRestaurant);

            DisplayRestaurant(selectedRestaurant);

        }

        private static void DisplayRestaurant(Uri selectedRestaurant)
        {
            var restaurant = XDocument.Load(selectedRestaurant.ToString())
                .ItemScope("Restaurant")
                .First();
            restaurant.WriteConsole();
            if (restaurant["acceptsReservations"] == "True")
                Console.WriteLine("Press enter to book a table");
            else
                Console.WriteLine("The restaurant does not take bookings");
            Console.ReadLine();
        }

        private static Uri SelectRestaurantUri(Uri restaurantListUri)
        {
            var document = XDocument.Load(restaurantListUri.ToString())
                                    .ItemScope("http://schema.org/Restaurant", "name", "address", "url")
                                    .ToList();

            foreach (var restaurant in document)
            {
                Console.WriteLine("{0}. {1} - {2}", 
                    document.IndexOf(restaurant) + 1, restaurant["name"], restaurant["url"]);
            }
            var selected = int.Parse(Console.ReadLine());
            return new Uri(document.ElementAt(selected - 1)["url"], UriKind.Absolute);
        }

        private static Uri DiscoverRestaurantListUri()
        {
            const string defaultHref = "http://localhost:40372";
            Console.Write("Enter server (default {0}):", defaultHref);
            var inputUri = Console.ReadLine();
            inputUri = string.IsNullOrEmpty(inputUri) ? defaultHref : inputUri;

            var req = WebRequest.Create(inputUri);
            var response = req.GetResponse();
            var linkHeader = response.GetLinkValue(REL_RESTAURANT_LIST);
            var baseUri = new Uri(inputUri, UriKind.Absolute);
            var linkUri = new Uri(linkHeader, UriKind.RelativeOrAbsolute);
            return new Uri(baseUri, linkUri);
        }
    }
    public static class Extensions
    {
        const string XHTML_NS = "http://www.w3.org/1999/xhtml";
        public static string HAttr(this XElement element, string attribName)
        {
            var attrib = element.Attribute(attribName);//XName.Get(attribName, XHTML_NS));
            return attrib != null ? attrib.Value : null;
        }
        public static void WriteConsole(this IDictionary<string,string> dic)
        {
            foreach(var kv in dic)
            {
                Console.WriteLine(kv.Key + ": " + kv.Value);
            }
        }
    }
}
