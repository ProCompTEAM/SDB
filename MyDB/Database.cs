﻿using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using MyDB.Segmentation;
using MyDB.Utils;

namespace MyDB
{
	public class Database
	{
		public static Database CurrentDatabase;
		
		public const int Width = 16;
		public const int Height = 16;
		
		public const int FullstackBytes = 256 * 256 * 256;
		
		public const string IndexSectorLabel = "INDEX";
		
		public Encoding TextEncoding = Encoding.UTF8;
		
		public readonly string Name;
		public readonly string Resources;
		
		internal string RootDirectory = "";
		
		public const int BLOCK_DEEP_KB16 = 16000;
		public const int BLOCK_DEEP_KB32 = 32000;
		public const int BLOCK_DEEP_KB64 = 64000;
		public const int BLOCK_DEEP_KB128 = 128000;
		public const int BLOCK_DEEP_MB = 1024000;
		
		public readonly int MaxBlockDeep;
		
		public const string DB_SECTION_FFORMAT = "{filename}.dbv1s";
		
		public enum StorageMode
		{
			Memory,
			Stable,
			Safe
		}
		
		public readonly StorageMode Mode;
			
		public Database(string name, string resources = "C://", int maxDeepBlock = BLOCK_DEEP_KB64, 
		               StorageMode mode = StorageMode.Safe)
		{
			CurrentDatabase = this;
			
			Name = name;
			
			Resources = resources;
			
			MaxBlockDeep = maxDeepBlock;
			
			ObjectDB.InitializeDefaultTypes();
			
			MakeIndex();
			
			Mode = mode;
			
			Initialize();
		}
		
		private void Initialize()
		{	
			RootDirectory = Resources + @"\" + Name + @"\";
			
			if(Mode == StorageMode.Safe || Mode == StorageMode.Stable)
			{
				if(!Directory.Exists(RootDirectory))
					Directory.CreateDirectory(RootDirectory);
			}
		}
		
		public static Database FromResources(string name, string resources = "C://", int maxDeepBlock = BLOCK_DEEP_KB64, 
		               StorageMode mode = StorageMode.Safe)
		{
			Database db = new Database(name, resources, maxDeepBlock, mode);
			
			db.LoadIndex();
			
			return db;
		}
		
		int PenId = 0x00;
		Section Pen = null;
		
		public byte MakeSection(string label)
		{
			if(PenId >= Width * Height)
				throw new Exception("База данных переполнена!");
			
			if(Pen != null) Pen.Update();
			
			label = Locale.ToUpper(label);
			
			if(label == IndexSectorLabel && PenId != 0)
				throw new Exception("Системный сектор уже создан!");
			
			PenId++;
			
			byte pen = Convert.ToByte(PenId - 1);
			
			Pen = new Section(pen, label);
			
			return pen;
		}
		
		private void MakeIndex()
		{
			MakeSection(IndexSectorLabel);
		}
		
		public byte SectorAbsoluteAddress
		{
			get { return Pen.Address; }
		}
		
		public Section CurrentSection
		{
			get { return Pen; }
		}
		
		public Section LoadSection(string sourceFile)
		{
			Pen = Section.ReadSection(sourceFile);
			
			return CurrentSection;
		}
		
		public Section LoadSection(string label, byte penId)
		{
			string source = Database.CurrentDatabase.RootDirectory + 
				Database.DB_SECTION_FFORMAT.Replace("{filename}", Sign.CreateSectionSignature(label, penId));
			
			return LoadSection(source);
		}
		
		public string[] ResourcesSectionsFiles
		{
			get
			{
				List<string> SectionFiles = new List<string>();
				
				string[] files = Directory.GetFiles(RootDirectory);
				
				string ext = Path.GetExtension(DB_SECTION_FFORMAT);
				
				foreach(string file in files)
				{
					if(Path.GetExtension(file) == ext)
					{
						SectionFiles.Add(file);
					}
				}
				
				return SectionFiles.ToArray();
			}
		}
		
		public Section LoadIndex()
		{
			return LoadSection(IndexSectorLabel, 0x00);
		}
		
		public override string ToString()
		{
			return Name;
		}

	}
}