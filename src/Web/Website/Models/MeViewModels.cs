using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Website.Models
{
	// Models returned by MeController actions.
	public class GetViewModel
	{
		[StringLength(3)]
		public string Language { get; set; }
	}
}