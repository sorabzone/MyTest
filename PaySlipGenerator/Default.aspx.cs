using CommonLoggers;
using OfficeOpenXml;
using PaySlipGenerator.Helper;
using System;
using System.IO;

namespace PaySlipGenerator
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ErroMsg.Text = "";
        }

        /// <summary>
        /// This event handles the generation of payslips
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                ErroMsg.Text = "";
                string filecontent = Convert.ToBase64String(uploadFile.FileBytes);

                if (Path.GetExtension(uploadFile.FileName).Equals(".xlsx"))
                {
                    var excel = new ExcelPackage(uploadFile.FileContent);
                    var paySlips = PaySlipWorker.GeneratePaySlipsExcel(excel, ddlState.SelectedValue);

                    string excelName = "PaySlips";
                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                        paySlips.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                    }
                }
                else
                    ErroMsg.Text = "Please select valid excel file.";
            }
            catch (AggregateException ex)
            {
                foreach(var exp in ex.InnerExceptions)
                {
                    CommonLogger.LogError(exp.InnerException != null ? exp.InnerException : exp);
                    ErroMsg.Text = exp.InnerException != null ? exp.InnerException.Message : exp.Message;
                }
                CommonLogger.LogError(ex);
                ErroMsg.Text = ErroMsg.Text + "\n" + ex.Message;
            }
            catch (Exception ex)
            {
                CommonLogger.LogError(ex);
                ErroMsg.Text = ex.Message;
            }
            finally
            {
                uploadFile.Dispose();

                if(Response.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    Response.End();
            }
        }
    }
}