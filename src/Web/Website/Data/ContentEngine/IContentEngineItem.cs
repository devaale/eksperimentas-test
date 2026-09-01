using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.Data.ContentEngine
{
	public interface IContentEngineItem
	{
		string Label { get; set; }
		string Type { get; set; }
	}
}
