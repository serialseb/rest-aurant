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
        static void Main(string[] args)
        {
            var restaurantListUri = DiscoverRestaurantListUri();
            Console.WriteLine("Using " + restaurantListUri);
            var userInput = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                 {
                                     {"name", ReadData("Name: ")},
                                     {"numberOfcovers", ReadData("Covers: ")},
                                     {"longnumber", ReadData("Credit Card: ")}
                                 };


            var selectedRestaurant = SelectRestaurantUri(restaurantListUri);
            Console.WriteLine("Using " + selectedRestaurant);

            var restaurant = DisplayRestaurant(selectedRestaurant);


            BookTable(restaurant, userInput);

        }
        private static void BookTable(XDocument restaurant, IDictionary<string,string> userInput)
        {
            var baseUri = restaurant.BaseUri;

            var response = ProcessForm(restaurant, userInput, baseUri);
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                    Console.WriteLine("Booking made!");
        }

        private static HttpWebResponse ProcessForm(XDocument restaurant, IDictionary<string, string> userInput, string baseUri)
        {
            Console.WriteLine("Do you want to cancel? (y/n)");
            if (Console.ReadLine() == "y")
                throw new NotImplementedException();
            var document = restaurant.Document;
            var form = document.HDescendants("form").First();

            var request = CompleteForm(userInput, baseUri, form);
            var response = (HttpWebResponse) request.GetResponse();
            var responseDoc = XDocument.Load(response.GetResponseStream());

            return responseDoc != null && 
                   responseDoc.HDescendants("form").Any() 
                   ? ProcessForm(responseDoc, userInput, baseUri) 
                   : response;
        }

        private static WebRequest CompleteForm(IDictionary<string, string> userInput, string baseUri, XElement form)
        {
            var content = string.Join("&", from input in form.HDescendants("input")
                                           let name = input.HAttr("name")
                                           where name != null
                                           let isHidden = input.HAttr("type") == "hidden"
                                           let value = isHidden ? input.HAttr("value") : userInput[name]
                                           select string.Format("{0}={1}", name, value));

            var destinationHref = form.HAttr("action");

            var destinationUri = destinationHref ?? baseUri;
            var request = GetDefaultPostRequest(destinationUri);
            using (var writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8))
                writer.Write(content);
            return request;
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
            if (restaurant["acceptsReservations"] == "Yes")
                Console.WriteLine("Press enter to book a table");
            else
                throw new InvalidOperationException("The restaurant does not take bookings");

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
            var linkHeader = response.GetLinkValue(Microdata.REL_RESTAURANT_LIST);
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
