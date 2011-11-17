<%@ Page Language="C#" 
Inherits="OpenRasta.Codecs.WebForms.ResourceView<System.Collections.Generic.IEnumerable<Rest.Aurant.Restaurant>>" %>
<%@ Import Namespace="OpenRasta.Web" %>
<%@ Import Namespace="OpenRasta.Web.Markup" %>
<%@ Import Namespace="Rest.Aurant" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>
        All our great restaurants!
    </title>
</head>
<body>
    <h1>The greatest restaurants</h1>
    <ol itemscope="itemscope" itemtype="http://schema.org/Restaurant">
    <% foreach(var restaurant in Database.Restaurants){ %>
        <li><div>
        <span itemprop="name"><%= restaurant.Name %></span>
            -
             <a href="<%= restaurant.CreateUri() %>" itemprop="url"><%= restaurant.CreateUri() %></a>
             </div>
             <div itemprop="address">
                <%=restaurant.Address %>
             </div>
             </li>

    <% } %>
    </ol>
</body>
</html>
