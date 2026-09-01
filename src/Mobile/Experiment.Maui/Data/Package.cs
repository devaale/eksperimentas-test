using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Maui.Data{
	/// <summary>
	/// @TODO: Still used anywhere? This class probably was created at the beginning of the project to handle some webservice requests, but barely used today. Need to check.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Package<T>
	{
		public T Subject { get; protected set; }
		public bool Result;
		public Package(T subject)
		{
			Subject = subject;
			Result = false;
		}
	}
}
