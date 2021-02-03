using System;
using System.Collections;
using System.Collections.Generic;

namespace Ryujinx.Memory.Range
{
    /// <summary>
    /// Sorted list of ranges that supports binary search.
    /// </summary>
    /// <typeparam name="T">Type of the range.</typeparam>
    public class RangeList<T> : IEnumerable<T> where T : IRange
    {
        private const int ArrayGrowthSize = 32;

        protected readonly IntervalTree<T> Items;

        public int Count => Items.Count;

        /// <summary>
        /// Creates a new range list.
        /// </summary>
        public RangeList()
        {
            Items = new IntervalTree<T>();
        }

        /// <summary>
        /// Adds a new item to the list.
        /// </summary>
        /// <param name="item">The item to be added</param>
        public void Add(T item)
        {
            Items.Add(item, item.EndAddress);
        }

        /// <summary>
        /// Removes an item from the list.
        /// </summary>
        /// <param name="item">The item to be removed</param>
        /// <returns>True if the item was removed, or false if it was not found</returns>
        public bool Remove(T item)
        {
            return Items.Remove(item);
        }

        /// <summary>
        /// Gets the first item on the list overlapping in memory with the specified item.
        /// </summary>
        /// <remarks>
        /// Despite the name, this has no ordering guarantees of the returned item.
        /// It only ensures that the item returned overlaps the specified item.
        /// </remarks>
        /// <param name="item">Item to check for overlaps</param>
        /// <returns>The overlapping item, or the default value for the type if none found</returns>
        public T FindFirstOverlap(T item)
        {
            return FindFirstOverlap(item.Address, item.Size);
        }

        /// <summary>
        /// Gets the first item on the list overlapping the specified memory range.
        /// </summary>
        /// <remarks>
        /// Despite the name, this has no ordering guarantees of the returned item.
        /// It only ensures that the item returned overlaps the specified memory range.
        /// </remarks>
        /// <param name="address">Start address of the range</param>
        /// <param name="size">Size in bytes of the range</param>
        /// <returns>The overlapping item, or the default value for the type if none found</returns>
        public T FindFirstOverlap(ulong address, ulong size)
        {
            /*   int index = BinarySearch(address, size);

               if (index < 0)
               {
                   return default(T);
               }
            */
            T[] arr = new T[0];

            int overlapCount = Items.OverlapsOf(address, address + size, ref arr);

            if(overlapCount == 0)
            {
                return default(T);
            }
            return arr[0];
        }

        /// <summary>
        /// Gets all items overlapping with the specified item in memory.
        /// </summary>
        /// <param name="item">Item to check for overlaps</param>
        /// <param name="output">Output array where matches will be written. It is automatically resized to fit the results</param>
        /// <returns>The number of overlapping items found</returns>
        public int FindOverlaps(T item, ref T[] output)
        {
            return Items.OverlapsOf(item.Address, item.EndAddress, ref output);
            //return FindOverlaps(item.Address, item.Size, ref output);
        }

        /// <summary>
        /// Gets all items on the list overlapping the specified memory range.
        /// </summary>
        /// <param name="address">Start address of the range</param>
        /// <param name="size">Size in bytes of the range</param>
        /// <param name="output">Output array where matches will be written. It is automatically resized to fit the results</param>
        /// <returns>The number of overlapping items found</returns>
        public int FindOverlaps(ulong address, ulong size, ref T[] output)
        {
            Console.WriteLine($"Finding overlaps for: {address}");
            return Items.OverlapsOf(address, address + size, ref output);
            /*
            int outputIndex = 0;

            ulong endAddress = address + size;

            foreach (T item in Items)
            {
                if (item.Address >= endAddress)
                {
                    break;
                }

                if (item.OverlapsWith(address, size))
                {
                    if (outputIndex == output.Length)
                    {
                        Array.Resize(ref output, outputIndex + ArrayGrowthSize);
                    }

                    output[outputIndex++] = item;
                }
            }

            return outputIndex;*/
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}