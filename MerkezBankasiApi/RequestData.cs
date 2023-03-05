namespace MerkezBankasiApi
{
    public class RequestData
    {
        public DateTime BaslangicTarih { get; set; }
        public DateTime BitisTarih { get; set; }
    }

    public class ResponseDataKur
    {
        public DateTime Tarih { get; set; }
        public string Kodu { get; set; }
        public string Adi { get; set; }
        public int Birimi { get; set; }
        public decimal AlisKuru { get; set; }
        public decimal SatisKuru { get; set; }
        public decimal EfektifAlisKuru { get; set; }
        public decimal EfektifSatisKuru { get; set; }
    }

    public class ResponseData
    {
        public List<ResponseDataKur> Data { get; set; }
        public string Hata { get; set; }
    }
}
