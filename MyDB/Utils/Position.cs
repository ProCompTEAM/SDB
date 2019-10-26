using System;

namespace MyDB.Utils
{
	public class Position
	{
		public readonly byte X, Y;
		
		public Position(byte offsetX, byte offsetY)
		{
			X = offsetX;
			Y = offsetY;
		}
		
		public Position(int offsetX, int offsetY)
		{
			if( offsetX >= 0 && offsetY <= 256 && offsetY >= 0 && offsetY <= 256 )
			{
				X = (byte) offsetX;
				Y = (byte) offsetY;
			}
			else throw new Exception("Позиция вышла за границы допустимого придела!");
		}
		
		public Address AsAddress(byte sectionId)
		{
			return new Address(sectionId, X, Y);
		}
		
		public Address AsAddress(Segmentation.Section section)
		{
			return new Address(section.Address, X, Y);
		}
	}
}
