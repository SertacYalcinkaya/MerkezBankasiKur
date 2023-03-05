using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace MerkezBankasiKur.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GunlukKurController : ControllerBase
    {
        [HttpPost]
        public ResponseData Run(RequestData requestData)
        {
            ResponseData result = new();

            try
            {
                string path = string.Format("http://www.tcmb.gov.tr/kurlar/{0}.xml",
                (requestData.IsBugun) ? "today" : string.Format("{2}{1}/{0}{1}{2}"),
                requestData.Gun.ToString().PadLeft(2, '0'), requestData.Ay.ToString().PadLeft(2, '0'), requestData.Yil
                );

                result.Data = new List<ResponseDataKur>();

                XmlDocument xml = new();
                xml.Load(path);

                if (xml.SelectNodes("Tarih_Date").Count < 1)
                {
                    result.Hata = "Bilgi bulunamadı.";

                    return result;
                }

                foreach (XmlNode node in xml.SelectNodes("Tarih_Date")[0].ChildNodes)
                {
                    ResponseDataKur responseDataKur = new()
                    {
                        Kodu = node.Attributes["Kod"].Value,
                        Adi = node["Isim"].InnerText,
                        Birimi = Convert.ToInt32(node["Unit"].InnerText),
                        AlisKuru = Convert.ToDecimal("0" + node["ForexBuying"].InnerText.Replace(".", ",")),
                        SatisKuru = Convert.ToDecimal("0" + node["ForexSelling"].InnerText.Replace(".", ",")),
                        EfektifAlisKuru = Convert.ToDecimal("0" + node["BanknoteBuying"].InnerText.Replace(".", ",")),
                        EfektifSatisKuru = Convert.ToDecimal("0" + node["BanknoteSelling"].InnerText.Replace(".", ","))
                    };

                    result.Data.Add(responseDataKur);
                }
            }
            catch (Exception ex)
            {
                result.Hata = ex.Message;
            }

            



            return result;
        }
    }
}
