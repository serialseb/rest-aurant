<%@ Page Language="C#" 
Inherits="OpenRasta.Codecs.WebForms.ResourceView<System.Collections.Generic.IEnumerable<Rest.Aurant.Booking>>" %>
<%@ Import Namespace="OpenRasta.Web.Markup" %>
<%@ Import Namespace="Rest.Aurant" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
    </title>
</head>
<body>
    <h1>New Reservation</h1>
    <% using (scope(Xhtml.Form(Resource).Method("POST"))) { %>
    <fieldset>
        <label>name:
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
