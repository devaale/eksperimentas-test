using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

namespace Experiment.Maui.UI.Controls{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PictureLeftButton : ContentView
	{
		#region Const
		const int DefaultPictureSize = 40;

		#endregion

		#region Properties

		/// <summary>
		/// Button Text
		/// </summary>
		public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(PictureLeftButton), string.Empty, BindingMode.OneWay, propertyChanged: TextPropertyChanged);
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

			var control = (PictureLeftButton)bindable;
			if (control != null)
			{
				control.LblName.Text = (string)newValue;
			}
		}

		/// <summary>
		/// Button ImageSource
		/// </summary>
		public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(PictureLeftButton), null, BindingMode.OneWay, propertyChanged: ImageSourcePropertyChanged);
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

			var control = (PictureLeftButton)bindable;
			if (control != null)
			{
				control.ImgIcon.Source = (ImageSource)newValue;
			}
		}

		/// <summary>
		/// Button ImageSize
		/// </summary>
		public static readonly BindableProperty ImageSizeProperty = BindableProperty.Create(nameof(ImageSize), typeof(int), typeof(PictureLeftButton), DefaultPictureSize, BindingMode.OneWay, propertyChanged: ImageSizePropertyChanged);
		public int ImageSize
		{
			get
			{
				return (int)GetValue(ImageSizeProperty);
			}

			set
			{
				SetValue(ImageSizeProperty, value);
			}
		}
		private static void ImageSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{

			var control = (PictureLeftButton)bindable;
			if (control != null)
			{
				control.ImgIcon.WidthRequest = (int)newValue;
				control.ImgIcon.HeightRequest = (int)newValue;
			}
		}

		/// <summary>
		/// Button Command
		/// </summary>
		public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(Command), typeof(PictureLeftButton));
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
		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(PictureLeftButton));
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
		/// Button TextColor
		/// </summary>
		public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(PictureLeftButton), Colors.Wheat, BindingMode.OneWay, propertyChanged: TextColorPropertyChanged);
		public Color TextColor
		{
			get
			{
				return (Color)GetValue(TextColorProperty);
			}

			set
			{
				SetValue(TextColorProperty, value);
			}
		}

		#endregion

		#region Ctor

		public PictureLeftButton ()
		{
			InitializeComponent ();

			ImgIcon.WidthRequest = DefaultPictureSize;
			ImgIcon.HeightRequest = DefaultPictureSize;
		}

		#endregion

		#region Events / Commands / Overrides
		private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			if (Command != null && IsEnabled)
			{
				Command.Execute(CommandParameter);
			}
		}
		private static void TextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{

			var control = (PictureLeftButton)bindable;
			if (control != null)
			{
				control.LblName.TextColor = (Color)newValue;
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

		#endregion
	}
}
