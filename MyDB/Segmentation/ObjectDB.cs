using System;
using System.Collections.Generic;

using MyDB.Utils;
using MyDB.Segmentation.Obj;

namespace MyDB.Segmentation
{
	public abstract class ObjectDB
	{
		protected Address Address;
		
		public byte Identifier;
		
		internal byte[] Content;
		
		public string Label = string.Empty;
		
		public int BytesCount;
		
		static readonly List<ObjectDB> Objects = new List<ObjectDB>();
		
		public byte Id
		{
			get { return Identifier; }
		}
		
		public abstract void Insert(params object[] data);
		
		public abstract void Update(params object[] data);
		
		public abstract string Execute(string[] args);
		
		protected void CreateDeepException()
		{
			throw new Exception("Размер внесенных данных привышает установленный лимит!");
		}
		
		public static void RegisterObject(ObjectDB obj)
		{
			Objects.Add(obj);
		}
		
		public static void UnregisterObject(ObjectDB obj)
		{
			Objects.Remove(obj);
		}
		
		public static Type RestoreObject(byte objectId)
		{
			foreach(ObjectDB obj in Objects) 
				if(obj.Id == objectId) return obj.GetType();
			
			return null;
		}
		
		static int InitLastId = 0;
		
		public static int LastDefaultId
		{
			get { return InitLastId; }
		}
		
		public static void InitializeDefaultTypes()
		{
			Objects.Clear();
			
			RegisterObject(new DataString());
			RegisterObject(new Integer());
			RegisterObject(new Structure());
			RegisterObject(new Group());
			RegisterObject(new Resource());
			RegisterObject(new Component());
			
			foreach(ObjectDB obj in Objects)
				if(obj.Id > InitLastId) InitLastId = obj.Id;
		}
	}
}
