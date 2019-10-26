using System;

namespace MyDB.Segmentation.Data
{
	public class XorBuilder : DataBuilder
	{
		public string UsePublicSignature()
		{
			return "xor";
		}
		
		public byte[] Key;

		public byte[] Encode(byte[] data)
		{
			int kpos = 0;
			
			for(int i = 0; i < data.Length; i++)
			{
				if(kpos == Key.Length) kpos = 0;
				
				data[i] = (byte) (data[i] ^ Key[kpos]);
				
				kpos++;
			}
			
			return data;
		}

		public byte[] Decode(byte[] data)
		{
			return Encode(data);
		}
	}
}
