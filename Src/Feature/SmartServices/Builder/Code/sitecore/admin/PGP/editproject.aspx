<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editproject.aspx.cs"
     Inherits="DEWAXP.Feature.Builder.sitecore.admin.PGP.EditProject" %>

<%@ Import Namespace="DEWAXP.Feature.Builder.Models.ProjectGeneration" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Admin console</title>
    <link rel="shortcut icon" href="/sitecore/images/favicon.ico" />
    <link href="/sitecore/shell/client/Speak/Assets/css/speak-default-theme.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function ConfirmDelete() {
            if (confirm('Are you sure you wish to delete this project?'))
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
                        Project adminstration
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
    <fieldset>
        <asp:ValidationSummary runat="server" ForeColor="red" DisplayMode="BulletList"
            HeaderText="Following error occurs....."
             ShowSummary="true" />
        <table cellspacing="0" rules="all" border="1" class="sc-table sc-table-header table">
            <tr>
                <td width="35%">
                    <label for="form-field-project" class="form-field__label">
                        Project Name  *
                       
                    </label>
                    </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtProject" MaxLength="100"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtProject"
                            ErrorMessage='Project name is required' Text="*"></asp:RequiredFieldValidator>
                    </span>
                    </td>
            </tr>
            <tr>
                <td>
                    <label for="form-field-project" class="form-field__label">
                        Contract number  *
                       
                    </label>
                     </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtContract" MaxLength="50"></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtContract"
                            ErrorMessage='Contract number is required' Text="*"></asp:RequiredFieldValidator>
                    </span>
                    </td>
            </tr>
                    <tr>
                <td>
                    <label for="form-field-project" class="form-field__label">
                        DMS Folder  *
                       
                    </label>
                     </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtDMSFolder" MaxLength="100"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDMSFolder"
                            ErrorMessage='DMS folder is required' Text="*"></asp:RequiredFieldValidator>
                    </span>
                    </td>
            </tr>
                    <tr>
                <td>
                    <label for="form-field-project" class="form-field__label">
                        ACL Name  *
                       
                    </label>
                     </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtAclName" MaxLength="50"></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAclName"
                            ErrorMessage='ACL name is required' Text="*"></asp:RequiredFieldValidator>
                    </span>
                    </td>
            </tr>
                    <tr>
                <td>
                    <label for="form-field-project" class="form-field__label">
                        ACL Domain  *
                       
                    </label>
                     </td>
                <td>
                    <span class="form-field__input-wrapper form-field__input-wrapper--select">
                       <asp:TextBox runat="server" ID="txtAclDomain" MaxLength="50"></asp:TextBox>
                         <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAclDomain"
                            ErrorMessage='ACL domain is required' Text="*"></asp:RequiredFieldValidator>
                    </span>
                    </td>
            </tr>
            </table>
        
        <asp:Button runat="server" ID="btnSave" OnClick="btnSave_OnClick" Text="Save"/>
        <asp:Button runat="server" ID="btnCancel" OnClick="btnCancel_OnClick" Text="Cancel"  CausesValidation="False"/>
        <asp:Button runat="server" ID="btnDel" OnClientClick="return ConfirmDelete();" OnClick="btnDel_OnClick" Text="Delete"  CausesValidation="False"/>
    </fieldset>
    </div>
    
 </div>
            </section>

        </div>
    </section>

    </form>
</body>
</html>
