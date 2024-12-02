using Microsoft.Reporting.WebForms;
using ReportTest.Reports;
using System.Data;
using System.Web.Mvc;

namespace ReportNewTest.Controllers
{
    public class ReportController : Controller
    {
        public ActionResult GetFile()
        {
            // Step 1: Create LocalReport for the main report
            var mainReport = new LocalReport
            {
                ReportPath = Server.MapPath("~/Reports/MainReport.rdlc")            
            };

            // Step 2: Load data for the main report
            var mainData = GetMainReportData();
            mainReport.DataSources.Add(new ReportDataSource("MainDataSet", mainData));
            // Step 3: process sub report data 
            mainReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingHandler);

            // Step 4: Render the main report
            string mimeType = "application/pdf";
            string reportFormat = "PDF"; // Supported formats: "PDF", "Excel", "Word"
            byte[] reportBytes = mainReport.Render(
                reportFormat, null, out mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings);

            // Step 5: Return the rendered main report as a response
            return File(reportBytes, mimeType, "MainReport.pdf");
        }

        private void SubreportProcessingHandler(object sender, SubreportProcessingEventArgs e)
        {
            // Step 5: Get the ItemId parameter for filtering
            string itemId = e.Parameters["ItemId"].Values[0];

            // Step 6: Fetch data for the subreport filtered by ItemId
            DataTable subReportData = GetSubReportData(itemId);

            // Step 7: Add data to the subreport
            var subReportDataSource = new ReportDataSource("SubDataSet", subReportData);
            e.DataSources.Add(subReportDataSource);
        }

        private DataTable GetMainReportData()
        {
            MainDataSet dataSet = new MainDataSet();
            var dataTable = dataSet._MainDataSet;

            // Add rows
            var row1 = dataTable.NewMainDataSetRow();
            row1.ItemId = "1";
            row1.ItemName = "Sample1";
            row1.Total = "100";
            dataTable.AddMainDataSetRow(row1);

            var row2 = dataTable.NewMainDataSetRow();
            row2.ItemId = "2";
            row2.ItemName = "Sample2";
            row2.Total = "200";
            dataTable.AddMainDataSetRow(row2);

            var row3 = dataTable.NewMainDataSetRow();
            row3.ItemId = "3";
            row3.ItemName = "Sample3";
            row3.Total = "300";
            dataTable.AddMainDataSetRow(row3);            

            return dataTable;
        }

        // Fetch data for the subreport filtered by ItemId
        private DataTable GetSubReportData(string itemId)
        {
            DataTable table = new DataTable();
            table.Columns.Add("DetailId", typeof(int));
            table.Columns.Add("ItemId", typeof(int));
            table.Columns.Add("DetailDescription", typeof(string));
            table.Columns.Add("Quantity", typeof(int));
            table.Columns.Add("Price", typeof(decimal));

            int id = int.Parse(itemId);
            if (id == 1)
            {
                table.Rows.Add(1, 1, "Detail A", 2, 50.25m);
                table.Rows.Add(2, 1, "Detail B", 1, 50.25m);
            }
            else if (id == 2)
            {
                table.Rows.Add(3, 2, "Detail C", 4, 50.00m);
            }
            else if (id == 3)
            {
                table.Rows.Add(4, 3, "Detail D", 3, 100.00m);
            }

            return table;
        }
    }
}
