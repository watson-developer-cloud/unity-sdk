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

using IBM.Cloud.SDK.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace IBM.Cloud.SDK.Utilities
{
    /// <summary>
    /// WAV Utility functions.
    /// </summary>
    static public class WaveFile
    {
        #region Private Types
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct IFF_FORM_CHUNK
        {
            public uint form_id;
            public uint form_length;
            public uint id;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct IFF_CHUNK
        {
            public uint id;
            public uint length;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct WAV_PCM
        {
            public ushort format_tag;
            public ushort channels;
            public uint sample_rate;
            public uint average_data_rate;
            public ushort alignment;
            public ushort bits_per_sample;
        };
        #endregion

        #region Private Functions
        private static T ReadType<T>(BinaryReader reader)
        {
#if NETFX_CORE
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf<T>());

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();
#else
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
#endif

            return theStructure;
        }

        private static void WriteType<T>(BinaryWriter writer, T data)
        {
            int size = Marshal.SizeOf(data);
            byte[] bytes = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(data, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);

            writer.Write(bytes, 0, size);
        }

        private static uint MakeID(string id)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(id);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private static string GetID(uint id)
        {
            byte[] bytes = BitConverter.GetBytes(id);
            return new string(new char[] { (char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3] });
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Creates a AudioClip object from WAV file data.
        /// </summary>
        /// <param name="clipName">What name to give the AudioClip.</param>
        /// <param name="data">The raw data of the WAV file.</param>
        /// <returns>Returns an AudioClip object on success, null on failure.</returns>
        public static AudioClip ParseWAV(string clipName, byte[] data)
        {
            MemoryStream stream = new MemoryStream(data, false);
            BinaryReader reader = new BinaryReader(stream);

            IFF_FORM_CHUNK form = ReadType<IFF_FORM_CHUNK>(reader);
            if (GetID(form.form_id) != "RIFF" || GetID(form.id) != "WAVE")
            {
                Log.Error("WaveFile.ParseWAV()", "Malformed WAV header: {0} != RIFF || {1} != WAVE", GetID(form.form_id), GetID(form.id));
                return null;
            }

            WAV_PCM header = new WAV_PCM();
            bool bHeaderFound = false;

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                IFF_CHUNK chunk = ReadType<IFF_CHUNK>(reader);

                int ChunkLength = (int)chunk.length;
                if (ChunkLength < 0)  // HACK: Deal with TextToSpeech bug where the chunk length is not set for the data chunk..
                    ChunkLength = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                if ((ChunkLength & 0x1) != 0)
                    ChunkLength += 1;

                long ChunkEnd = reader.BaseStream.Position + ChunkLength;
                if (GetID(chunk.id) == "fmt ")
                {
                    bHeaderFound = true;
                    header = ReadType<WAV_PCM>(reader);
                }
                else if (GetID(chunk.id) == "data")
                {
                    if (!bHeaderFound)
                    {
                        Log.Error("WaveFile.ParseWAV()", "Failed to find header.");
                        return null;
                    }
                    byte[] waveform = reader.ReadBytes(ChunkLength);

                    // convert into a float based wave form..
                    int channels = (int)header.channels;
                    int bps = (int)header.bits_per_sample;
                    float divisor = 1 << (bps - 1);
                    int bytesps = bps / 8;
                    int samples = waveform.Length / bytesps;

                    Log.Debug("WaveFile.ParseWAV()", "WAV INFO, channels = {0}, bps = {1}, samples = {2}, rate = {3}",
                        channels, bps, samples, header.sample_rate);

                    float[] wf = new float[samples];
                    if (bps == 16)
                    {
                        for (int s = 0; s < samples; ++s)
                            wf[s] = ((float)BitConverter.ToInt16(waveform, s * bytesps)) / divisor;
                    }
                    else if (bps == 32)
                    {
                        for (int s = 0; s < samples; ++s)
                            wf[s] = ((float)BitConverter.ToInt32(waveform, s * bytesps)) / divisor;
                    }
                    else if (bps == 8)
                    {
                        for (int s = 0; s < samples; ++s)
                            wf[s] = ((float)BitConverter.ToChar(waveform, s * bytesps)) / divisor;
                    }
                    else
                    {
                        Log.Error("WaveFile.ParseWAV()", "Unspported BPS {0} in WAV data.", bps.ToString());
                        return null;
                    }

                    AudioClip clip = AudioClip.Create(clipName, samples, channels, (int)header.sample_rate, false);
                    clip.SetData(wf, 0);

                    return clip;
                }

                reader.BaseStream.Position = ChunkEnd;
            }

            return null;
        }

        /// <summary>
        /// Creates a WAV file from a AudioClip object.
        /// </summary>
        /// <param name="clip">The AudioClip object to generate the WAV file from.</param>
        /// <param name="bps">How many bits per sample we should use in the WAV file, 8, 16, or 32.</param>
        /// <returns>Returns a byte array of the raw data of the WAV file.</returns>
        public static byte[] CreateWAV(AudioClip clip, int bps = 16)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            IFF_FORM_CHUNK form = new IFF_FORM_CHUNK();
            form.form_id = MakeID("RIFF");
            form.form_length = 0xffffffff;
            form.id = MakeID("WAVE");
            WriteType(writer, form);

            long form_start = writer.BaseStream.Position;
            float divisor = 1 << (bps - 1);

            WAV_PCM header = new WAV_PCM();
            header.format_tag = 1;
            header.alignment = 2;
            header.bits_per_sample = (ushort)bps;
            header.sample_rate = (uint)clip.frequency;
            header.channels = (ushort)clip.channels;
            header.average_data_rate = (uint)((bps / 8) * clip.channels * clip.frequency);

            IFF_CHUNK fmt = new IFF_CHUNK();
            fmt.id = MakeID("fmt ");
            fmt.length = (uint)Marshal.SizeOf(header);
            WriteType(writer, fmt);
            WriteType(writer, header);

            IFF_CHUNK data = new IFF_CHUNK();
            data.id = MakeID("data");
            data.length = (uint)((bps / 8) * clip.samples * clip.channels);
            WriteType(writer, data);

            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            if (bps == 16)
            {
                for (int i = 0; i < samples.Length; ++i)
                    writer.Write((short)(samples[i] * divisor));
            }
            else if (bps == 32)
            {
                for (int i = 0; i < samples.Length; ++i)
                    writer.Write((int)(samples[i] * divisor));
            }
            else if (bps == 8)
            {
                for (int i = 0; i < samples.Length; ++i)
                    writer.Write((char)(samples[i] * divisor));
            }
            else
            {
                Log.Error("WaveFile.CreateWAV()", "Unsupported BPS {0} in WAV data.", bps.ToString());
                return null;
            }

            // lastly, update the form length..
            form.form_length = (uint)(writer.BaseStream.Position - form_start);
            writer.Seek(0, SeekOrigin.Begin);
            WriteType(writer, form);

#if NETFX_CORE
            ArraySegment<byte> bytes;
            if (stream.TryGetBuffer(out bytes))
                return bytes.Array;
            else
                return null;
#else
            return stream.GetBuffer();
#endif
        }
        #endregion
    }
}
