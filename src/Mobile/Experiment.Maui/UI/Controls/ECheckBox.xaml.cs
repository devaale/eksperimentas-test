using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.UI.Controls{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ECheckBox : ContentView
	{
		/// <summary>
		/// Text
		/// </summary>
		public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ECheckBox), string.Empty, BindingMode.OneWay, propertyChanged: TextPropertyChanged);
		public string Text
		{
			get
			{
				return (string)GetValue(TextProperty);
			}

			set
			{
				SetValue(TextProperty, value);
			}
		}
		private static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{

			var control = (ECheckBox)bindable;
			if (control != null)
			{
				control.LblName.Text = (string)newValue;
			}
		}

		/// <summary>
		/// Text
		/// </summary>
		public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(ECheckBox), false, BindingMode.TwoWay, propertyChanged: IsCheckedPropertyChanged);
		public bool IsChecked
		{
			get
			{
				return (bool)GetValue(IsCheckedProperty);
			}

			set
			{
				SetValue(IsCheckedProperty, value);
			}
		}
		private static void IsCheckedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			// This updating Checkbox by this property state.
			// As they both have IsChecked properties, which are not the same, but should be synchronized.
			var control = (ECheckBox)bindable;
			if (control != null)
			{
				control.ChkMain.IsChecked = (bool)newValue;
			}
		}

		public ECheckBox ()
		{
			InitializeComponent ();
		}

		private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			ChkMain.IsChecked = !ChkMain.IsChecked;
		}

		private void ChkMain_CheckedChanged(object sender, CheckedChangedEventArgs e)
		{
			// This reflecting CheckBox state to control's IsChecked property.
			// As they both have IsChecked properties, which are not the same, but should be synchronized.
			IsChecked = ChkMain.IsChecked;
		}
	}
}
