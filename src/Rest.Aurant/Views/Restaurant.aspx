<%@ Page Language="C#" Inherits="OpenRasta.Codecs.WebForms.ResourceView<System.Collections.Generic.IEnumerable<Rest.Aurant.Restaurant>>" %>

<%@ Import Namespace="OpenRasta.Web.Markup" %>
<%@ Import Namespace="Rest.Aurant" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <%--<h1><%= Resource. %></h1>--%>
    <% using (scope(Xhtml.Form(Resource).Method("POST"))) { %>
    <fieldset>
        <legend>New reservation</legend>
        <label>
            name:
            <%= Xhtml.TextBox<Booking>(_=>_.Name) %>
        </label>
        <%= label
                ["covers: "]
                [Xhtml.TextBox<Booking>(_=>_.Covers)] %>
        <input type="submit" value="Reserve!" />
    </fieldset>
    <%} %>
</body>
</html>
