using System;

using System.Collections.Generic;
using System.IO;
using MyDB.Segmentation.Data;

namespace MyDB.Logger
{
	public class ContentLogger
	{
		public readonly string Source;
		
		readonly DataBuilder Builder;
		readonly BinaryWriter Writer;
		
		public ContentLogger(string sourceFile, DataBuilder builder)
		{
			Source = sourceFile;
			Builder = builder;
			
			Writer = new BinaryWriter(File.Open(Source, FileMode.Append));
		}
		
		~ContentLogger()
		{
			Writer.Close();
		}
		
		public const byte EVENT_STARTPOINT = 0x00;
		public const byte EVENT_MESSAGE = 0x01;
		public const byte EVENT_LOAD_S = 0x02;
		public const byte EVENT_UNLOAD_S = 0x03;
		public const byte EVENT_SETUP = 0x04;
		public const byte EVENT_INSERT = 0x05;
		public const byte EVENT_UPDATE = 0x06;
		public const byte EVENT_DELETE = 0x07;
		public const byte EVENT_SAVED_S = 0x08;
		
		public readonly List<byte> Ignored = new List<byte>();
		
		public void Push(byte evt, string data)	
		{
			if(Ignored.Contains(evt)) return;
			
			long dt = DateTime.UtcNow.ToBinary();
			byte[] msg = Database.CurrentDatabase.TextEncoding.GetBytes(data);
			msg = Builder.Encode(msg);
			int count = msg.Length;
			
			Writer.Write(evt);
			Writer.Write(dt);
			Writer.Write(count);
			Writer.Write(msg);
		}
	}
}
