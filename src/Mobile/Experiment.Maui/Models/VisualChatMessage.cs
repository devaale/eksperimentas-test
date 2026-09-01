using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

using Experiment.Data.Models;

namespace Experiment.Maui.Models{
	public class VisualChatMessage : ChatMessage
	{
		#region Const / ReadOnly

		static readonly Thickness MarginToLeft = new Thickness(40, 0, 0, 0);
		static readonly Thickness MarginToRight = new Thickness(0, 0, 0, 40);

		static readonly Style PanelOutMessage = (Style)Application.Current.Resources["msgOutPanel"];
		static readonly Style PanelInMessage = (Style)Application.Current.Resources["msgInPanel"];

		static readonly Style LabelOutMessage = (Style)Application.Current.Resources["msgOutLabel"];
		static readonly Style LabelInMessage = (Style)Application.Current.Resources["msgInLabel"];

		#endregion

		#region Properties

		public Style PanelStyle { get => IsMyMessage ? PanelOutMessage : PanelInMessage; }
		public Style LabelStyle { get => IsMyMessage ? LabelOutMessage : LabelInMessage; }
		public TextAlignment MessageAlignment { get => (IsMyMessage ? TextAlignment.Start : TextAlignment.End); }
		public Color MessageBackColor { get => (IsMyMessage ? Colors.Gold : Colors.Aquamarine); }
		public FontAttributes FontAttribute { get => !IsMyMessage && !Read.HasValue ? FontAttributes.Bold : FontAttributes.None; }

		#endregion
	}
}

