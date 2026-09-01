using System;
using System.Collections.Generic;
using System.Text;

using Experiment.Data.Models;

namespace Experiment.Maui.Models{
	public class VisualUser : UserInfo
	{
		public string BallanceInfo
		{
			get
			{
				if(IsMe)
				{
					return string.Format(E.T("tokenBallance"), Tokens);
				}
				return string.Empty;
			}
		}
	}
}
