using System;
using System.Collections.Generic;
using System.Text;

namespace GopherClient.ViewModel.ResourceTypes
{
	public class ErrorResource : ResourceTypeBase
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

		public ErrorResource(string message){
			Message = message;
		}
	}
}
