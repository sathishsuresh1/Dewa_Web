<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="edituser.aspx.cs"
     Inherits="DEWAXP.Feature.Builder.sitecore.admin.PGP.EditUser" %>

<%@ Import Namespace="DEWAXP.Feature.Builder.Models.ProjectGeneration" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Admin console</title>
    <link rel="shortcut icon" href="/sitecore/images/favicon.ico" />
    <link href="/sitecore/shell/client/Speak/Assets/css/speak-default-theme.css" rel="stylesheet" type="text/css" />
     <script type="text/javascript">
        function ConfirmDelete() {
            if (confirm('Are you sure you wish to delete this user?'))
                return true;
            else
                return false;
        }
    </script>
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
        <asp:Label runat="server" ID="lblerror" ForeColor="red" EnableViewState="False"> </asp:Label>
    <fieldset>
         <asp:ValidationSummary runat="server" ForeColor="red" DisplayMode="BulletList"
            HeaderText="Following error occurs....."
             ShowSummary="true" />
        <table cellspacing="0" rules="all" border="1" class="sc-table sc-table-header table">
            <tr>
                <td width="35%">
                    <label for="form-field-project" class="form-field__label">
                        User Name  *
                       
                    </label>
                </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtusername" MaxLength="15" ></asp:TextBox>
                         
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtusername"
                            ErrorMessage='User Name is required' Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtusername"
                            ErrorMessage="Invalid User Name" ValidationExpression="^[A-z0-9]+$" ></asp:RegularExpressionValidator>
                    </span>
                </td>
            </tr>
           
            <tr>
                <td>
            <label for="form-field-project" class="form-field__label">
                        Projects
                       
                    </label>
                    </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                      <asp:ListBox runat="server" SelectionMode="Multiple" ID="ddlProjects" Rows="6"
                          DataTextField="ProjectName" DataValueField="Id" />
                        
                    </span>
                    </td>
            </tr>
            <tr>
                <td>
            <label for="form-field-project" class="form-field__label">
                        Email address  *
                       
                    </label>
                    </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtEmailaddress" MaxLength="50"></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmailaddress"
                            ErrorMessage='Email is required' Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmailaddress"
                            ErrorMessage="Invalid email" ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" ></asp:RegularExpressionValidator>
                    </span>
                    </td>
            </tr>
            <tr>
                <td>
            <label for="form-field-project" class="form-field__label">
                        Company name
                       
                    </label>
                    </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtcompanyname" MaxLength="50"></asp:TextBox>
                        
                    </span>
                    </td>
            </tr>
                    <tr>
                <td>
                    <label for="form-field-project" class="form-field__label">
                        City  *
                       
                    </label>
                    </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtCity" MaxLength="30"></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCity"
                            ErrorMessage='City is required' Text="*"></asp:RequiredFieldValidator>
                    </span>
                    </td>
            </tr>
                    <tr>
                <td>
                    <label for="form-field-project" class="form-field__label">
                        P O Box  *
                       
                    </label>
                    </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtPobox" MaxLength="10"></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPobox"
                            ErrorMessage='P O Box is required' Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPobox"
                            ErrorMessage="Invalid P O Box" ValidationExpression="^[0-9]*$" ></asp:RegularExpressionValidator>
                    </span>
                    </td>
            </tr>
                    <tr>
                <td>
                    <label for="form-field-project" class="form-field__label">
                        Telephone number  *
                       
                    </label>
                    </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtTelephone" MaxLength="15"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTelephone"
                            ErrorMessage='Telephone is required' Text="*"></asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator runat="server" ControlToValidate="txtTelephone"
                            ErrorMessage="Invalid phone number" ValidationExpression="\d{9}$" ></asp:RegularExpressionValidator>
                    </span>
                    </td>
            </tr>
                    <tr>
                <td>
                    <label for="form-field-project" class="form-field__label">
                        Mobile number
                       
                    </label>
                    </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtMobile" MaxLength="15"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtMobile"
                            ErrorMessage="Invalid mobile number" ValidationExpression="\d{10}$" ></asp:RegularExpressionValidator>
                    </span>
                    </td>
            </tr>
                    <tr>
                <td>
                    <label for="form-field-project" class="form-field__label">
                        Fax number  *
                       
                    </label>
                    </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtFax" MaxLength="15"></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFax"
                            ErrorMessage='Fax is required' Text="*"></asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator runat="server" ControlToValidate="txtFax"
                            ErrorMessage="Invalid fax number" ValidationExpression="\d{9}$" ></asp:RegularExpressionValidator>
                    </span>
                    </td>
            </tr>
                    <tr>
                <td>
                    <label for="form-field-project" class="form-field__label">
                        Company Location
                       
                    </label>
                    </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtLocation"></asp:TextBox>
                        
                    </span>
                    </td>
            </tr>
            </table>
                    
                    
                    
                    
                    
                    
        
        <asp:Button runat="server" ID="btnSave" OnClick="btnSave_OnClick" Text="Save"/>
        <asp:Button runat="server" ID="btnCancel" OnClick="btnCancel_OnClick" Text="Cancel"  CausesValidation="False"/>
        <asp:Button runat="server" ID="btnDel" OnClientClick="return ConfirmDelete();" OnClick="btnDel_OnClick" Text="Delete"  CausesValidation="False"/>
        <asp:Button runat="server" ID="btnReset" Text="Reset password" OnClick="btnReset_OnClick"  CausesValidation="False"/>
    </fieldset>
    </div>
  
 </div>
            </section>

        </div>
    </section>

      </form>
</body>
</html>
