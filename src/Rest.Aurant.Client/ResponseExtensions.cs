using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Rest.Aurant.Client;

static internal class ResponseExtensions
{
    public static string GetLinkValue(this WebResponse response, string rel)
    {
        return (from header in response.Headers["Link"].Split(',')
                let components = header.Split(';')
                let uri = components[0]
                where components.Any(
                    component =>
                    Regex.IsMatch(component,
                                  @"rel\s*=\s*" + Regex.Escape(rel)))
                select uri.Substring(1, uri.Length - 2))
            .FirstOrDefault();
    }

    public static IEnumerable<Dictionary<string, string>> ItemScope(this XDocument document, string schema, params string[] propertyNames)
    {
        schema = schema.StartsWith("http") ? schema : "http://schema.org/" + schema;

        return from node in document.Document.Descendants()
               where node.HAttr("itemtype") == schema
               where node.HAttr("itemscope") == "itemscope"
               select (from property in node.Descendants()
                       let propName = property.HAttr("itemprop")
                       where propertyNames.Contains(propName)
                       select  property).ToDictionary(_=>_.HAttr("itemprop"), _=>_.Value);
    }
}