using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace IvaETicaret.Email
{
    public static class SenderEmail
    {
        public static void Gonder(string konu, string icerik, string GondMail)
        {
            MailMessage ePosta = new MailMessage();
            ePosta.From = new MailAddress("info@ivaheryerde.com.tr");
            //
            ePosta.To.Add(GondMail);
            //ePosta.To.Add("eposta2@gmail.com");
            //ePosta.To.Add("eposta3@gmail.com");
            //
            //ePosta.Attachments.Add(new Attachment(@"C:\deneme-upload.jpg"));
            ePosta.Subject = konu;
            ePosta.Body = icerik;
            ePosta.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.Host = "mail.ivaheryerde.com.tr";
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("info@ivaheryerde.com.tr", "Cib_17421");

            // object userState = ePosta;
            bool kontrol = true;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
                // smtp.SendAsync(ePosta, (object)ePosta);
                smtp.Send(ePosta);

            }
            catch (SmtpException ex)
            {
                kontrol = false;
            }

        }
    }
}
