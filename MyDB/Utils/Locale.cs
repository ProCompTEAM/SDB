using System;

namespace MyDB.Utils
{
	public static class Locale
	{
		const string MinA = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяabcdefghijklmnopqrstuvwxyz";
		const string MaxA = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯABCDEFGHIJKLMNOPQRSTUVWXYZ";
		
		public static string ToUpper(string str) 
		{
			string result = string.Empty;
			
			for(int i = 0; i < str.Length; i++)
			{
				char x = str[i];
				
				int pos = MinA.IndexOf(x);
				
				if(pos == -1) result += x;
				else result += MaxA.Substring(pos, 1);
			}
			
			return result;
		} 
		
		public static string ToLower(string str) 
		{
			string result = string.Empty;
			
			for(int i = 0; i < str.Length; i++)
			{
				char x = str[i];
				
				int pos = MaxA.IndexOf(x);
				
				if(pos == -1) result += x;
				else result += MinA.Substring(pos, 1);
			}
			
			return result;
		} 
	}
}
