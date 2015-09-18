using UnityEngine;
using System.Collections;

public class WUtilities {


	// convert two bytes to one float in the range -1 to 1
	public static float bytesToFloat(byte firstByte, byte secondByte) {
		// convert two bytes to one short (little endian)
		short s = (short)((secondByte << 8) | firstByte);
		// convert to range from -1 to (just below) 1
		return s / 32768.0F;
	}
	
//	public static int bytesToInt32(byte[] bytes,int offset=0){
//		int value=0;
//		for(int i=0;i<4;i++){
//			value |= ((int)bytes[offset+i])<<(i*8);
//		}
//		return value;
//	}
//	
//	public static int bytesToInt16(byte[] bytes,int offset=0){
//		int value=0;
//		for(int i=0;i<2;i++){
//			value |= ((int)bytes[offset+i])<<(i*8);
//		}
//		return value;
//	}

}
