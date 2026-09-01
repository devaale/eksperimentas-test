using System;
using System.Diagnostics;
using System.Text;

using Experiment.Core.Metadata;

namespace Experiment.Core.Text{
	public class Base64 : IDecoder
	{
		static readonly Encoding DefaultEncoding = Encoding.UTF8;

		static Base64 _Instance;
		public static Base64 Instance
		{
			get
			{
				if(_Instance == null)
					_Instance = new Base64();

				return _Instance;
			}
		}

		/// <summary>
		/// Encode
		/// </summary>
		/// <param name="plainText"></param>
		/// <returns></returns>
		public string Encode(string plainText, Encoding enc)
		{
			if (enc == null)
				throw new ArgumentNullException(string.Format("{0} parameter must be not NULL", nameof(enc)));

			var plainTextBytes = enc.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

		public string Encode(string plainText)
		{
			return Encode(plainText, DefaultEncoding);
		}

		/// <summary>
		/// Decode
		/// </summary>
		/// <param name="base64EncodedData"></param>
		/// <returns></returns>
		public string Decode(string base64EncodedData, Encoding enc)
		{
			if (enc == null)
				throw new ArgumentNullException(string.Format("{0} parameter must be not NULL", nameof(enc)));

			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
			return enc.GetString(base64EncodedBytes);
		}

		public string Decode(string base64EncodedData)
		{
			return Decode(base64EncodedData, DefaultEncoding);
		}

	}
}
