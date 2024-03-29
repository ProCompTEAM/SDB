﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using MyDB.Utils;
using MyDB.Segmentation.Data;
using MyDB.Segmentation.Meta;
using MyDB.Logger;

namespace MyDB.Segmentation
{
	public class Section
	{
		public const byte DB_SECTION_VERSION = 0x01;
		public const int Size = 256;
		
		protected MetaInfo Info = new MetaInfo();
		
		internal DataBuilder Builder;
		
		readonly ObjectDB[,] Content = new ObjectDB[Size, Size];
		
		public Section(byte address, string label, DataBuilder builder = null,
		              string DefaultBuilderPassword = "default")
		{
			Info.Address = address;
			Info.Label = label;
			
			for(int offsetX = 0; offsetX < Size; offsetX++)
				for(int offsetY = 0; offsetY < Size; offsetY++)
					Content[offsetX, offsetY] = null;
			
			if(builder == null) 
			{
				Builder = new XorBuilder();
				((XorBuilder) Builder).Key = Database.CurrentDatabase.TextEncoding.GetBytes(DefaultBuilderPassword);
			}
			
			if(Logger != null)
				Logger.Push(ContentLogger.EVENT_LOAD_S, "Section '" + Label + " loaded on " + Address);
		}
		
		~Section()
		{
			if(Logger != null)
				Logger.Push(ContentLogger.EVENT_UNLOAD_S, "Section '" + Label + " unloaded on " + Address);
		}
		
		public Address Set(ObjectDB obj, Position pos)
		{
			if(Content[pos.X, pos.Y] != null) Remove(pos);
			
			Content[pos.X, pos.Y] = obj;
			
			if(Logger != null)
				Logger.Push(ContentLogger.EVENT_SETUP, pos.AsAddress(Address).ToString());
			
			return new Address(Info.Address, pos.X, pos.Y);
		}
		
		public Address Set(ObjectDB obj, int x, int y)
		{
			return Set(obj, new Position(x, y));
		}
		
		public Address Add(ObjectDB obj)
		{
			Position pos = null;
			
			for(int offsetX = 0; offsetX < Size; offsetX++)
			{
				for(int offsetY = 0; offsetY < Size; offsetY++)
				{
					if(Content[offsetX, offsetY] == null)
					{
							pos = new Position(offsetX, offsetY);
							goto enf;
					}
				}
			}
			
			enf:
			
			if(pos == null) throw new Exception("Секция переполнена!");
			
			return Set(obj, pos);
		}
		
		public void AddRange(params ObjectDB[] objs)
		{
			foreach(ObjectDB o in objs) Add(o);
		}
		
		public void Remove(Position pos)
		{
			Content[pos.X, pos.Y] = null;
		}
		
		public void Update()
		{
			if(Database.CurrentDatabase.Mode == Database.StorageMode.Safe) 
			{
				Save();
			}
		}
		
		public string Label
		{
			get { return Info.Label; }
		}
		
		public byte Address
		{
			get { return Info.Address; }
		}
		
		/*
		 * Data Format: <VER-DB><DEEP-METAINFO><METAINFO><1=ID><1=X><1=Y><4=DEEP-LABEL><N=DATA-LABEL><4=DEEP-CONTENT><N=CONTENT>
		*/
		
		private void Save()
		{
			string source = Database.CurrentDatabase.RootDirectory + 
				Database.DB_SECTION_FFORMAT.Replace("{filename}", Sign.CreateSectionSignature(Info.Label, Info.Address));
			
			ObjectDB current;
			byte[] label;
			
			using (BinaryWriter writer = new BinaryWriter(File.Open(source, FileMode.Create)))
			{
				writer.Write(DB_SECTION_VERSION);
				
				//DATABASE TITLE (METAINFO)				
				byte[] meta = Builder.Encode(Info.Data);
				writer.Write(meta.Length);
				writer.Write(meta);
				
				//BLOCKS OF DATA
				for(int offsetX = 0; offsetX < 256; offsetX++)
					for(int offsetY = 0; offsetY < 256; offsetY++)
						if(Content[offsetX, offsetY] != null)
				{
					current = Content[offsetX, offsetY];
					
					writer.Write(current.Id);
					writer.Write(offsetX);
					writer.Write(offsetY);
					
					label = Builder.Encode(Database.CurrentDatabase.TextEncoding.GetBytes(current.Label));
					writer.Write(label.Length);
					writer.Write(label);
					
					byte[] currdat = Builder.Encode(current.Content);
					writer.Write(currdat.Length);
					writer.Write(currdat);
				}
			}
			
			if(Logger != null)
				Logger.Push(ContentLogger.EVENT_SAVED_S, "Section '" + Label + " saved on " + Address);
		}
		
