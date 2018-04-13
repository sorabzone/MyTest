using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace PaySlipGenerator
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ErroMsg.Text = "";
        }

        protected void btnTranslate_Click(object sender, EventArgs e)
        {
            try
            {
                ErroMsg.Text = "";
                string filecontent = Convert.ToBase64String(uploadFile.FileBytes);

                if (Path.GetExtension(uploadFile.FileName).Equals(".xlsx"))
                {
                    var excel = new ExcelPackage(uploadFile.FileContent);
                    var dt = excel.ToDataTable();

                    string excelName = "PaySlips";
                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                        dt.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
                else
                    ErroMsg.Text = "Please select valid excel file.";
            }
            catch (Exception ex)
            {
                ErroMsg.Text = ex.Message + "\n Stack Trace: " + ex.StackTrace;
                throw;
            }
            finally
            {
            }
        }
    }
}