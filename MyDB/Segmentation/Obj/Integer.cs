using System;

namespace MyDB.Segmentation.Obj
{
	public class Integer : ObjectDB
	{	
		public Integer()
		{
			Identifier = 0x02;
		}
		
		/*
		 * Argument1 > int32 : Content
		*/
		
		public override void Insert(params object[] args)
		{
			int data = (int) args[0];
			
			byte[] content = BitConverter.GetBytes(data);
			
			base.Content = content;
		}
		
		/*
		 * Argument1 > int32 : New Content
		*/
		
		public override void Update(params object[] args)
		{
			Insert(args);
		}
		
		public int ContentInt32
		{
			get { return BitConverter.ToInt32(Content, 0); }
		}
		
		public void Parse(string data)
		{
			Insert(int.Parse(data));
		}
		
		public override string Execute(string[] args)
		{
			return "";
		}
	}
}
