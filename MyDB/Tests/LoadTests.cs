using System;
using MyDB.Segmentation.Obj;
using MyDB.Utils;

namespace MyDB.Tests
{
	public static class LoadTests
	{
		public static string Directory = "D://";
		public static string NameDB = "DB";
		
		public static void Test1()
		{
			Database db = new Database(NameDB, Directory);
			db.EnableDefaultLogger();
			
			db.MakeSection("Test1");
			
			while(!db.CurrentSection.Full)
			{
				Integer integer = new Integer();
				integer.Insert(DateTime.Now.Millisecond);
				db.CurrentSection.Add(integer);
			}
			
			db.CurrentSection.Update();
			
			Console.WriteLine("OK");
		}
		
		public static void Test2()
		{
			Database db = Database.FromResources(NameDB, Directory);
			db.EnableDefaultLogger();
			
			db.LoadSection("Test1", new SectionsInfo(db).GetIdsByLabel("Test1")[0]);

			foreach(Integer integer in db.CurrentSection.Range(0, 0, 40, 40))
			{
				Console.WriteLine(" - " + integer.ContentInt32); 
			}
			
			foreach(Position pos in Position.Range(20, 20, 40, 40))
			{
				Integer i = new Integer();
				i.Insert(100000000);
				db.CurrentSection.Set(i, pos);
			}
			
			db.CurrentSection.Update();
		}
	}
}
