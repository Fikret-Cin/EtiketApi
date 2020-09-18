using iTextSharp.text.pdf;
using System.IO;
using System.Web.Http;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using iTextSharp.text.pdf.qrcode;

namespace EtiketApi.Controllers
{
    public class EtiketController : ApiController
    {
        public HttpResponseMessage Post([FromBody] YaziciData yaziciData)
        {
            try
            {
                var str = JsonConvert.SerializeObject(yaziciData);
                string config = @"Server=OOZBEK;Database=EtiketApi;Trusted_Connection = True;";
                SqlConnection con = new SqlConnection(config);
                string query = @"INSERT INTO Etiket (Etiket) VALUES ('" + str + "')";
                SqlCommand cmd = new SqlCommand(query, con);
                if (con.State == System.Data.ConnectionState.Closed)
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (System.Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            //Etiket Template Yolu
            string uygulamaDizini = System.AppDomain.CurrentDomain.BaseDirectory;
            FileInfo etiketTipi = new FileInfo(@uygulamaDizini + "Templates\\" + yaziciData.Etiket + ".pdf");

            //Template Doldurma            
            FileInfo temp = new FileInfo(@uygulamaDizini + "Templates\\temp.pdf");
            PdfReader pdfReader = new PdfReader(etiketTipi.FullName);
            FileStream stream = new FileStream(temp.FullName,FileMode.Create);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, stream);
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            
            foreach (var item in yaziciData.EtiketData)
            {
                if (item.Key != null && item.Key != "qrCode")
                {
                    pdfFormFields.SetField(item.Key.ToString(), item.Value.ToString());
                }
                else if (item.Key != null && item.Key == "qrCode" )
                {
                    BarcodeQRCode testqr = new BarcodeQRCode(item.Value, 50, 50, null); ;
                    iTextSharp.text.Image img = testqr.GetImage();
                    img.SetAbsolutePosition(10,20);
                    pdfStamper.GetOverContent(1).AddImage (img);
                }
            }
            pdfStamper.FormFlattening = false;
            pdfStamper.Close();

            //Yazdırma İşlemi
            Spire.Pdf.PdfDocument yazdirilacakDosya = new Spire.Pdf.PdfDocument();
            yazdirilacakDosya.LoadFromFile(temp.FullName);
            yazdirilacakDosya.PrintSettings.PrinterName = yaziciData.YaziciAdi;
            yazdirilacakDosya.PrintSettings.Copies = (short)yaziciData.EtiketAdedi;
            yazdirilacakDosya.Print();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
