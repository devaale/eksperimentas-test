using System;
using System.Collections.Concurrent;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

namespace Website.Custom.Implementation
{
	/// <summary>
	/// Class taken from https://stackoverflow.com/a/45789677
	/// </summary>
	public class SmtpMailService : IIdentityMessageService
	{
		readonly ConcurrentQueue<SmtpClient> _clients = new ConcurrentQueue<SmtpClient>();

		public async Task SendAsync(IdentityMessage message)
		{
			var client = GetOrCreateSmtpClient();
			Exception exp = null;

			try
			{
				MailMessage mailMessage = new MailMessage();

				mailMessage.To.Add(new MailAddress(message.Destination));
				mailMessage.Subject = message.Subject;
				mailMessage.Body = message.Body;

				mailMessage.BodyEncoding = Encoding.UTF8;
				mailMessage.SubjectEncoding = Encoding.UTF8;
				mailMessage.IsBodyHtml = true;

				// there can only ever be one-1 concurrent call to SendMailAsync
				await client.SendMailAsync(mailMessage);
			}
			catch(Exception ex)
			{
				exp = ex;
			}
			finally
			{
				_clients.Enqueue(client);
			}

			if(exp != null)
			{
				throw exp;
			}
		}

		private SmtpClient GetOrCreateSmtpClient()
		{
			SmtpClient client = null;
			if (_clients.TryDequeue(out client))
			{
				return client;
			}

			client = new SmtpClient();
			return client;
		}
	}
}