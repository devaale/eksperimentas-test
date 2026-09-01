using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.BL.Data{
	/// <summary>
	/// Class representation of tblScanSessionStatus DB table
	/// </summary>
	public enum ScanSessionStatus : int
	{
		/// <summary>
		/// 0 – Įrašas sukurtas ir įrenginio apklausa pradėta;
		/// </summary>
		ScanStarted = 0,

		/// <summary>
		/// 10 – Įrašas jau turi duomenis data lauke ir užfiksuotą proceso pabaigos datą – yra paruoštas tolimesniam apdirbimui;
		/// </summary>
		ScanSuccessfulFinished = 10,

		/// <summary>
		/// 20 - Įrašas sėkmingai apdorotas ir iš jo binarinės informacijos sukurti normalūs šios informacijos reikšmių įrašai.
		/// </summary>
		ParsingFinished = 20,

        /// <summary>
		/// 30 - visos reikšmės iš šios sesijos yra patikrintos per alertų prizmę
		/// </summary>
        AlertsChecked = 30,

        /// <summary>
        /// 35 - visi potencialūs alertai iš šios sesijos yra išsiusti gavėjams
        /// </summary>
        AlertsNotified = 35,

		/// <summary>
		/// 40 – Įrašas pažymėtas archyvavimui;
		/// </summary>
		MarkedForArchiving = 40,

		/// <summary>
		/// 50 – Įrašas pažymėtas pašalinimui;
		/// </summary>
		MarkedForDeletion = 50,

		/// <summary>
		/// 60 - Įraše išsaugoti nepilni duomenys (pvz. nutrūko ryšys ir pan.);
		/// </summary>
		ErrorNotFullData = 60,

		/// <summary>
		/// 61 – Nepavyko susisiekti su valdikliu;
		/// </summary>
		ConnectionIssues = 61,

		/// <summary>
		/// Simple errorz
		/// </summary>
		Error = 62,

		/// <summary>
		/// 63 – Kritinė klaida;
		/// </summary>
		CriticalError = 63,
	}
}
