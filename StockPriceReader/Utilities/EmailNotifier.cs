using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockPrice
{
    public class EmailNotifier
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public EmailNotifier(string username, string password)
        {
            Username = username;
            Password = password;
        }

        private void Send(MailMessage msg)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(Username, Password);
            bool success = false;
            while (!success)
            {
                try
                {
                    client.Send(msg);
                    success = true;
                }
                catch (Exception e)
                {
                    success = false;
                    Console.WriteLine(e.Message);
                    Thread.Sleep(10000);
                    EmailNotifier en = new EmailNotifier("ff12sender", "33914047");

                    en.Send(msg);

                } 
            }
        }

        public void Send(string subject, string message)//, string attachmentStr)
        {
            MailMessage msg = new MailMessage("ff12sender@gmail.com", "fferreira12@gmail.com");
            msg.Subject = subject;
            msg.Body = message;
            //Attachment at = new Attachment(attachmentStr);
            //msg.Attachments.Add(at);
            Send(msg);
        }

        public void Send(List<Stock> rankedStocks)
        {
            StringBuilder sb = new StringBuilder();

            int i = 1;

            sb.Append("i.  " + "\t\t" + "Code" + "\t\t" + "Points" + "\n");

            foreach (Stock s in rankedStocks)
            {
                sb.Append(i.ToString("000") + ".\t\t" + s.stockCode.PadRight(10, ' ') + "\t\t" + s.indicators.Punctuation.ToString("0.###") + "\n");
                i++;
            }

            string subject = "StockMarket Analysis - " + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString();

            Send(subject, sb.ToString());
        }
    }
}
