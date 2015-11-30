/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System;
using System.Collections.Generic;

#pragma warning disable 67

namespace IBM.Watson.Utilities
{
    /// <summary>
    /// Observable implementation of the generic List class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ObservedList<T> : List<T>
    {
        /// <summary>
        /// Invoked when an element is changed.
        /// </summary>
        public event Action<int> Changed = delegate { };
        /// <summary>
        /// Invoked when elements are added, removed, or changed.
        /// </summary>
        public event Action Updated = delegate { };
        /// <summary>
        /// Invoked when a element is removed.
        /// </summary>
        public event Action Removed = delegate { };
        /// <summary>
        /// Invoked when a element is added.
        /// </summary>
        public event Action Added = delegate { };
        /// <summary>
        /// Add a new item to the list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public new void Add(T item)
        {
            base.Add(item);
            Added();
            Updated();
        }
        /// <summary>
        /// Remove a element from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public new void Remove(T item)
        {
            base.Remove(item);
            Removed();
            Updated();
        }
        /// <summary>
        /// Add a range of objects to this list.
        /// </summary>
        /// <param name="collection">The range to add.</param>
        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            Updated();
        }
        /// <summary>
        /// Remove a range from this list.
        /// </summary>
        /// <param name="index">Start index.</param>
        /// <param name="count">Count to remove.</param>
        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            Updated();
        }
        /// <summary>
        /// Clear this list.
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            Updated();
        }
        /// <summary>
        /// Insert an item into this list.
        /// </summary>
        /// <param name="index">Index to insert at.</param>
        /// <param name="item">Item to insert.</param>
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            Updated();
        }
        /// <summary>
        /// Insert a range of items into this list.
        /// </summary>
        /// <param name="index">The start index.</param>
        /// <param name="collection">The items to insert.</param>
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            Updated();
        }
        /// <summary>
        /// Remove matching items from this list.
        /// </summary>
        /// <param name="match">The comparison operator.</param>
        public new void RemoveAll(Predicate<T> match)
        {
            base.RemoveAll(match);
            Updated();
        }

        /// <summary>
        /// Access a element in this list.
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <returns>The item.</returns>
        public new T this[int index]
        {
            get { return base[index]; }
            set
            {
                base[index] = value;
                Changed(index);
            }
        }
    }
}
