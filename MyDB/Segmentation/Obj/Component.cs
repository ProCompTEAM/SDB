using System;
using System.Reflection;

namespace MyDB.Segmentation.Obj
{
	public class Component : DataString
	{
		public Component()
		{
			Identifier = 0x06;
		}
		
		public string Run()
		{
			string text = this.ContentString;
			
			if(text[0] == '=')
			{
				switch(text.Substring(1))
				{
						case "time": return DateTime.Now.ToLongTimeString();
						case "date": return DateTime.Now.ToLongDateString();
						case "label": return Database.CurrentDatabase.CurrentSection.Label;
						case "name": return Database.CurrentDatabase.Name;
						case "count": return Database.CurrentDatabase.CurrentSection.Length.ToString();
				}
			}
			
			return "";
		}
		
		public override string Execute(string[] args)
		{
			return "";
		}
	}
}
