using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GopherClient.Model
{
	public class HistoryStack<T>
	{
		LinkedList<T> stack;
		LinkedListNode<T> current;
		int maxlength;

		public T Value{
			get{
				if (stack.Count == 0)
					return default(T);
				return current.Value;
			}
		}

		public bool HasPrevious{
			get{
				if (current == null)
					return false;
				return current.Previous != null;
			}
		}

		public bool HasNext
		{
			get{
				if (current == null)
					return false;
				return current.Next != null;
			}
		}

		public HistoryStack(int maxlength){
			this.stack = new LinkedList<T>();
			this.maxlength = maxlength;
		}

		public void Push(T value){
			if(current != null)
			if(current.Next != null){
				while(stack.Last != current)
					stack.RemoveLast();
			}
			stack.AddLast(value);
			current = stack.Last;
		}

		public T Previous(){
			current = current.Previous;
			return current.Value;
		}

		public T Next(){
			current = current.Next;
			return current.Value;
		}
	}
}
