<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PaySlipGenerator._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="box-shadow bg-white">
        <table width="90%" style="margin-left: 25px; margin-right: 25px; margin-bottom: 25px;" cellpadding="2px;">
            <br />
            <p style="margin-left: 25px;">
                <asp:Label ID="ErroMsg" ForeColor="red" runat="server" />
            </p>
            <tr>
                <td colspan="3">
                    <span class="h2">
                        <label>Generate PaySlips</label></span>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <hr class="mb-4">
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <label>State:</label>
                    <asp:DropDownList runat="server" ID="ddlState">
                        <asp:ListItem Selectked="True" Text="NSW" Value="NSW"></asp:ListItem>
                        <asp:ListItem Text="Victoria" Value="Victoria"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <label></label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:FileUpload ID="uploadFile" runat="server" /></td>
                <td></td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:Button Text="Generate File" ID="btnGenerate" runat="server" OnClick="btnGenerate_Click" class="btn btn-primary btn-block" />
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <br />
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
