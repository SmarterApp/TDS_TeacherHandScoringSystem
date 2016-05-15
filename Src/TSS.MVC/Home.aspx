<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="TSS.MVC.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <script type="text/javascript">
        keepalive_url = "openam_keepalive.aspx";
        interval = 6000; // 10 minutes (milliseconds)
        function ssoKeepAlive() {
            $.get(keepalive_url, function (data, status) {
                if (data.substring(0, 2) == "OK") {
                    //alert("Data: " + data + "\nStatus: " + status);
                }
                else {
                    //window.location = "/samldemo/InitiateLogout.aspx"
                    return;
                }
                setTimeout("ssoKeepAlive()", interval);
            });

        }
        window.onload = function () { setTimeout("ssoKeepAlive()", interval); }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <select name="menu" onchange="top.location.href=this.options[this.selectedIndex].value;" value="GO">
            <option selected="selected">Select One</option>
            <option value="http://tide.uat.airast.org/samlDemo/home.aspx">SAML TIDE</option>
            <option value="http://test.reports.airast.org/testuat/home.aspx">SAML Reports</option>
            <option value="http://openamtest1.airast.org/TIDEOpenAM/">Policy agent TDS</option>
        </select>
        <div>
        </div>
        <asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click">logout</asp:LinkButton>
    </form>
</body>
</html>
