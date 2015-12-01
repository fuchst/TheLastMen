using UnityEngine;
using System.Collections;
using C5;
using System;

public class PriorityQueue<T> {

    public class PriorityQueueElement : IComparable<PriorityQueueElement>
    {
        public int key;
        public T value;

        public PriorityQueueElement(int _key, T _value)
        {
            this.key = _key;
            this.value = _value;
        }

        public int CompareTo(PriorityQueueElement other)
        {
            if (this.key <= other.key)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    private IntervalHeap<PriorityQueueElement> m_Items;

    public PriorityQueue()
    {
        m_Items = new IntervalHeap<PriorityQueueElement>();
    }

	// Use this for initialization
	public void Enqueue(int priority, T value)
    {
        m_Items.Add(new PriorityQueueElement(priority, value));
    }

    public PriorityQueueElement Dequeue()
    {
        return m_Items.DeleteMin();
    }

    public int GetSize()
    {
        return m_Items.Count;
    }

    public bool IsEmpty()
    {
        return m_Items.IsEmpty;
    }
}
