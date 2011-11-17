using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using OpenRasta;
using OpenRasta.Pipeline;
using OpenRasta.Plugins.ContentLocation;
using Rest.Aurant.Handlers;
using Rest.Aurant.Resources;
using OpenRasta.Configuration;
using OpenRasta.Web;
using OpenRasta.Codecs;

namespace Rest.Aurant
{

    public class Configuration : IConfigurationSource
    {
        public void Configure()
        {
            using (OpenRastaConfiguration.Manual)
            {

                //ResourceSpace.Uses.ContentLocation();
                ResourceSpace.Uses.PipelineContributor<WriteLinks>();

                
                ResourceSpace.Has
                    .ResourcesOfType<IndexPage>()
                    .AtUri("/")
                    .HandledBy<IndexPageHandler>()
                    .AsJsonDataContract()
                    .And
                    .RenderedByAspx("~/Views/IndexPage.aspx");

                ResourceSpace.Has
                    .ResourcesOfType<IEnumerable<Restaurant>>()
                    .AtUri("/restaurants")
                    .HandledBy<RestaurantHandler>()
                    .RenderedByAspx("~/Views/Restaurants.aspx");
                ResourceSpace.Has
                    .ResourcesOfType<Restaurant>()
                    .AtUri("/restaurants/{identifier}")
                    .HandledBy<RestaurantHandler>()
                    .RenderedByAspx("~/Views/Restaurant.aspx");

                ResourceSpace.Has
                    .ResourcesOfType<Booking>()
                    .AtUri("/restaurants/bookings/{id}")
                    .HandledBy<BookingsHandler>()
                    .RenderedByAspx("~/Views/Booking.aspx");

                ResourceSpace.Has
                    .ResourcesOfType<IEnumerable<Booking>>()
                    .AtUri("/restaurants/{restaurantidentifier}/bookings")
                    .HandledBy<BookingsHandler>()
                    .RenderedByAspx("~/Views/Bookings.aspx");


                ResourceSpace.Has
                    .ResourcesOfType<MetaRedirect>()
                    .WithoutUri
                    .TranscodedBy<MetaRedirect>();
            }
        }
    }

    public class WriteLinks : IPipelineContributor
    {
        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(WriteLink).Before<KnownStages.IResponseCoding>();
        }

        private PipelineContinuation WriteLink(ICommunicationContext arg)
        {
            arg.Response.Headers.Add("Link",
                string.Format("<{0}>;rel=http://rest.aurant.org/restaurant-list",
                "/restaurants"));
            return PipelineContinuation.Continue;
        }
    }

    public static class Database
    {
        static Database()
        {
            Restaurants = new List<Restaurant>
                              {
                                  new Restaurant
                                      {
                                          Name = "Wahaca",
                                          Address = "Soho",
                                          Identifier = "soho_wahaca",
                                          AcceptsReservations = true
                                      }
                              };
        }
        public static ICollection<Restaurant> Restaurants { get; set; }
        public static ICollection<Booking> Bookings { get; set; }
    }

    public class RestaurantHandler
    {
        public object Get()
        {
            return Database.Restaurants;
        }
        public object Get(string identifier)
        {
            return Database.Restaurants.First(_ => _.Identifier == identifier);
        }
    }

    public class Restaurant
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Identifier { get; set; }

        public bool AcceptsReservations { get; set; }
    }

    public class BookingsHandler
    {
        private static readonly ICollection<Booking> _bookings
            = new List<Booking>();
        public object Get(int id)
        {
            return _bookings.ElementAt(id);
        }
        public object Post(Booking booking)
        {
            booking.Id = _bookings.Count;
            _bookings.Add(booking);

            var redirectLocation = booking.CreateUri();
            return new OperationResult.Created
                       {
                           RedirectLocation = redirectLocation,
                           ResponseResource = new MetaRedirect(redirectLocation)
                       };
        }
        public object Get()
        {
            return _bookings;
        }
    }
    [MediaType("text/html")]
    public class MetaRedirect : IMediaTypeWriter
    {
        public override string ToString()
        {
            return string.Format(
                @"<html>
    <head>
        <meta http-equiv=""refresh"" content=""0; URL={0}"" />
    </head>
  </html>", _redirectLocation);
        }

        private readonly Uri _redirectLocation;
        public MetaRedirect() {}

        public MetaRedirect(Uri redirectLocation)
        {
            _redirectLocation = redirectLocation;
        }

        public object Configuration { get; set; }
        public void WriteTo(object entity, IHttpEntity response, string[] codecParameters)
        {
            var content = Encoding.UTF8.GetBytes(entity.ToString());
            response.Stream.Write(content, 0, content.Length);
        }
    }

    public class Booking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfCovers { get; set; }
        public string RestaurantIdentifier { get; set; }
    }

    public class IndexPageHandler
    {
        private static IndexPage _indexPage;

        static IndexPageHandler()
        {
            _indexPage = new IndexPage {Message = "hello world"};
        }

        public object Get()
        {
            return _indexPage;
        }
        public object Post(IndexPage indexPage)
        {
            _indexPage = indexPage;
            return new OperationResult.SeeOther
                       {
                           RedirectLocation = indexPage.CreateUri()
                       };
        }
    }

    public class IndexPage
    {
        public string Message { get; set; }
    }
}