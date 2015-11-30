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
    [Serializable]
    public class ObservedList<T> : List<T>
    {

        public event Action<int> Changed = delegate { };
        public event Action Updated = delegate { };
        public event Action Removed = delegate { };
        public event Action Added = delegate { };

        public new void Add(T item)
        {
            base.Add(item);
            Added();
            Updated();
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            Removed();
            Updated();
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            Updated();
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            Updated();
        }

        public new void Clear()
        {
            base.Clear();
            Updated();
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            Updated();
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            Updated();
        }

        public new void RemoveAll(Predicate<T> match)
        {
            base.RemoveAll(match);
            Updated();
        }

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
