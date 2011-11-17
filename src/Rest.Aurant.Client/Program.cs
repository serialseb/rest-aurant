using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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

            var restaurant = DisplayRestaurant(selectedRestaurant);
            var name = ReadData("Name: ");
            string numberOfCovers = ReadData("Number of covers");
            BookTable(restaurant, name, int.Parse(numberOfCovers));

        }

        private static void BookTable(XDocument restaurant, string name, int numberOfcovers)
        {
            var form = restaurant.Document.HDescendants("form")
                                          .Where(_ => _.HAttr("name") == "http://rest.aurant.org/table-booker")
                                          .First();
            var destinationHref = form.HAttr("action");
            var content = string.Format("Name={0}&Covers={1}", name, numberOfcovers);

            var destinationUri = destinationHref ?? restaurant.BaseUri;
            var request = GetDefaultPostRequest(destinationUri);
            using (var writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8))
                writer.Write(content);
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.Created)
                Console.WriteLine("Yay you're in!");
        }

        private static WebRequest GetDefaultPostRequest(string destinationUri)
        {
            var request = WebRequest.Create(destinationUri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            return request;
        }

        private static string ReadData(string requestText)
        {
            Console.Write(requestText);
            return Console.ReadLine();
        }

        private static XDocument DisplayRestaurant(Uri selectedRestaurant)
        {
            var restaurantDocument = XDocument.Load(selectedRestaurant.ToString(), LoadOptions.SetBaseUri);
            var restaurant = restaurantDocument
                .ItemScope("Restaurant")
                .First();
            restaurant.WriteConsole();
            if (restaurant["acceptsReservations"] == "True")
                Console.WriteLine("Press enter to book a table");
            else
                Console.WriteLine("The restaurant does not take bookings");

            return restaurantDocument;
        }

        private static Uri SelectRestaurantUri(Uri restaurantListUri)
        {
            var document = XDocument.Load(restaurantListUri.ToString())
                                    .ItemScope("http://schema.org/Restaurant")
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
        public static IEnumerable<XElement> HDescendants(this XContainer element, string elementName)
        {
            return element.Descendants(XName.Get(elementName, XHTML_NS));
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
