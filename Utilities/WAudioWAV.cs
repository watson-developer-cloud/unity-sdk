using UnityEngine;
using System;
using System.Collections;
using System.IO;

//REF1: https://github.com/watson-developer-cloud/speech-ios-sdk
//REF2: http://answers.unity3d.com/questions/737002/wav-byte-to-audioclip.html

public class WAudioWAV : WUtilities  {

	// properties
	public float[] leftChannel{get; internal set;}
	public float[] rightChannel{get; internal set;}
	public int channelCount {get;internal set;}
	public int sampleCount {get;internal set;}
	public int frequency {get;internal set;}


	public WAudioWAV(byte[] wavRaw, bool isRaw){

		byte[] wav = null;
		if(isRaw)
			wav = getAudioByteArrayAfterStrippingAndAddingNewWAVHeader(wavRaw);
		else
			wav = wavRaw;

		// Determine if mono or stereo - WAV Channel information
		channelCount = BitConverter.ToInt16(wav,22);
		
		// Getting the frequency
		frequency = BitConverter.ToInt32(wav,24);
		
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
		sampleCount = (wav.Length - pos)/2;     // 2 bytes per sample (16 bit sound mono)
		if (channelCount == 2) sampleCount /= 2;        // 4 bytes per sample (16 bit stereo)
		
		// Allocate memory (right will be null if only mono sound)
		leftChannel = new float[sampleCount];
		if (channelCount == 2) rightChannel = new float[sampleCount];
		else rightChannel = null;
		
		// Write to double array/s:
		int i=0;
		while (pos < wav.Length) {
			leftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
			pos += 2;
			if (channelCount == 2) {
				rightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
				pos += 2;
			}
			i++;
		}
	}

	public static byte[] getAudioByteArrayAfterStrippingAndAddingNewWAVHeader(byte[] wav) {
		
		int headerSize = 44;
		int metadataSize = 48;
		int sampleRate = 0;

		if(wav.Length > 28){	//Get WAV Sample rate from 24
			sampleRate = BitConverter.ToInt32(wav, 24);
		}
		
		//Creating a new WAV without header information
		byte[] wavNoheader = new byte[wav.Length - (headerSize+metadataSize)];
		System.Buffer.BlockCopy(wav,headerSize+metadataSize, wavNoheader, 0, wav.Length - (headerSize+metadataSize));
		
		//Adding WAV header information
		byte[] newWavData = getAudioByteArrayWithWAVHeader(wavNoheader, sampleRate);
		
		return newWavData;
	}
	
	public static byte[] getAudioByteArrayWithWAVHeader(byte[] wavNoheader, int sampleRate) {
		
		int headerSize = 44;
		long totalAudioLen = wavNoheader.Length;
		long totalDataLen = wavNoheader.Length + headerSize-8;
		long longSampleRate = (sampleRate == 0 ? 48000 : sampleRate);
		int channels = 1;
		long byteRate = 16 * 11025 * channels/8;
		
		//Byte *header = (Byte*)malloc(44);
		byte[] header = new byte[44];
		header[0] = (byte)'R';  // RIFF/WAVE header
		header[1] = (byte)'I';
		header[2] = (byte)'F';
		header[3] = (byte)'F';
		header[4] = (Byte) (totalDataLen & 0xff);
		header[5] = (Byte) ((totalDataLen >> 8) & 0xff);
		header[6] = (Byte) ((totalDataLen >> 16) & 0xff);
		header[7] = (Byte) ((totalDataLen >> 24) & 0xff);
		header[8] = (byte)'W';
		header[9] = (byte)'A';
		header[10] = (byte)'V';
		header[11] = (byte)'E';
		header[12] = (byte)'f';  // 'fmt ' chunk
		header[13] = (byte)'m';
		header[14] = (byte)'t';
		header[15] = (byte)' ';
		header[16] = 16;  // 4 bytes: size of 'fmt ' chunk
		header[17] = 0;
		header[18] = 0;
		header[19] = 0;
		header[20] = 1;  // format = 1
		header[21] = 0;
		header[22] = (Byte) channels;
		header[23] = 0;
		header[24] = (Byte) (longSampleRate & 0xff);
		header[25] = (Byte) ((longSampleRate >> 8) & 0xff);
		header[26] = (Byte) ((longSampleRate >> 16) & 0xff);
		header[27] = (Byte) ((longSampleRate >> 24) & 0xff);
		header[28] = (Byte) (byteRate & 0xff);
		header[29] = (Byte) ((byteRate >> 8) & 0xff);
		header[30] = (Byte) ((byteRate >> 16) & 0xff);
		header[31] = (Byte) ((byteRate >> 24) & 0xff);
		header[32] = (Byte) (2 * 8 / 8);  // block align
		header[33] = 0;
		header[34] = 16;  // bits per sample
		header[35] = 0;
		header[36] = (byte)'d';
		header[37] = (byte)'a';
		header[38] = (byte)'t';
		header[39] = (byte)'a';
		header[40] = (Byte) (totalAudioLen & 0xff);
		header[41] = (Byte) ((totalAudioLen >> 8) & 0xff);
		header[42] = (Byte) ((totalAudioLen >> 16) & 0xff);
		header[43] = (Byte) ((totalAudioLen >> 24) & 0xff);
		
		byte[] newWavData = new byte[wavNoheader.Length + 44];
		
		System.Buffer.BlockCopy(header,0, newWavData, 0, 44);
		System.Buffer.BlockCopy(wavNoheader,0, newWavData, 44, wavNoheader.Length);
		
		return newWavData;
	}
	
	public override string ToString (){
		return string.Format ("Waton WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}", leftChannel.Length, rightChannel, channelCount, sampleCount, frequency);
	}
}