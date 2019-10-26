using System;

namespace MyDB.Segmentation.Data
{
	public interface DataBuilder
	{
		string UsePublicSignature();
		
		byte[] Encode(byte[] data);
		
		byte[] Decode(byte[] data);
	}
}
