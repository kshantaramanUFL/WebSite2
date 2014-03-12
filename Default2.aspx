<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default2.aspx.cs" Inherits="Default2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:FileUpload runat="server" ID="Uploader" /><br /><br />
    <asp:Button runat="server" ID="UploadButton" Text="Upload" 
        onclick="UploadButton_Click" />
     <asp:Label ID="Label1" runat="server" Text="Label">
     <br />
     <br /></asp:Label>
     <br /><br />
     <asp:DropDownList ID="DropDownList1" runat="server" style="text-align: right" EnableViewState="true">
     </asp:DropDownList>
     </form>
</body>
</html>
