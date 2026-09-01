using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Website.Models
{
	[Table("tblUiWord")]
	public class Word
	{
		[Key, Column(Order = 0, TypeName = "varchar")]
		[StringLength(64)]
		public string Alias { get; set; }

		[Key, Column(Order = 1, TypeName = "varchar")]
		[StringLength(4)]
		public string Code { get; set; }

		[ForeignKey(nameof(Code))]
		public Language Language { get; set; }

		[Column(TypeName = "nvarchar(max)")]
		public string Text { get; set; }

		public bool Autoadded { get; set; }
	}
}