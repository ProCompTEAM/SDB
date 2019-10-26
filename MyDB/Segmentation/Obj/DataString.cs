using System;

using MyDB.Utils;

namespace MyDB.Segmentation.Obj
{
	public class DataString : ObjectDB
	{	
		public DataString()
		{
			Identifier = 0x01;
		}
		
		/*
		 * Argument1 > string : Content
		*/
		
		public override void Insert(params object[] args)
		{
			string data = (string) args[0];
			
			byte[] content = Database.CurrentDatabase.TextEncoding.GetBytes(data);
			
			base.Content = content;
		}
		
		/*
		 * Argument1 > string : New Content
		*/
		
		public override void Update(params object[] args)
		{
			Insert(args);
		}
		
		public string ContentString
		{
			get { return Database.CurrentDatabase.TextEncoding.GetString(Content); }
		}
		
		public override string Execute(string[] args)
		{
			return "";
		}
		
		public void AddLine(string line)
		{
			if(Content == null || Content.Length < 1) Insert(line);
			else Insert(ContentString + Environment.NewLine + line);
		}
	}
}
