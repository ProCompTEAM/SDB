using System;

using System.IO;
using MyDB.Segmentation;
using MyDB.Segmentation.Obj;
using MyDB.Utils;

namespace MyDB
{
	class Program
	{
		public static void Main(string[] args)
		{	
			Console.ReadKey();
		}
		
		static void Test1()
		{
			Database db = new Database("MyDatabase", "D://");
			byte adr = db.MakeSection("ABC");
			
			for(int i = 1; i <= 256; i++)
			{
				DataString s = new DataString();
				s.Label = "Объект " + i.ToString();
				s.Insert("[Данные удалены]");
				Console.WriteLine("ok = " + db.CurrentSection.Add(s).ToString());
			}
			
			Console.WriteLine("saving...");
			
			db.CurrentSection.Update();
			
			Console.WriteLine("! saved.");
		}
		
		static void Test2()
		{
			Database db = Database.FromResources("MyDatabase", "D://");
 			SectionsInfo si = new SectionsInfo(db);
 			db.LoadSection("ABC", si.GetIdsByLabel("ABC")[0]);
 			
 			foreach(DataString str in db.CurrentSection.AsList())
 			{
 				Console.WriteLine(str.Label + " > " + str.ContentString);
 			}
		}
		
		//===var 2===
		static void Test3()
		{
			Database db = new Database("MyDatabase", "D://");
			byte adr = db.MakeSection("Дампы времени");
			
			DateTime time;
			
			for(int i = 1; i <= 30000; i++)
			{
				time = DateTime.Now;
				
				Integer integer = new Integer();
				integer.Label = "Дамп от " + time.ToString();
				integer.Insert(i);
				Console.WriteLine("ok = " + db.CurrentSection.Add(integer).ToString());
			}
			
			Console.WriteLine("saving...");
			
			db.CurrentSection.Update();
			
			Console.WriteLine("! saved.");
		}
		
		static void Test4()
		{
			Database db = Database.FromResources("MyDatabase", "D://");
 			SectionsInfo si = new SectionsInfo(db);
 			db.LoadSection("Дампы времени", si.GetIdsByLabel("Дампы времени")[0]);
 			
 			foreach(Integer i in db.CurrentSection.AsList())
 			{
 				Console.WriteLine(i.Label + " > " + i.ContentInt32);
 			}
		}
		
		//===var 3===
		
		[Serializable]
		struct TestSctruct
		{
			public string PlayerName;
			public DateTime CreationTime;
			public double Z, Y, X;
		}
		
		static void Test5()
		{
			Database db = new Database("MyDatabase", "D://");
			byte adr = db.MakeSection("Хранение объектов");
			
			TestSctruct ts;
			
			for(int i = 1; i <= 10000; i++)
			{
				ts = new TestSctruct();
				if(i % 2 == 0) ts.PlayerName = "Steve";
				else ts.PlayerName = "Alex";
				ts.X = i / 2;
				ts.Y = i / 4;
				ts.Z = i / 6;
				ts.CreationTime = DateTime.Now;
				
				Structure integer = new Structure();
				integer.Label = "Структура №" + (i + 1);
				integer.Insert(ts);
				Console.WriteLine("ok = " + db.CurrentSection.Add(integer).ToString());
			}
			
			Console.WriteLine("saving...");
			
			db.CurrentSection.Update();
			
			Console.WriteLine("! saved.");
		}
		
		static void Test6()
		{
			Database db = Database.FromResources("MyDatabase", "D://");
 			SectionsInfo si = new SectionsInfo(db);
 			db.LoadSection("Хранение объектов", si.GetIdsByLabel("Хранение объектов")[0]);
 			
 			TestSctruct ts;
 			
 			foreach(Structure i in db.CurrentSection.AsList())
 			{
 				ts = (TestSctruct) i.ContentData;
 				
 				Console.WriteLine(i.Label + " > " + ts.PlayerName + " " + ts.CreationTime.ToString() + 
 				                  " " + ts.X + " " + ts.Y + " " + ts.Z);
 			}
		}
		
