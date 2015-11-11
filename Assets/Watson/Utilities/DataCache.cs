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

        public DataCache()
        {}
        public DataCache(string cacheName)
        {
            Initialize(cacheName);
        }

        public void Initialize(string cacheName)
        {
            if (string.IsNullOrEmpty(cacheName))
                throw new ArgumentNullException("cacheName");
            if (cacheName.Contains('/'))
                throw new ArgumentException("cacheName");

            m_CachePath = Application.streamingAssetsPath + "/" + cacheName + "/";
            if (!Directory.Exists(m_CachePath))
                Directory.CreateDirectory(m_CachePath);

            foreach (var f in Directory.GetFiles(m_CachePath))
            {
                string id = Path.GetFileNameWithoutExtension(f);
                m_Cache[id] = File.ReadAllBytes(f);
            }
        }

        public byte[] Find(string id)
        {
            byte[] data = null;
            if (m_Cache.TryGetValue(id, out data))
                return data;

            return null;
        }

        public void Save(string id, byte[] data)
        {
            id = id.Replace('/', '_');

            m_Cache[id] = data;
            File.WriteAllBytes(m_CachePath + id + ".bytes", data);
        }

        public void Flush()
        {
            m_Cache.Clear();
            foreach( var f in Directory.GetFiles(m_CachePath))
                File.Delete( f );
        }

    }
}
