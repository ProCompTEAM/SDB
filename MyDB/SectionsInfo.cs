using System;
using System.Collections.Generic;
using System.IO;

using MyDB.Utils;

namespace MyDB
{
	public class SectionsInfo
	{
		Database DB;
		
		readonly List<string> Sections = new List<string>();
		
		public SectionsInfo(Database db)
		{
			DB = db;
		}
		
		public string[] Signatures
		{
			get { return Sections.ToArray(); }
		}
		
		public void Update()
		{
			Sections.Clear();
			
			foreach(string file in DB.ResourcesSectionsFiles)
			{
				Sections.Add(Path.GetFileNameWithoutExtension(file));
			}
		}
		
		public byte[] GetIdsByLabel(string label)
		{
			List<byte> result = new List<byte>();
			
			Update();
			
			for(int i = 0; i < 256; i++)
			{
				string sign = Sign.CreateSectionSignature(Locale.ToUpper(label), (byte) i);
				
				if(Sections.IndexOf(sign) != -1) result.Add((byte) i);
			}
			
			return result.ToArray();
		}
	}
}
