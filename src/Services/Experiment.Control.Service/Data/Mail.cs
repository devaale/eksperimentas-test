using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Core.BL.Data;
using Experiment.Core.Metadata;

using Experiment.Control.Service.Config;

namespace Experiment.Control.Service.Data{
    public class Mail
    {
        /// <summary>
        /// Windows environment system variable. 
        /// For proper this system work it should be set correctly, elsewhere we shutting down all this software.
        /// </summary>
        public const string ENV_EXP_HOME = "EXP_HOME";

        public const string DEFAULT_MAIL_CONFIG_PATH = @"\src\config\config-mail.json";

        public const string Host = "host";
        public const string Username = "username";
        public const string Password = "password";

        private const string DEBUG_TYPE = "SendMail";
        public const string wLocation = DEBUG_TYPE + "::Mail";

        static ILogger _Logger;

        public Mail(ILogger logger)
        {
            _Logger = logger;
        }
        
        protected static string _ExpHome = string.Empty;
        public static string ExpHome
        {
            get
            {
                if (string.IsNullOrEmpty(_ExpHome))
                {
                    _ExpHome = Environment.GetEnvironmentVariable(ENV_EXP_HOME);
                }
                return _ExpHome;
            }
        }

        public static SendMailState Send(Dictionary<string, string> mailSettings, DataRow row)
		{
			SendMailState retVal = SendMailState.None;
            
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                message.From = new MailAddress(row[Defaults.DB_FROM].ToString());
                message.To.Add(new MailAddress(row[Defaults.DB_TO].ToString()));
                message.Subject = row[Defaults.DB_SUBJECT].ToString();
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = row[Defaults.DB_BODY].ToString();

                smtp.Port = 587;
                smtp.Host = mailSettings[Host];
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(mailSettings[Username], mailSettings[Password]);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.Send(message);

                retVal = SendMailState.Sent;
            }
            catch (Exception ex)
            {
                retVal = SendMailState.Error;

                Console.WriteLine("Exception caught in Mail.Send(): {0}",
                    ex.ToString());
            }

            return retVal;
		}

        public static Dictionary<string, string> _MailSettings = new Dictionary<string, string>();
        public Dictionary<string, string> MailSettings()
        {
            if (!_MailSettings.Any())
            {
                InitSettings();
            }

            if (!_MailSettings.Any())
            {
                LoadDefaultMailSettings();

                var vStep = string.Format("{0}, Default mail settings loaded", wLocation);
                _Logger.WriteLine(5, vStep);
            }
            return _MailSettings;
        }

        static void InitSettings()
        {
            if (string.IsNullOrEmpty(ExpHome))
                throw new Exception("EXP_HOME was not found!");

            var fullMailConfigPath = ExpHome + DEFAULT_MAIL_CONFIG_PATH;
            LoadAndParseSettings(fullMailConfigPath);
        }

        static void LoadDefaultMailSettings()
        {
            _MailSettings.Add(Host, "smtp.office365.com");
            _MailSettings.Add(Username, "ese.alerts@energus.eu");
            _MailSettings.Add(Password, "Energus2021");
        }

        static void LoadAndParseSettings(string path)
        {
            try
            {
                using (StreamReader file = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    ConfigObject o = (ConfigObject)serializer.Deserialize(file, typeof(ConfigObject));

                    _MailSettings.Add(Host, o.Settings.Host);
                    _MailSettings.Add(Username, o.Settings.Username);
                    _MailSettings.Add(Password, o.Settings.Password);

                    file.Close();
                }

                var vStep = string.Format("{0}, Mail settings loaded from config file: {1}", wLocation, DEFAULT_MAIL_CONFIG_PATH);
                _Logger.WriteLine(5, vStep);
            }
            catch (Exception ex)
            {
                var vStep = string.Format("{0}, Mail config file problem", wLocation);
                _Logger.WriteLine(5, vStep);
            }
        }
    }
}
