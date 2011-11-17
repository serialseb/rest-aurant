<%@ Page Language="C#" Inherits="OpenRasta.Codecs.WebForms.ResourceView<Rest.Aurant.IndexPage>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%= Resource.Message %>
    </title>
</head>
<body>
    <div>
        <%= Resource.Message %>
    </div>
    <div>
        Links:
        <ul>
            <li><a href="/restaurants">View restaurants</a></li>
        </ul>
    </div>
    <form action="." method="POST">
        <fieldset>
            <legend>Update homepage title</legend>
            <input type="text" name="Message" value="<%= Resource.Message %>" />
            <input type="submit" />
        </fieldset>
    </form>
</body>
</html>
