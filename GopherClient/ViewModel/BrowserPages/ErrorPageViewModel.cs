using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient.ViewModel.BrowserPages
{
	public class ErrorPageViewModel : BrowserPageBase
	{
		string _message;
		public string Message{
			get{
				return _message;
			}
			private set{
				_message = value;
			}
		}

		public ErrorPageViewModel(string message){
			Message = message;
		}
	}
}
