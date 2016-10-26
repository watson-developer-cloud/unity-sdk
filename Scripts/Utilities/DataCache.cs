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
using System.IO;
using System.Linq;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Logging;

namespace IBM.Watson.DeveloperCloud.Utilities
{
  /// <summary>
  /// This class manages a cache of binary data by ID.
  /// </summary>
  public class DataCache
  {
    #region Private Data
    private string m_CachePath = null;
    private long m_MaxCacheSize = -1;
    private double m_MaxCacheAge = 0;
    private long m_CurrentCacheSize = 0;

    private class CacheItem
    {
      public string Path { get; set; }
      public string Id { get; set; }
      public DateTime Time { get; set; }
      public byte[] Data { get; set; }
    };
    private Dictionary<string, CacheItem> m_Cache = new Dictionary<string, CacheItem>();
    #endregion

    /// <summary>
    /// DataCache constructor.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="maxCacheSize">Maximum cache size in bytes.</param>
    /// <param name="maxCacheAge">Maximum age of a cache item in hours.</param>
    public DataCache(string cacheName, long maxCacheSize = 1024 * 1024 * 50, double maxCacheAge = 24 * 7)
    {
      Initialize(cacheName, maxCacheSize, maxCacheAge);
    }

    /// <summary>
    /// Initializes this DataCache object.
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="maxCacheSize">Maximum cache size in bytes.</param>
    /// <param name="maxCacheAge">Maximum age of a cache item in hours.</param>
    public void Initialize(string cacheName, long maxCacheSize, double maxCacheAge)
    {
      if (string.IsNullOrEmpty(cacheName))
        throw new ArgumentNullException("cacheName");
      cacheName = cacheName.Replace('/', '_');

      m_MaxCacheSize = maxCacheSize;
      m_MaxCacheAge = maxCacheAge;

      m_CachePath = Application.persistentDataPath + Constants.Path.CACHE_FOLDER + "/" + cacheName + "/";
      if (!Directory.Exists(m_CachePath))
        Directory.CreateDirectory(m_CachePath);

      foreach (var f in Directory.GetFiles(m_CachePath))
      {
        DateTime lastWrite = File.GetLastAccessTime(f);
        double age = (DateTime.Now - lastWrite).TotalHours;
        if (age < m_MaxCacheAge)
        {
          CacheItem item = new CacheItem();
          item.Path = f;
          item.Id = Path.GetFileNameWithoutExtension(f);
          item.Time = lastWrite;
          item.Data = null;

          m_Cache[item.Id] = item;
          m_CurrentCacheSize += (new FileInfo(f)).Length;
        }
        else
        {
          Log.Debug("DataCache", "Removing aged cache item {0}", f);
          File.Delete(f);
        }
      }
    }

    /// <summary>
    /// Find a data object by ID.
    /// </summary>
    /// <param name="id">The ID to find.</param>
    /// <returns>The cached data, or null if not found.</returns>
    public byte[] Find(string id)
    {
      if (!string.IsNullOrEmpty(id))
      {
        id = id.Replace('/', '_');

        CacheItem item = null;
        if (m_Cache.TryGetValue(id, out item))
        {
#if !UNITY_ANDROID
          item.Time = DateTime.Now;

          File.SetLastWriteTime(item.Path, item.Time);
#endif

          if (item.Data == null)
          {
            item.Data = File.ReadAllBytes(item.Path);
          }

          return item.Data;
        }
      }

      return null;
    }

    /// <summary>
    /// Is the data cached?
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsCached(string id)
    {
      bool isCached = false;

      if (!string.IsNullOrEmpty(id))
      {
        id = id.Replace('/', '_');

        CacheItem item = null;
        if (m_Cache.TryGetValue(id, out item))
        {
          isCached = File.Exists(item.Path);
        }
      }

      return isCached;
    }

    /// <summary>
    /// Save data into the cache by ID.
    /// </summary>
    /// <param name="id">The ID to save.</param>
    /// <param name="data">The data of the object to save.</param>
    public void Save(string id, byte[] data)
    {
      if (data != null && data.Length > 0)
      {
        id = id.Replace('/', '_');

        if (m_Cache.ContainsKey(id))
        {
          Log.Debug("DataCache", "Has same key in the cache. Flushing old one: {0}", id);
          Flush(id);
        }

        CacheItem item = new CacheItem();
        item.Path = m_CachePath + id + ".bytes";
        item.Id = id;
        item.Time = DateTime.Now;
        item.Data = data;

        File.WriteAllBytes(item.Path, data);
        m_CurrentCacheSize += item.Data.Length;

        m_Cache[id] = item;

        while (m_CurrentCacheSize > m_MaxCacheSize)
          FlushOldest();
      }
      else
      {
        Log.Error("DataCache", "Empty data came to the cache, couldn't cache any null data");
      }
    }

    /// <summary>
    /// Flush a specific item from the cache.
    /// </summary>
    /// <param name="id">The ID of the object to flush.</param>
    public void Flush(string id)
    {
      id = id.Replace('/', '_');

      CacheItem item = null;
      if (m_Cache.TryGetValue(id, out item))
      {
        Log.Debug("DataCache", "Flushing {0} from cache.", item.Path);

        if (item.Data != null)
          m_CurrentCacheSize -= item.Data.Length;
        else
          m_CurrentCacheSize -= (new FileInfo(item.Path)).Length;

        File.Delete(item.Path);

        m_Cache.Remove(id);
      }
    }

    /// <summary>
    /// Flush any aged out data from the cache.
    /// </summary>
    public void FlushAged()
    {
      List<CacheItem> flush = new List<CacheItem>();
      foreach (var kp in m_Cache)
      {
        double age = (DateTime.Now - kp.Value.Time).TotalHours;
        if (age > m_MaxCacheAge)
          flush.Add(kp.Value);
      }

      foreach (var item in flush)
        Flush(item.Id);
    }

    /// <summary>
    /// Flush the oldest item from the cache.
    /// </summary>
    public void FlushOldest()
    {
      CacheItem oldest = null;
      foreach (var kp in m_Cache)
      {
        if (oldest == null || kp.Value.Time < oldest.Time)
          oldest = kp.Value;
      }

      if (oldest != null)
      {
        Flush(oldest.Id);
      }
    }

    /// <summary>
    /// Flush all data from the cache.
    /// </summary>
    public void Flush()
    {
      foreach (var kp in m_Cache)
        File.Delete(kp.Value.Path);
      m_Cache.Clear();
    }
  }
}
