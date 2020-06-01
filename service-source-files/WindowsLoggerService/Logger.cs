using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace WindowsLoggerService
{
    partial class Logger : ServiceBase
    {
        private Timer timerfortextlog = new Timer();
        private Timer timerformaillog = new Timer();
        private CultureInfo culture = new CultureInfo("tr-TR");

        public Logger()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string message = "Service çalıştı. -> " + SetCurrentTime();
            SendMail(message);
            WriteToFile(message);
            SetTimers(timerfortextlog, OnElapsedTimeText, 1);
            SetTimers(timerformaillog, OnElapsedTimeMail, 10);
        }
        protected override void OnStop()
        {
            string message = "Service durduruldu. -> " + SetCurrentTime();
            SendMail(message);
            WriteToFile(message);
        }
        private void OnElapsedTimeText(object source, ElapsedEventArgs e)
        {
            string message = "Service çalışıyor. -> " + SetCurrentTime();
            WriteToFile(message);
        }
        private void OnElapsedTimeMail(object source, ElapsedEventArgs e)
        {
            string message = "Service çalışıyor. -> " + SetCurrentTime();
            SendMail(message);
        }

        public void SetTimers(Timer timer, ElapsedEventHandler handler, byte minute)
        {
            timer.Elapsed += new ElapsedEventHandler(handler);
            timer.Interval = 1000 * 60 * minute;
            timer.Enabled = true;
        }

        public string SetCurrentTime()
        {
            return DateTime.Now.ToString("dd MMMM yyyy - HH:mm:ss", culture);
        }
        public void WriteToFile(string message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(message);
                }
            }
        }
        public void SendMail(string message)
        {
            string mailFromAddress = "<mailfrom>";
            string mailFromPassword = "<mailfrompass>";
            List<string> mailToList = new List<string>() { "<mailToList>" };
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mail.From = new MailAddress(mailFromAddress);
                foreach (var item in mailToList)
                {
                    mail.To.Add(new MailAddress(item));
                }
                mail.Subject = "Windows Logger Service";
                mail.IsBodyHtml = false; //to make message body as html  
                mail.Body = message;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(mailFromAddress, mailFromPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mail);
            }
            catch (Exception ex) { WriteToFile(ex.Message + " -> " + SetCurrentTime()); }
        }
    }
}

