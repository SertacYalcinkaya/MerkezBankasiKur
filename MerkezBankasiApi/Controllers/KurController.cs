using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Xml;

namespace MerkezBankasiApi.Controllers
{
    public class KurController : ControllerBase
    {
        [HttpPost]
        public ResponseData Run(RequestData requestData)
        {
            ResponseData result = new();

            try
            {
                int fark = (int)(requestData.BitisTarih - requestData.BaslangicTarih).TotalDays;

                result.Data = new List<ResponseDataKur>();

                for (int i = 0; i < fark; i++)
                {
                    DateTime tempDate = requestData.BaslangicTarih.AddDays(i);
                    if (tempDate.DayOfWeek == DayOfWeek.Sunday || tempDate.DayOfWeek == DayOfWeek.Saturday)
                        continue;

                    string path = string.Format("http://www.tcmb.gov.tr/kurlar/{0}.xml",
                        string.Format("{2}{1}/{0}{1}{2}",
                        tempDate.Day.ToString().PadLeft(2, '0'), tempDate.Month.ToString().PadLeft(2, '0'), tempDate.Year
                        ));

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
                    request.Timeout = 30000; // Timeout süresi 30 saniye
                    XmlDocument xml = new();
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            xml.Load(dataStream);
                        }
                    }

                    if (xml.SelectNodes("Tarih_Date").Count < 1)
                    {
                        result.Hata = "Bilgi bulunamadı.";

                        return result;
                    }

                    foreach (XmlNode node in xml.SelectNodes("Tarih_Date")[0].ChildNodes)
                    {
                        if (node.Attributes["Kod"].Value == "USD")
                        {
                            ResponseDataKur responseDataKur = new()
                            {
                                Tarih = tempDate.Date,
                                Kodu = node.Attributes["Kod"].Value,
                                Adi = node["Isim"].InnerText,
                                Birimi = Convert.ToInt32(node["Unit"].InnerText),
                                AlisKuru = Convert.ToDecimal("0" + node["ForexBuying"].InnerText),
                                SatisKuru = Convert.ToDecimal("0" + node["ForexSelling"].InnerText),
                                EfektifAlisKuru = Convert.ToDecimal("0" + node["BanknoteBuying"].InnerText),
                                EfektifSatisKuru = Convert.ToDecimal("0" + node["BanknoteSelling"].InnerText)
                            };

                            result.Data.Add(responseDataKur);
                        }
                    }
                }

                result = GetMaxFiveDays(result);
            }
            catch (Exception ex)
            {
                result.Hata = ex.Message;
            }

            return result;
        }

        private ResponseData GetMaxFiveDays(ResponseData responseData)
        {
            ResponseData result = new();

            result.Data = responseData.Data.OrderByDescending(o => o.AlisKuru).Take(5).ToList();

            return result;
        }
    }
}
