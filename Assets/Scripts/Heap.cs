using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] _items;
    int _currentItemCount;
    
    public Heap(int maxHeapSize)
    {
        _items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        //Put on bottom of heap
        item.HeapIndex = _currentItemCount;
        _items[_currentItemCount] = item;
        //Sort up to correct position
        SortUp(item);
        _currentItemCount++;
    }
    
    public T RemoveFirst()
    {
        //Get top item from heap
        T firstItem = _items[0];
        //Decrement item count
        _currentItemCount--;
        //Move last item to top of heap
        _items[0] = _items[_currentItemCount];
        //Update first item's heap index
        _items[0].HeapIndex = 0;
        //Sort down to correct position
        SortDown(_items[0]);
        //Return first item
        return firstItem;
    }

    private void SortDown(T item)
    {
        while (true)
        {
            //Get left and right child indices
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            //Set swap index to current item
            int swapIndex = 0;
            
            //If left child is within heap
            //Note: Left child must be within heap to have a right child
            if (childIndexLeft < _currentItemCount)
            {
                //Set swap index to left child
                swapIndex = childIndexLeft;
                //If right child is within heap
                if (childIndexRight < _currentItemCount)
                {
                    //If right child is less than left child
                    if (_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0)
                    {
                        //Set swap index to right child
                        swapIndex = childIndexRight;
                    }
                }
                //If current item is less than swap item
                if (item.CompareTo(_items[swapIndex]) < 0)
                {
                    //Swap items
                    Swap(item, _items[swapIndex]);
                }
                else
                {
                    //If item f cost is less than swap item f cost
                    //Current item is in correct position
                    return;
                }
            }
            else
            {
                //Has no children
                //Current item is in correct position
                return;
            }
        }
    }
    
    public int Count => _currentItemCount;
    
    public bool Contains(T item) => Equals(_items[item.HeapIndex], item);

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        
        while (true)
        {
            T parentItem = _items[parentIndex];
            //If item has smaller fCost than parent, swap
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            //Update parent index
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }
    
    private void Swap(T itemA, T itemB)
    {
        //Swap items in array
        _items[itemA.HeapIndex] = itemB;
        _items[itemB.HeapIndex] = itemA;
        //Swap indexes
        (itemA.HeapIndex, itemB.HeapIndex) = (itemB.HeapIndex, itemA.HeapIndex);
    }
    
}

//Make items can be compared
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}