		//===var 4=== GROUPS ===
		static Position grp;
		static void Test7()
		{
			Database db = new Database("MyDatabase", "D://");
			byte adr = db.MakeSection("Тест группировки");
			
			Group group1 = new Group();
			Group group2 = new Group();
			Group group3 = new Group();
			Group group4 = new Group();
			
			db.CurrentSection.AddRange(group1, group2, group3);
			group4.Label = "НазваниеГруппы";
			grp = db.CurrentSection.Add(group4).AsPosition();
			Console.WriteLine(grp.AsAddress(db.CurrentSection));
			
			for(int i = 1; i <= 400; i++)
			{
				DataString s = new DataString();
				s.Label = "Объект " + i.ToString();
				s.Insert("Был создан объект с этой строкой");
				
				if(i >= 0 && i < 100) group1.Insert(db.CurrentSection.Add(s).AsPosition());
				if(i >= 100 && i < 200) group2.Insert(db.CurrentSection.Add(s).AsPosition());
				if(i >= 200 && i < 300) group3.Insert(db.CurrentSection.Add(s).AsPosition());
				if(i >= 300 && i < 400) group4.Insert(db.CurrentSection.Add(s).AsPosition());
			}
			
			Console.WriteLine("saving...");
			
			db.CurrentSection.Update();
			
			Console.WriteLine("! saved.");
		}
		
		static void Test8()
		{
			//need Test7() before run
			Database db = Database.FromResources("MyDatabase", "D://");
 			SectionsInfo si = new SectionsInfo(db);
 			db.LoadSection("Тест группировки", si.GetIdsByLabel("Тест группировки")[0]);
 			
 			Group group = (Group) db.CurrentSection.Get(grp);
 			Console.WriteLine(group.Label);
 			foreach(DataString str in db.CurrentSection.AsList(group.ContentLinks))
 				Console.WriteLine(str.Label + " > " + str.ContentString);
		}
		
		//===var 5=== Resources ===
		static void Test9()
		{
			Database db = new Database("MyDatabase", "D://");
			db.MakeSection("-= <= RESOURCES => =-");
			
			Resource resource = new Resource();
			resource.FromFile("D://MyDatabase/testImage.png");
			
			db.CurrentSection.Add(resource);
			
			Console.WriteLine("saving...");
			
			db.CurrentSection.Update();
			
			Console.WriteLine("! saved.");
		}
		
		static void Test10()
		{
			Database db = Database.FromResources("MyDatabase", "D://");
 			SectionsInfo si = new SectionsInfo(db);
 			db.LoadSection("-= <= RESOURCES => =-", si.GetIdsByLabel("-= <= RESOURCES => =-")[0]);
 			
 			Resource resource = (Resource) db.CurrentSection.Get(new Position(0, 0));
 			File.WriteAllBytes("D://MyDatabase/resultImage.png", resource.ContentData);
		}
		
		//===var 6=== Components ===
		static void Test11()
		{
			Database db = new Database("MyDatabase", "D://");
			db.MakeSection("Компоненты");
			
			Component com1 = new Component();
			com1.Insert("=time");
			Component com2 = new Component();
			com2.Insert("=date");
			Component com3 = new Component();
			com3.Insert("=label");
			Component com4 = new Component();
			com4.Insert("=name");
			Component com5 = new Component();
			com5.Insert("=count");
			
			db.CurrentSection.AddRange(com1, com2, com3, com4, com5);
			
			Console.WriteLine("saving...");
			
			db.CurrentSection.Update();
			
			Console.WriteLine("! saved.");
		}
		
		static void Test12()
		{
			Database db = Database.FromResources("MyDatabase", "D://");
 			SectionsInfo si = new SectionsInfo(db);
 			db.LoadSection("Компоненты", si.GetIdsByLabel("Компоненты")[0]);
 			
 			Component com1 = (Component) db.CurrentSection.Get(new Position(0, 0));
 			Component com2 = (Component) db.CurrentSection.Get(new Position(0, 1));
 			Component com3 = (Component) db.CurrentSection.Get(new Position(0, 2));
 			Component com4 = (Component) db.CurrentSection.Get(new Position(0, 3));
 			Component com5 = (Component) db.CurrentSection.Get(new Position(0, 4));
 			
 			Console.WriteLine("{0}, {1}, {2}, {3}, {4}", com1.Run(), com2.Run(), com3.Run(), com4.Run(), com5.Run());
		}
		
		//===var 7=== DataText : Test Lines Stack ===
		static void Test13()
		{
			Database db = new Database("MyDatabase", "D://");
			byte adr = db.MakeSection("Тексты");
			
			DataString s = new DataString();
			s.Label = "Пожилая чешуя";
			
			for(int i = 1; i <= 256; i++)
			{
				s.AddLine("Строка №" + i);
			}
			
			db.CurrentSection.Add(s);
			
			Console.WriteLine("saving...");
			
			db.CurrentSection.Update();
			
			Console.WriteLine("! saved.");
		}
		
		static void Test14()
		{
			Database db = Database.FromResources("MyDatabase", "D://");
 			SectionsInfo si = new SectionsInfo(db);
 			db.LoadSection("Тексты", si.GetIdsByLabel("Тексты")[0]);
 			Console.WriteLine(((DataString) db.Select(0,0)).ContentString);
		}
	}
}