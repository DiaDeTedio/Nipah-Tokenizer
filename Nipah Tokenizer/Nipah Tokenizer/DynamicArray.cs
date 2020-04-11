using System;
using System.Collections.Generic;
using System.Linq;
using Arr = System.Array;

namespace NipahTokenizer
{
	public class DynamicArray<T>
	{
		T[] array;
		int count;

		public int Count => count;
		public T[] Array => array;
		public T this[int index]
		{
			get {
				return array[index];
			}
			set {
				array[index] = value;
			}
		}

		public TNew[] Cast<TNew>()
		{
			TNew[] of = new TNew[count];
			for(int i = 0; i < count; i++)
				of[i] = (TNew)(dynamic)array[i];
			return of;
		}
		public TNew[] As<TNew>() where TNew : class
		{
			TNew[] of = new TNew[count];
			for(int i = 0; i < count; i++)
				of[i] = array[i] as TNew;
			return of;
		}

		public void ForEach(Action<T> iterator) => Arr.ForEach(array, iterator);

		public void Add(T item)
		{
			count++;
			Arr.Resize(ref array, count);
			array[count - 1] = item;
		}
		public void Remove(T item)
		{
			count--;
			var lst = array[count];
			Arr.Resize(ref array, count);
			if(Equals(lst, item))
				return;
			int toRemove = Arr.IndexOf(array, item);
			if(toRemove > -1) {
				for(int i = toRemove + 1; i < count; i++) {
					array[i] = array[i - 1];
				}
				array[count - 1] = lst;
			}
		}
		public void Clear()
		{
			count = 0;
			Arr.Resize(ref array, count);
		}

		public int IndexOf(T item) => Arr.IndexOf(array, item);
		public bool Contains(T item) => Arr.IndexOf(array, item) > -1;
		public bool Exists(Predicate<T> predicate) => Arr.Exists(array, predicate);

		public IEnumerator<T> GetEnumerator()
		{
			for(int i = 0; i < count; i++)
				yield return array[i];
		}

		public DynamicArray()
		{
			array = new T[0];
		}
		public DynamicArray(int capacity)
		{
			array = new T[capacity];
		}
		public DynamicArray(T[] src)
		{
			count = src.Length;
			array = new T[count];
			Arr.Copy(src, array, count);
		}
		public DynamicArray(List<T> src)
		{
			count = src.Count;
			array = src.ToArray();
		}
	}
}
