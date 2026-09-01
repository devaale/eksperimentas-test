using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.Metadata{
	/// <summary>
	/// Written to simplify MVVM ViewModel load triggering function calls from forms.
	/// 
	/// </summary>
	public interface ILoadableAsync
	{
		Task LoadAsync(object sender);
	}

#if USAGE_FOR_PAGES_NOT_HERE
	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (BindingContext != null && BindingContext is ILoadableAsync)
		{
			await ((ILoadableAsync)BindingContext).LoadAsync(this);
		}
	}
#endif

}
