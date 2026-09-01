using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core{
	public class UrlMan
	{
		string _Url;
		string _Host;
		int? _Port;

		public string Url
		{
			get => _Url;
			set
			{
				_Url = value;

				string[] tokens = _Url.Split(':');

				if (tokens.Length > 0)
				{
					Host = tokens[0];
				}

				if (tokens.Length > 1)
				{
					int port = 0;

					if (int.TryParse(tokens[1], out port))
					{
						Port = port;
					}
					else
					{
						Port = DefaultPort;
					}
				}
			}
		}

		public string Host { get; protected set; }
		public int? Port { get; protected set; } = null;
		public int? DefaultPort { get; protected set; } = null;

		public UrlMan(string url)
		{
			Url = url;
		}

		public UrlMan(int defaultPort, string url)
		{
			DefaultPort = defaultPort;
			Url = url;
		}
	}
}