		public static Section ReadSection(string sourceFile, DataBuilder builder = null, string DefaultBuilderPassword = "default")
		{
			if(builder == null) 
			{
				builder = new XorBuilder();
				((XorBuilder) builder).Key = Database.CurrentDatabase.TextEncoding.GetBytes(DefaultBuilderPassword);
			}
			
			Section section;
			int size;
			
			using (BinaryReader reader = new BinaryReader(File.Open(sourceFile, FileMode.Open)))
            {
				//READING METAINFO
                byte section_version = reader.ReadByte();
                	
                if(section_version != DB_SECTION_VERSION)
                	throw new Exception("Секция повреждена или недоступна!");
                	
                size = reader.ReadInt32();
                byte[] metadata = reader.ReadBytes(size);
                MetaInfo info = MetaInfo.ReadFrom(builder.Decode(metadata));
                
                section = new Section(info.Address, info.Label, builder);
				
                int sz;
                byte[] data;
                
                while(reader.PeekChar() > -1)
                {
                	byte objectId = reader.ReadByte();
                	ObjectDB obj = (ObjectDB) Activator.CreateInstance(ObjectDB.RestoreObject(objectId));
                	
                	int ox = reader.ReadInt32();
                	int oy = reader.ReadInt32();
                	
                	sz = reader.ReadInt32();
                	data = builder.Decode(reader.ReadBytes(sz));
                	
                	obj.Label = Database.CurrentDatabase.TextEncoding.GetString(data);
                	
                	sz = reader.ReadInt32();
                	data = builder.Decode(reader.ReadBytes(sz));
                	
                	obj.Content = data;
                	
                	section.Set(obj, new Position(ox, oy));
                }
            }
			
			section.Builder = builder;
			
			return section;
		}
		
		public ObjectDB Get(Position pos)
		{
			return Content[pos.X, pos.Y];
		}
		
		public ObjectDB Get(int x, int y)
		{
			return Content[x, y];
		}
		
		public override string ToString()
		{
			return Info.Label;
		}
		
		public ObjectDB[] AsList()
		{
			List<ObjectDB> list = new List<ObjectDB>();
			
			for(int offsetX = 0; offsetX < Size; offsetX++)
				for(int offsetY = 0; offsetY < Size; offsetY++)
					if(Content[offsetX, offsetY] != null) list.Add(Content[offsetX, offsetY]);
			
			return list.ToArray();
		}
		
		public ObjectDB[] AsList(Position[] positions)
		{
			List<ObjectDB> list = new List<ObjectDB>();
			
			foreach(Position pos in positions)
				list.Add(Get(pos));
			
			return list.ToArray();
		}
		
		public ContentLogger Logger
		{
			get { return Database.CurrentDatabase.Logger; }
		}
		
		public int Length
		{
			get 
			{
				int l = 0;
				
				for(int offsetX = 0; offsetX < Size; offsetX++)
					for(int offsetY = 0; offsetY < Size; offsetY++)
						if(Content[offsetX, offsetY] != null) l++;	
				
				return l;
			}
		}
		
		public bool Full
		{
			get
			{
				bool full = true;
				
				for(int offsetX = 0; offsetX < Size; offsetX++)
					for(int offsetY = 0; offsetY < Size; offsetY++)
						if(Content[offsetX, offsetY] == null) full = false;
				
				return full;
			}
		}
		
		public ObjectDB[] Range(Position pos1, Position pos2)
		{
			if(pos1.Average > pos2.Average) throw new Exception("Позиции за пределами допустимых значений");
			
			List<ObjectDB> list = new List<ObjectDB>();
			
			for(int offsetX = pos1.X; offsetX < pos2.X; offsetX++)
				for(int offsetY = pos1.Y; offsetY < pos2.Y; offsetY++)
					if(Content[offsetX, offsetY] != null) list.Add(Content[offsetX, offsetY]);
			
			return list.ToArray();
		}
		
		public ObjectDB[] Range(int xFrom, int yFrom, int xTo, int yTo)
		{
			return Range(new Position(xFrom, yFrom), new Position(xTo, yTo));
		}
	}
}
