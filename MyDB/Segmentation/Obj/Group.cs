using System;
using System.IO;
using System.Collections.Generic;

using MyDB.Utils;

namespace MyDB.Segmentation.Obj
{
	public class Group : ObjectDB
	{
		public Group()
		{
			Identifier = 0x04;
		}
		
		List<Position> Links = new List<Position>();
		
		/*
		 * Argument1 > Position : Content
		*/
		
		public override void Insert(params object[] args)
		{
			Position adr = (Position) args[0];
			
			Links.Add(adr);
			
			base.Content = ToBytes();
		}
		
		/*
		 * Argument1 > Position : New Content
		*/
		
		public override void Update(params object[] args)
		{
			Links.Clear();
			
			Insert(args);
		}
		
		public void Clear()
		{
			Links.Clear();
		}
		
		public Position[] ContentLinks
		{
			get 
			{
				Position[] ls = ToLinks();
				
				Links.Clear();
				Links.AddRange(ls);
				
				return ls;
			}
		}
		
		public Position[] ContentBody
		{
			get { return Links.ToArray(); }
		}
		
		public int Length
		{
			get { return Links.Count; }
		}
		
		public bool Unset(Position pos)
		{
			foreach(Position cpos in Links)
			{
				if(pos == cpos) 
				{
					Links.Remove(pos);
					
					base.Content = ToBytes();
					
					return true;
				}
			}
			
			return false;
		}
		
		public override string Execute(string[] args)
		{
			return "";
		}
		
		byte[] ToBytes()
		{
			byte[] data = new byte[Links.Count * 2];
			
			int index = 0;
			
			foreach(Position pos in Links)
			{
				data[index] = pos.X;
				data[index + 1] = pos.Y;
				index += 2;
			}
			
			return data;
		}
		
		Position[] ToLinks()
		{
			List<Position> links = new List<Position>();
			
			Position pos;
			
			for(int pen = 0; pen < Content.Length; pen += 2)
			{
				pos = new Position(Content[pen], Content[pen + 1]);
				
				links.Add(pos);
			}
			
			return links.ToArray();
		}
	}
}
