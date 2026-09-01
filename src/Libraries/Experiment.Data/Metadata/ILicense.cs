using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Enums;

namespace Experiment.Data.Metadata{
	public interface ILicense
	{
		/// <summary>
		/// PK
		/// </summary>
		int Id { get; set; }

		/// <summary>
		/// UserId for which issued the license
		/// </summary>
		string UserId { get; set; }

		/// <summary>
		/// OrderId
		/// </summary>
		Guid? OrderId { get; set; }

		/// <summary>
		/// License type/License data
		/// </summary>
		UserLicenseType Type { get; set; }

		/// <summary>
		/// License valid from date
		/// </summary>
		DateTime ValidFrom { get; set; }

		/// <summary>
		/// License valid to date
		/// </summary>
		DateTime ValidUntil { get; set; }

		/// <summary>
		/// License is active
		/// </summary>
		bool Active { get; set; }
	}
}
