using System;
using System.IO;

namespace MyDB.Segmentation.Meta
{
	public class MetaInfo
	{
		public const byte Version = 0x01;
		
		public byte Address;
		public string Label;
		
		internal byte[] Data
		{
			get 
			{
				MemoryStream stream = new MemoryStream();
				
				stream.WriteByte(Version);
				
				stream.WriteByte(Address);
				
				byte[] label = Database.CurrentDatabase.TextEncoding.GetBytes(Label);
				stream.Write(label, 0, label.Length);
				
				return stream.ToArray();
			}
		}
		
		public static MetaInfo ReadFrom(byte[] rawData)
		{
			MetaInfo info = new MetaInfo();
			
			MemoryStream stream = new MemoryStream(rawData);
			
			stream.Seek(0, SeekOrigin.Begin);
				
			byte version = (byte) stream.ReadByte();
			if(version != Version)
				throw new Exception("Версия метаданных секции не зарегистрирована!");
			
			info.Address = (byte) stream.ReadByte();
			
			int labelSize = rawData.Length - 2;
			byte[] label = new byte[labelSize];
			stream.Read(label, 0, labelSize);
			info.Label = Database.CurrentDatabase.TextEncoding.GetString(label);
				
			return info;
		}
	}
}
