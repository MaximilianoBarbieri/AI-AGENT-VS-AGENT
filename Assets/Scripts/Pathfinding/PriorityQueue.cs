using System.Collections.Generic;
using System.Linq;

public class PriorityQueue<T>
{
    private SortedList<float, Queue<T>> _elements = new SortedList<float, Queue<T>>();
    public int Count { get; private set; }

    public void Enqueue(T elem, float cost)
    {
        if (!_elements.ContainsKey(cost))
            _elements[cost] = new Queue<T>();

        _elements[cost].Enqueue(elem);
        Count++;
    }

    public T Dequeue()
    {
        if (Count == 0)
            return default;

        var firstKey = _elements.Keys.First();
        var queue = _elements[firstKey];

        T elem = queue.Dequeue();
        if (queue.Count == 0)
            _elements.Remove(firstKey);

        Count--;
        return elem;
    }

    public bool Contains(T elem)
    {
        return _elements.Values.Any(queue => queue.Contains(elem));
    }
}