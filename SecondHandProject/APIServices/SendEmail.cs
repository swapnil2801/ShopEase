using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using MailKit.Net.Smtp;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SecondHandProject.APIServices
{
    public class SendEmail
    {
        public class MailModel
        {
            public string FromEmail { get; set; }
            public string EmailPassword { get; set; }
            public string ToEmail { get; set; }
            public string CC { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public bool IsBodyHtml { get; set; }
        }
        public async Task<string> VerifyEmail(string Email, int Code)
        {
            try
            {
                MailModel model = new MailModel();
                model.FromEmail = "9284037084s@gmail.com";
                model.EmailPassword = "khin ppgt eioo pgzd";
                model.ToEmail = Email;
                model.Subject = "Verification Email from SecondHand Application";
                model.Body = @"<div style = 'text-align:center';>
                                    <h1>Welcome to second Hand Application</h1>
                                    <h3>Your Email verification code is : </h3> " + Code.ToString() + "</div>";
                model.IsBodyHtml = true;
                model.CC = "";

                /*using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(model.FromEmail);
                    mail.To.Add(model.ToEmail);
                    mail.CC.Add(model.CC);
                    mail.Subject = model.Subject;
                    mail.Body = model.Body;
                    mail.IsBodyHtml = true;

                    using(SmtpClient smtp = new SmtpClient("smtp.gmail.com",587))
                    {
                        smtp.Credentials = new NetworkCredential(model.FromEmail, model.EmailPassword);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                        return ("Email has been sent Successfully");
                    }
                }*/

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(model.FromEmail));
                email.To.Add(MailboxAddress.Parse(model.ToEmail));
                email.Subject = model.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = model.Body };

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(model.FromEmail, model.EmailPassword);
                smtp.Send(email);
                smtp.Disconnect(true);
                return ("Email has been sent Successfully");
            }
            catch(Exception ex)
            {
                return (ex.Message);
            }
        }
    }
}
