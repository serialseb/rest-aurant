<%@ Page Language="C#" 
Inherits="OpenRasta.Codecs.WebForms.ResourceView<Rest.Aurant.Booking>" %>
<%@ Import Namespace="OpenRasta.Web.Markup" %>
<%@ Import Namespace="Rest.Aurant" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
    </title>
</head>
<body>
    <h1>Existing booking</h1>
    <dl>
        <dt>Id</dt>
        <dd><%= Resource.Id %></dd>

        <dt>Name</dt>
        <dd><%= Resource.Name %></dd>
        
        <dt>Covers</dt>
        <dd><%= Resource.Covers %></dd>
    </dl>
</body>
</html>
