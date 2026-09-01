using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	/// <summary>
	/// Universal named Id item, which has int Id and string Name.
	/// Can be used anywhere.
	/// </summary>
	public class NamedDbItem<T>: INamedDbItem<T>
	{
		public T Id { get; set; }
		public string Name { get; set; }
	}
}
