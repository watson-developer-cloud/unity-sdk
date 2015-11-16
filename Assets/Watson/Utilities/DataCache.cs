using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IBM.Watson.Utilities
{
    public class DataCache
    {
        private string m_CachePath = null;
        private Dictionary<string, byte[]> m_Cache = new Dictionary<string, byte[]>();

        /// <summary>
        /// DataCache constructor.
        /// </summary>
        /// <param name="cacheName">The name of the cache.</param>
        /// <param name="useStreamingPath">If true, then cache files are stored in the streaming asset path.</param>
        public DataCache(string cacheName, bool useStreamingPath = false)
        {
            Initialize(cacheName,useStreamingPath);
        }

        /// <summary>
        /// Initializes this DataCache object.
        /// </summary>
        /// <param name="cacheName">The name of the cache.</param>
        /// <param name="useStreamingPath">If true, then cache files are stored in the streaming asset path.</param>
        public void Initialize(string cacheName, bool useStreamingPath = false )
        {
            if (string.IsNullOrEmpty(cacheName))
                throw new ArgumentNullException("cacheName");
            if (cacheName.Contains('/'))
                throw new ArgumentException("cacheName");


            m_CachePath = (useStreamingPath ? Application.streamingAssetsPath : Application.persistentDataPath) + "/" + cacheName + "/";
            if (!Directory.Exists(m_CachePath))
                Directory.CreateDirectory(m_CachePath);

            foreach (var f in Directory.GetFiles(m_CachePath))
            {
                string id = Path.GetFileNameWithoutExtension(f);
                m_Cache[id] = File.ReadAllBytes(f);
            }
        }

        /// <summary>
        /// Find a data object by ID.
        /// </summary>
        /// <param name="id">The ID to find.</param>
        /// <returns>The cached data, or null if not found.</returns>
        public byte[] Find(string id)
        {
            byte[] data = null;
            if (m_Cache.TryGetValue(id, out data))
                return data;

            return null;
        }

        /// <summary>
        /// Save data into the cache by ID.
        /// </summary>
        /// <param name="id">The ID to save.</param>
        /// <param name="data"></param>
        public void Save(string id, byte[] data)
        {
            id = id.Replace('/', '_');

            m_Cache[id] = data;
            File.WriteAllBytes(m_CachePath + id + ".bytes", data);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Flush()
        {
            m_Cache.Clear();
            foreach( var f in Directory.GetFiles(m_CachePath))
                File.Delete( f );
        }

    }
}
