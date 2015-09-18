using UnityEngine;
using System;
using System.Collections;
using System.IO;

//REF: http://answers.unity3d.com/questions/737002/wav-byte-to-audioclip.html

public class WAudioWAV : WUtilities  {

	// properties
	public float[] LeftChannel{get; internal set;}
	public float[] RightChannel{get; internal set;}
	public int ChannelCount {get;internal set;}
	public int SampleCount {get;internal set;}
	public int Frequency {get;internal set;}

	public WAudioWAV(byte[] wav){
		
		// Determine if mono or stereo - WAV Channel information
		ChannelCount = BitConverter.ToInt16(wav,22);
		
		// Getting the frequency
		Frequency = BitConverter.ToInt32(wav,24);
		
		// Get past all the other sub chunks to get to the data subchunk:
		int pos = 12;   // First Subchunk ID from 12 to 16
		
		// Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
		while(!(wav[pos]==100 && wav[pos+1]==97 && wav[pos+2]==116 && wav[pos+3]==97)) {
			pos += 4;
			int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
			pos += 4 + chunkSize;
		}
		pos += 8;
		
		// Pos is now positioned to start of actual sound data.
		SampleCount = (wav.Length - pos)/2;     // 2 bytes per sample (16 bit sound mono)
		if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)
		
		// Allocate memory (right will be null if only mono sound)
		LeftChannel = new float[SampleCount];
		if (ChannelCount == 2) RightChannel = new float[SampleCount];
		else RightChannel = null;
		
		// Write to double array/s:
		int i=0;
		while (pos < wav.Length) {
			LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
			pos += 2;
			if (ChannelCount == 2) {
				RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
				pos += 2;
			}
			i++;
		}
	}
	
	public override string ToString (){
		return string.Format ("Waton WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}", LeftChannel.Length, RightChannel, ChannelCount, SampleCount, Frequency);
	}
}