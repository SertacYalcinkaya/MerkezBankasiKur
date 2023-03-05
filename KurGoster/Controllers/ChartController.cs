using Microsoft.AspNetCore.Mvc;
using MerkezBankasiApi;
using MerkezBankasiApi.Controllers;
using Microsoft.Office.Interop.Excel;

namespace KurGoster.Controllers
{
    public class ChartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult VisualizeResult()
        {
            return Json(KurList());
        }

        public ResponseData KurList()
        {
            KurController gunlukKurController = new KurController();
            ResponseData result = gunlukKurController.Run(new RequestData
            {
                BaslangicTarih = new DateTime(2023, 2, 3),
                BitisTarih = new DateTime(2023, 3, 5)
            });

            ExportToExcel(result.Data);

            return result;
        }

        public void ExportToExcel(List<ResponseDataKur> data)
        {
            var excelApp = new Application();
            var workbook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            var worksheet = (Worksheet)workbook.Worksheets[1];

            // Tablo başlıkları
            worksheet.Cells[1, 1] = "Tarih";
            worksheet.Cells[1, 2] = "Kodu";
            worksheet.Cells[1, 3] = "Adi";
            worksheet.Cells[1, 4] = "Birimi";
            worksheet.Cells[1, 5] = "AlisKuru";
            worksheet.Cells[1, 6] = "SatisKuru";
            worksheet.Cells[1, 7] = "EfektifAlisKuru";
            worksheet.Cells[1, 8] = "EfektifSatisKuru";

            // Tablo verileri
            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cells[row, 1] = item.Tarih;
                worksheet.Cells[row, 2] = item.Kodu;
                worksheet.Cells[row, 3] = item.Adi;
                worksheet.Cells[row, 4] = item.Birimi;
                worksheet.Cells[row, 5] = item.AlisKuru;
                worksheet.Cells[row, 6] = item.SatisKuru;
                worksheet.Cells[row, 7] = item.EfektifAlisKuru;
                worksheet.Cells[row, 8] = item.EfektifSatisKuru;
                row++;
            }

            // Excel dosyasını kaydet
            workbook.SaveAs("Data.xlsx");
            workbook.Close();
            excelApp.Quit();
        }
    }
}
