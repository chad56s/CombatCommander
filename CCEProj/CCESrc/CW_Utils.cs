#define DEBUG

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using System.Reflection;
using System.ArrayExtensions;

namespace CW_Utils {

	/*************************************************************************************************************************
	 * 
	 * Heap
	 * 
	 * ***********************************************************************************************************************/
	public abstract class Heap<T> : IEnumerable<T> {
		private const int InitialCapacity = 0;
		private const int GrowFactor = 2;
		private const int MinGrow = 1;

		private int _capacity = InitialCapacity;
		private T[] _heap = new T[InitialCapacity];
		private int _tail = 0;							//points to the next available space in the heap (insertion point)

		public int Count { get { return _tail; } }
		public int Capacity { get { return _capacity; } }

		protected Comparer<T> Comparer { get; private set; }
		protected abstract bool Dominates(T x, T y);

		protected Heap()
			: this(Comparer<T>.Default) {
		}

		protected Heap(Comparer<T> comparer)
			: this(Enumerable.Empty<T>(), comparer) {
		}

		protected Heap(IEnumerable<T> collection)
			: this(collection, Comparer<T>.Default) {
		}

		protected Heap(IEnumerable<T> collection, Comparer<T> comparer) {
			if (collection == null) throw new ArgumentNullException("collection");
			if (comparer == null) throw new ArgumentNullException("comparer");

			Comparer = comparer;

			foreach (var item in collection) {
				if (Count == Capacity)
					Grow();

				_heap[_tail++] = item;
			}

			for (int i = Parent(_tail - 1); i >= 0; i--)
				BubbleDown(i);
			
		}

		public void Add(T item) {
			if (Count == Capacity)
				Grow();

			_heap[_tail++] = item;
			BubbleUp(_tail - 1);
		}

		public void Replace(T x, T y) {
			int idx = Array.IndexOf(_heap, x);
			if (idx >= 0 && idx <= _tail) {

				_heap[idx] = y;

				if (Dominating(idx) == idx)
					BubbleUp(idx);
				else
					BubbleDown(idx);
			}
		}

		private void BubbleUp(int i) {
			if (i == 0 || Dominates(_heap[Parent(i)], _heap[i]))
				return; //correct domination (or root)

			Swap(i, Parent(i));
			BubbleUp(Parent(i));
		}

		public T GetMin() {
			if (Count == 0) throw new InvalidOperationException("Heap is empty");
			return _heap[0];
		}

		public T ExtractDominating() {
			if (Count == 0) throw new InvalidOperationException("Heap is empty");
			T ret = _heap[0];
			_tail--;
			Swap(_tail, 0);
			BubbleDown(0);

			return ret;
		}

		private void BubbleDown(int i) {
			int dominatingNode = Dominating(i);
			if (dominatingNode == i) return;
			Swap(i, dominatingNode);
			BubbleDown(dominatingNode);
		}

		private int Dominating(int i) {
			int dominatingNode = i;
			dominatingNode = GetDominating(YoungChild(i), dominatingNode);
			dominatingNode = GetDominating(OldChild(i), dominatingNode);

			return dominatingNode;
		}

		private int GetDominating(int newNode, int dominatingNode) {
			if (newNode < _tail && !Dominates(_heap[dominatingNode], _heap[newNode]))
				return newNode;
			else
				return dominatingNode;
		}

		private void Swap(int i, int j) {
			T tmp = _heap[i];
			_heap[i] = _heap[j];
			_heap[j] = tmp;
		}

		private static int Parent(int i) {
			return (i + 1) / 2 - 1;
		}

		private static int YoungChild(int i) {
			return (i + 1) * 2 - 1;
		}

		private static int OldChild(int i) {
			return YoungChild(i) + 1;
		}

		private void Grow() {
			int newCapacity = _capacity * GrowFactor + MinGrow;
			var newHeap = new T[newCapacity];
			Array.Copy(_heap, newHeap, _capacity);
			_heap = newHeap;
			_capacity = newCapacity;
		}

		public IEnumerator<T> GetEnumerator() {
			return _heap.Take(Count).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}

	public class MaxHeap<T> : Heap<T> {
		public MaxHeap()
			: this(Comparer<T>.Default) {
		}

		public MaxHeap(Comparer<T> comparer)
			: base(comparer) {
		}

		public MaxHeap(IEnumerable<T> collection, Comparer<T> comparer)
			: base(collection, comparer) {
		}

		public MaxHeap(IEnumerable<T> collection)
			: base(collection) {
		}

		protected override bool Dominates(T x, T y) {
			return Comparer.Compare(x, y) >= 0;
		}

	}

	public class MinHeap<T> : Heap<T> {
		public MinHeap()
			: this(Comparer<T>.Default) {
		}

		public MinHeap(Comparer<T> comparer)
			: base(comparer) {
		}

		public MinHeap(IEnumerable<T> collection)
			: base(collection) {
		}

		public MinHeap(IEnumerable<T> collection, Comparer<T> comparer)
			: base(collection, comparer) {
		}

		protected override bool Dominates(T x, T y) {
			return Comparer.Compare(x, y) <= 0;
		}
	}


	/*************************************************************************************************************************
	 * 
	 * RANDOMIZER
	 * 
	 * ***********************************************************************************************************************/

	public static class Randomizer {
		private static Random randomer = new Random(Guid.NewGuid().GetHashCode());

		public static List<int> rolls = new List<int>();
		public static int result = 0;

		public static int get(int min, int max, int number) {
			result = 0;
			rolls = new List<int>();

			for (int i = 0; i < number; i++) {

				if (min >= max)
					rolls.Add(min);
				else
					rolls.Add(randomer.Next(min, max + 1));
			}

			result = rolls.Sum();

			return result;
		}

		public static int get(int min, int max) {
			return get(min, max, 1);
		}

		public static double getDouble(double max = 1) {
			return randomer.NextDouble() * max;
		}
	}


}



namespace System
{
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }
        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        public static T Copy<T>(this T original)
        {
            return (T)Copy((Object)original);
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }

    namespace ArrayExtensions
    {
        public static class ArrayExtensions
        {
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0) return;
                ArrayTraverse walker = new ArrayTraverse(array);
                do action(array, walker.Position);
                while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }

}