using Experiment.Maui.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Experiment.Maui.UI.Controls{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DashboardButton : ContentView
	{
		/// <summary>
		/// Button Text
		/// </summary>
		public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(DashboardButton), string.Empty, BindingMode.OneWay, propertyChanged: TextPropertyChanged);
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

			var control = (DashboardButton)bindable;
			if (control != null)
			{
				control.LblName.Text = (string)newValue;
			}
		}

		/// <summary>
		/// Button ImageSource
		/// </summary>
		public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(DashboardButton), null, BindingMode.OneWay, propertyChanged: ImageSourcePropertyChanged);
		public ImageSource ImageSource
		{
			get
			{
				return (ImageSource)GetValue(ImageSourceProperty);
			}

			set
			{
				SetValue(ImageSourceProperty, value);
			}
		}
		private static void ImageSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{

			var control = (DashboardButton)bindable;
			if (control != null)
			{
				control.ImgIcon.IsVisible = newValue != null;
				control.ImgIconOnly.IsVisible = newValue != null && control.IconOnly;
				control.ImgIcon.Source = (ImageSource)newValue;
				control.ImgIconOnly.Source = (ImageSource)newValue;
			}
		}

		/// <summary>
		/// Button Command
		/// </summary>
		public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(Command), typeof(DashboardButton));
		public Command Command
		{
			get
			{
				return (Command)GetValue(CommandProperty);
			}

			set
			{
				SetValue(CommandProperty, value);
			}
		}

		/// <summary>
		/// Button CommandParameter
		/// </summary>
		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(DashboardButton));
		public object CommandParameter
		{
			get
			{
				return (object)GetValue(CommandParameterProperty);
			}

			set
			{
				SetValue(CommandParameterProperty, value);
			}
		}

		/// <summary>
		/// Shows only the icon, centered.
		/// </summary>
		public static readonly BindableProperty IconOnlyProperty = BindableProperty.Create(nameof(IconOnly), typeof(bool), typeof(DashboardButton), false, BindingMode.OneWay, propertyChanged: IconOnlyPropertyChanged);
		public bool IconOnly
		{
			get => (bool)GetValue(IconOnlyProperty);
			set => SetValue(IconOnlyProperty, value);
		}
		private static void IconOnlyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var control = (DashboardButton)bindable;
			if (control != null)
			{
				var iconOnly = newValue is bool b && b;
				control.NormalLayout.IsVisible = !iconOnly;
				control.IconOnlyLayout.IsVisible = iconOnly;
				control.LblName.IsVisible = !iconOnly;
				control.ImgIconOnly.IsVisible = iconOnly && control.ImageSource != null;
			}
		}

		/// <summary>
		/// Makes the whole button see-through when true.
		/// </summary>
		public static readonly BindableProperty IsTransparentProperty = BindableProperty.Create(nameof(IsTransparent), typeof(bool), typeof(DashboardButton), false, BindingMode.OneWay, propertyChanged: IsTransparentPropertyChanged);
		public bool IsTransparent
		{
			get => (bool)GetValue(IsTransparentProperty);
			set => SetValue(IsTransparentProperty, value);
		}
		private static void IsTransparentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var control = (DashboardButton)bindable;
			if (control?.FrmRoot != null)
			{
				control.FrmRoot.Opacity = (newValue is bool b && b) ? 0.45 : 1.0;
			}
		}

		public DashboardButton ()
		{
			InitializeComponent ();
			FrmRoot.Opacity = 1.0;
		}

		private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			if (Command != null)
			{
				Command.Execute(CommandParameter);
			}
		}

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName == nameof(IsEnabled))
			{
				ImgIcon.IsEnabled = IsEnabled;
				LblName.IsEnabled = IsEnabled;
			}
		}

	}
}
