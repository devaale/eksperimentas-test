using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Website.Models
{
	[Table("tblUiLanguage")]
	public class Language
	{
		[Key]
		[Column(TypeName = "varchar")]
		[StringLength(4)]
		public string Code { get; set; }

		[Column(TypeName = "nvarchar")]
		[StringLength(256)]
		public string Name { get; set; }
	}
}