using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace Website.Models
{
	// @TODO: Maybe to create ExperimentApiViewModels.cs and to move it there. As here should be stored ExperimentViewModels for ExperimentController, which is for Web.api MVC, not REST
	public class TreeItem
	{
		public string Id { get; set; }
		public string Parent { get; set; }
		public string Text { get; set; }
		public string Type { get; set; }
	}
}