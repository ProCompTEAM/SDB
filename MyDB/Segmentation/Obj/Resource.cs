using System;
using System.Collections.Generic;
using System.IO;

namespace MyDB.Segmentation.Obj
{
	public class Resource : ObjectDB
	{
		public Resource()
		{
			Identifier = 0x05;
		}
		
		/*
		 * Arguments* > BytesArray { byte, byte .. byte } : Content
		*/
		
		public override void Insert(params object[] args)
		{
			List<byte> bytes = new List<byte>();
			
			foreach(byte b in args) bytes.Add(b);
			
			Content = bytes.ToArray();
		}
		
		/*
		 * Arguments* > BytesArray { byte, byte .. byte } : New Content
		*/
		
		public override void Update(params object[] args)
		{
			Insert(args);
		}
		
		public byte[] ContentData
		{
			get { return Content; }
		}
		
		public void From(byte[] data)
		{
			List<byte> bytes = new List<byte>();
			
			foreach(byte b in data) bytes.Add(b);
			
			Content = bytes.ToArray();
		}
		
		public void FromFile(string path)
		{
			From(File.ReadAllBytes(path));
		}
		
		public void FromStream(Stream stream, int offset = 0)
		{
			byte[] data = new byte[stream.Length - offset];
			
			stream.Read(data, offset, data.Length);
			
			From(data);
		}
		
		public MemoryStream Stream
		{
			get { return new MemoryStream(Content); }
		}
		
		public byte ReadByte(int offset = 0)
		{
			return Content[offset];
		}
		
		public byte[] Read(int count, int offset = 0)
		{
			byte[] result = new byte[count];
			
			this.Stream.Read(result, offset, count);
			
			return result;
		}
		
		public bool Boolean
		{
			set
			{
				if(Content.Length < 1) Content[0] = new byte();
				
				Content[0] = (byte) (value ? 255 : 0);
			}
			
			get { return (Content.Length > 0 && Content[0] == 255); }
		}
		
		public override string Execute(string[] args)
		{
			return "";
		}
	}
}
