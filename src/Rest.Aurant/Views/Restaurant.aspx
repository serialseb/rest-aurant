﻿<%@ Page Language="C#" Inherits="OpenRasta.Codecs.WebForms.ResourceView<Rest.Aurant.Restaurant>" %>

<%@ Import Namespace="OpenRasta.Web" %>
<%@ Import Namespace="OpenRasta.Web.Markup" %>
<%@ Import Namespace="Rest.Aurant" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <dl itemscope="itemscope" itemtype="http://schema.org/Restaurant">
        <dt>Name:</dt>
        <dd itemprop="name"><%= Resource.Name %></dd>
        <dt>Address:</dt>
        <dd itemprop="address"><%= Resource.Address %></dd>
        <dt>Takes bookings:</dt>
        <dd itemprop="acceptsReservations"><%= Resource.AcceptsReservations ? "Yes" : "No"%></dd>
    </dl>
    <% using (scope(Xhtml
           .Form(Resource)
           .Action(typeof(IEnumerable<Booking>).CreateUri(new { restaurantidentifier = Resource.Identifier}))
           .Method("POST").Attr("name", "http://rest.aurant.org/table-booker")))
       { %>
    <fieldset>
        <legend>New reservation</legend>
        <label>
            name:
            <%= Xhtml.TextBox<Booking>(_=>_.Name).Name("name") %>
        </label>
        <%--<%= label
                ["covers: "]
                [Xhtml.TextBox<Booking>(_=>_.NumberOfCovers).Name("numberOfCovers")] %>--%>
        <input type="submit" value="Reserve!" />
    </fieldset>
    <%} %>
</body>
</html>
