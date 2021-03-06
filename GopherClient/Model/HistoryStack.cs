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
            set
            {
				if (stack.Count == 0)
					return;
				current.Value = value;
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
			if (stack.Count > maxlength)
				stack.RemoveFirst();
		}

		public T Previous(){
			current = current.Previous;
			return current.Value;
		}

		public T Next(){
			current = current.Next;
			return current.Value;
		}

		public T[] GetHistory(){
			IEnumerator<T> enumerator = stack.GetEnumerator();
			int i = 0;
			T[] result = new T[stack.Count];
			while(enumerator.MoveNext()){
				result[i] = enumerator.Current;
			}
			return result;
		}

		public T Pop()
        {
			T last = stack.Last.Value;
			stack.RemoveLast();
			current = stack.Last;
			return last;
        }

        public override string ToString()
        {
			string result = "";
			foreach(T v in GetHistory())
            {
				result += v.ToString() + "\n";
            }
			return result;
        }
    }
}