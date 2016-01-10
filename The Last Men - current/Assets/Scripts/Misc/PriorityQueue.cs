using UnityEngine;
using System.Collections;
using C5;
using System;

public class PriorityQueue<T> {

    public class PriorityQueueElement : IComparable<PriorityQueueElement>
    {
        public int m_key;
        public T m_value;
        public IPriorityQueueHandle<PriorityQueueElement> m_handle;

        public PriorityQueueElement(int _key, T _value)
        {
            this.m_key = _key;
            this.m_value = _value;
            this.m_handle = null;
        }

        public int CompareTo(PriorityQueueElement other)
        {
            if (this.m_key <= other.m_key)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    // Store elems of priority queue
    private IntervalHeap<PriorityQueueElement> m_Items;

    public PriorityQueue()
    {
        m_Items = new IntervalHeap<PriorityQueueElement>();
    }

	// Use this for initialization
	public void Enqueue(int priority, T value)
    {
        IPriorityQueueHandle<PriorityQueueElement> handle = null;
        PriorityQueueElement elem;

        m_Items.Add(ref handle, new PriorityQueueElement(priority, value));
        m_Items.Find(handle, out elem);
        elem.m_handle = handle;
    }

    public void Enqueue(PriorityQueueElement elem)
    {
        IPriorityQueueHandle<PriorityQueueElement> handle = null;

        m_Items.Add(ref handle, elem);
        m_Items.Find(handle, out elem);
        elem.m_handle = handle;
    }

    public PriorityQueueElement Dequeue()
    {
        PriorityQueueElement elem = m_Items.DeleteMin();

        // Set handle to null <- elem not in queue
        elem.m_handle = null;

        return elem;
    }

    public void UpdateElement(PriorityQueueElement elem)
    {
        m_Items.Replace(elem.m_handle, elem);
    }

    public bool Contains(T value, out PriorityQueueElement elem)
    {
        foreach(PriorityQueueElement e in m_Items)
        {
            if(EqualityComparer<T>.Default.Equals(e.m_value, value))
            {
                elem = e;
                return true;
            }
        }

        elem = null;
        return false;
    }

    public bool Contains(T value)
    {
        foreach (PriorityQueueElement e in m_Items)
        {
            if (EqualityComparer<T>.Default.Equals(e.m_value, value))
            {
                return true;
            }
        }
        return false;
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
