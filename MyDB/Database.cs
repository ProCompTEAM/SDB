using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using MyDB.Segmentation;
using MyDB.Segmentation.Data;
using MyDB.Utils;
using MyDB.Logger;

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
		
		public const string DB_SECTION_FFORMAT = "{filename}.dbv1s";
		
		public ContentLogger Logger;
		
		public enum StorageMode
		{
			Memory,
			Stable,
			Safe
		}
		
		public readonly StorageMode Mode;
			
		public Database(string name, string resources = "C://", 
		                StorageMode mode = StorageMode.Safe, ContentLogger logger = null)
		{
			Logger = logger;
			
			CurrentDatabase = this;
			
			Name = name;
			
			Resources = resources;
			
			ObjectDB.InitializeDefaultTypes();
			
			MakeIndex();
			
			Mode = mode;
			
			Initialize();
			
			if(Logger != null)
				Logger.Push(ContentLogger.EVENT_STARTPOINT, "Registered DB '" + Name + "' in '" + RootDirectory + "'");
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
		
		public static Database FromResources(string name, string resources = "C://", 
		               StorageMode mode = StorageMode.Safe)
		{
			Database db = new Database(name, resources, mode);
			
			db.LoadIndex();
			
			return db;
		}
		
		public void EnableDefaultLogger(string DefaultBuilderPassword = "default")
		{
			XorBuilder builder = new XorBuilder();
			builder.Key = Database.CurrentDatabase.TextEncoding.GetBytes(DefaultBuilderPassword);
			Logger = new ContentLogger(RootDirectory + "default.logdb", builder);
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
			label = Locale.ToUpper(label);
			
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
		
		public ObjectDB Select(Address address)
		{
			if(CurrentSection.Address != address.XX)
				throw new Exception("Обращение к заявленной секции невозможно!");
			
			return CurrentSection.Get(address.AsPosition());
		}
		
		public ObjectDB Select(Position position)
		{
			return Select(position.AsAddress(CurrentSection.Address));
		}
		
		public ObjectDB Select(byte ox, byte oy)
		{
			return Select(new Address(CurrentSection.Address, ox, oy));
		}

	}
}
