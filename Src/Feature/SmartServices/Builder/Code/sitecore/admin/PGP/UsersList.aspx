﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UsersList.aspx.cs" 
    Inherits="DEWAXP.Feature.Builder.sitecore.admin.PGP.UsersList" %>

<%@ Import Namespace="DEWAXP.Feature.Builder.Models.ProjectGeneration" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Admin console</title>
    <link rel="shortcut icon" href="/sitecore/images/favicon.ico" />
    <link href="/sitecore/shell/client/Speak/Assets/css/speak-default-theme.css" rel="stylesheet" type="text/css" />
</head>
<body class="sc sc-fullWidth">
    <form id="form1" runat="server">
    <header class="sc-globalHeader">
        <div class="row sc-globalHeader-content">
            <div class="col-md-6">
                <div class="sc-globalHeader-startButton">
                </div>
                <div class="sc-globalHeader-navigationToggler">
                </div>
            </div>
            <div class="col-md-6">
                <div class="sc-globalHeader-loginInfo">

                    <ul data-sc-id="AccountInformation" class="sc-accountInformation" data-sc-require="/-/speak/v1/business/AccountInformation.js">
                        
                        <li>
                            <asp:LinkButton runat="server" ID="btnlogout" OnClick="Logout_Click">Logout</asp:LinkButton>
                        </li>
                        
                        <li>Administrator
                            <img src="/~/icon/People/16x16/Astrologer.png" />
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </header>


    <section class="sc-applicationContent">
        <header class="sc-applicationHeader">

            <div class="sc-applicationHeader-row1">
                <div class="sc-applicationHeader-content">
                    <div class="sc-applicationHeader-title">
                        User adminstration
                    </div>
                </div>


            </div>
            <div class="sc-applicationHeader-row2">
                <div class="sc-applicationHeader-back">
                     
                </div>
                <div class="sc-applicationHeader-contextSwitcher">
                </div>
                <div class="sc-applicationHeader-actions">
                   <a href="UsersList.aspx"><strong>User management</strong></a><span style="padding:20px"></span>
                    <a href="Projects.aspx"><strong>Project management</strong></a>
                </div>
            </div>

        </header>


        <div class="row sc-contentRowFix">
            <section class="col-md-9 sc-applicationContent-main">
                <div data-sc-id="Main" class="sc-border sc-show-padding sc_Border_98 data-sc-registered">

    
    <div>
        <a href="/sitecore/admin/PGP/edituser.aspx">Add a new user</a><br/>
    <asp:repeater runat="server" ID="rptProjects">
         <HeaderTemplate>
            <table cellspacing="0" rules="all" border="1" class="sc-table sc-table-header table">
            <tr>
                <th scope="col" style="width: 25%" class="sc-text-align-left sc-table-head">
                    User name
                </th>
                <th scope="col" style="width: 25%" class="sc-text-align-left sc-table-head">
                    Email address
                </th>
               
            </tr>
        </HeaderTemplate>
        <ItemTemplate>
        <tr>
            <td>
                <asp:HyperLink runat="server" NavigateUrl='<%# "/sitecore/admin/PGP/edituser.aspx?id=" + (Container.DataItem as ProjectUser).Id%>'>
                    <%# (Container.DataItem as ProjectUser).UserName %>
                </asp:HyperLink>
            </td>
            <td>
                <asp:Label ID="lblContactName" runat="server" Text='<%# (Container.DataItem as ProjectUser).Emailaddress %>' />
            </td>
           
        </tr>
    </ItemTemplate>
         <FooterTemplate>
        </table>
    </FooterTemplate>
    </asp:repeater>
        
        <asp:Label runat="server" ID="lblNoprojects" Visible="False">There are no users set up yet</asp:Label>
    </div>
    </form>
 </div>
            </section>

        </div>
    </section>


</body>
</html>
