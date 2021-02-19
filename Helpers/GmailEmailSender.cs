using System;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

public class GmailEmailSender  
   {  
       private readonly GmailEmailSettings settings;  
  
       public GmailEmailSender(GmailEmailSettings settings)  
       {  
           this.settings = settings;  
       }  
  
       public async Task<ResponseMessage> SendAsync(EmailMessage message)  
       {  
           // Message  
            var msg = new MailMessage();  
            msg.Subject = message.Subject;  
            msg.From = new MailAddress(message.From, "User");  

            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.Body, null, MediaTypeNames.Text.Html));
            foreach (var to in message.To)
            {
                msg.To.Add(new MailAddress(to));
            }
            

            //    if (message.CC.Count > 0)  
            //        msg.AddCcs(message.CC.Select(s => new EmailAddress(s)).ToList());  

            //    if (message.BCC.Count > 0)  
            //        msg.AddBccs(message.BCC.Select(s => new EmailAddress(s)).ToList());  

            //    if (message.Attachments.Count > 0)  
            //        msg.AddAttachments(message.Attachments.Select(s => new Attachment  
            //        {  
            //            Filename = s,  
            //            Content = Convert.ToBase64String(System.IO.File.ReadAllBytes(s))  
            //        }).ToList());  

            // Send  
            SmtpClient smtpClient = new SmtpClient(settings.Server, Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(settings.Username, settings.Password);
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            await smtpClient.SendMailAsync(msg);

            // Return  
            return new ResponseMessage("Ok");  
       }  
   }