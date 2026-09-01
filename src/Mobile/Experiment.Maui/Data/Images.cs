using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Controls;

namespace Experiment.Maui.Data{
    internal static class Images
    {
        /// <summary>
        /// Images folder or what to add to Settings.Server URL to get images folder
        /// </summary>
        const string IMAGES_FOLDER = "Content/img/ui/";

        internal static string UrlImagesFolder { get => Settings.Server + IMAGES_FOLDER; }
        internal static string UrlMinusImg { get => UrlImagesFolder + "1.png"; }
		internal static string UrlPlusImg { get => UrlImagesFolder + "2.png"; }
		internal static string UrlCrossImg { get => UrlImagesFolder + "4.png"; }
		internal static string UrlMultipointImg { get => UrlImagesFolder + "7.png"; }
		internal static string UrlLicensesImg { get => UrlImagesFolder + "10.png"; }
		internal static string UrlGiveTokenImg { get => UrlImagesFolder + "9.png"; }

		internal static string UrlSettingsImg { get => UrlImagesFolder + "5.png"; }
		internal static string UrlZoomImg { get => UrlImagesFolder + "6.png"; }
		internal static string UrlLikeImg { get => UrlImagesFolder + "26.png"; }
		internal static string UrlVisibilityImg { get => UrlImagesFolder + "visibility.svg"; }
		internal static string UrlVisibilityOffImg { get => UrlImagesFolder + "visibility_off.svg"; }

		// Main menu?
		internal static string UrlDevices { get => UrlImagesFolder + "5.png"; }
		internal static string UrlEcosystem { get => UrlImagesFolder + "13.png"; }
		internal static string UrlChat { get => UrlImagesFolder + "12.png"; }
		internal static string UrlAlarms { get => UrlImagesFolder + "11.png"; }
		internal static string UrlGraphs { get => UrlImagesFolder + "6.png"; }
		internal static string UrlControl { get => UrlImagesFolder + "14.png"; }
		internal static string UrlDeterioration { get => UrlImagesFolder + "1.png"; }
		internal static string UrlEngineering { get => UrlMultipointImg; }
		internal static string UrlWallet { get => UrlImagesFolder + "9.png"; }
		internal static string UrlTest { get => UrlImagesFolder + "3.png"; }
		internal static string UrlSettings { get => UrlImagesFolder + "5.png"; }
		internal static string UrlLogoff { get => UrlImagesFolder + "11.png"; }
	}
}

