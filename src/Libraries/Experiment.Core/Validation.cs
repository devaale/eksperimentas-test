using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Core{
	public class Validation
	{

		/// <summary>
		/// Validate do object is not null
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="parameterName"></param>
		public static void RequireValid<T>(T parameter, string parameterName)
		{
			if (parameter == null)
			{
				throw new Exception(string.Format(
					"Object '{0}' is not valid!",
					parameterName));
			}
		}


		public static void RequireValidString(string parameter, string parameterName)
		{
			if (string.IsNullOrEmpty(parameter))
			{
				throw new Exception(string.Format(
					"String '{0}' is not valid!",
					parameterName));
			}
		}

	}
}
