using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate int Priority<T> (T left, T right);

class Node<T>
{
    private T value;
}

public class Heap<T>
{
    private List<T> _arrayHeap;
    private Priority<T> _priorityCalc;

    public List<T> ArrayHeap => _arrayHeap;

    public Heap(Priority<T> priorityCalc, List<T> heap = null)
    {
        if (heap == null)
        {
            heap = new List<T>();
        }
        _arrayHeap = heap;
        _priorityCalc = priorityCalc;
    }

    public int Count => _arrayHeap.Count;

    private void BubbleDown(int n = 0)
    {
        // formula is (2 * n + nChild)
        int childL = 2 * n + 1;
        int childR = 2 * n + 2;
        
        // Check that we are not gonna crash everything
        if (childL >= _arrayHeap.Count) return;
        // If we cannot have a right child we just use the same leftChild
        if (childR >= _arrayHeap.Count) childR = childL;
        
        // Which child we should compare to?
        int child = Check(childL, childR) == childL ? childL : childR;
        
        // While current is less than count, childToCompare less than count and the child should be higher than current
        while (n < _arrayHeap.Count && child < _arrayHeap.Count && Check(child, n) == child)
        {
            // They should be swapped, let's swap!
            Swap(n, child);
            // Current is now the childNode            
            n = child;
            
            // Choose next child
            int nextChildLeft = 2 * n + 1;
            int nextChildRight = 2 * n + 2;
            
            if (nextChildLeft >= _arrayHeap.Count)
            {
                // Stop swapping, we are on the last one
                return;
            }

            if (nextChildRight >= _arrayHeap.Count)
            {
                nextChildRight = nextChildLeft;
            }
            
            // Which child we should compare next?
            child = Check(nextChildLeft, nextChildRight) == nextChildLeft ? nextChildLeft : nextChildRight;
        }
    }

    private int Check(int i, int j)
    {
        return _priorityCalc(_arrayHeap[i], _arrayHeap[j]) <= 0 ? i : j;
    }
    
    private void BubbleUp(int child)
    {
        int parent = (int) Mathf.Floor(((float) (child - 1)) / 2.0f);
        while (child >= 0 && parent >= 0 && Check(parent, child) == child)
        {
            Swap(parent, child);
            child = parent;
            parent = (int) Mathf.Floor((child - 1) / 2.0f);
        }
    }

    private void Swap(int i, int j)
    {
        T temp = _arrayHeap[i];
        _arrayHeap[i] = _arrayHeap[j];
        _arrayHeap[j] = temp;
    }

    public void Enqueue(T toAdd)
    {
        _arrayHeap.Add(toAdd);
        BubbleUp(_arrayHeap.Count - 1);
    }

    public T Dequeue()
    {
        T last = _arrayHeap[0];
        Swap(0, _arrayHeap.Count - 1);
        // I swear to god if C# or .NET doesn't optimize this to a pop operation...
        _arrayHeap.RemoveAt(_arrayHeap.Count - 1);
        BubbleDown();
        return last;
    }
}
