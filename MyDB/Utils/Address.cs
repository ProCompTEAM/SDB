using System;

namespace MyDB.Utils
{
	public class Address
	{
		public readonly byte XX, YY, ZZ;
		
		public Address(byte sector, byte offsetX, byte offsetY)
		{
			XX = sector;
			YY = offsetX;
			ZZ = offsetY;
		}
		
		public Address(int sector, int offsetX, int offsetY)
		{
			if(
				sector >= 0 && sector <= 256 &&
				offsetX >= 0 && offsetY <= 256 &&
				offsetY >= 0 && offsetY <= 256
			)
			{
				XX = (byte) sector;
				YY = (byte) offsetX;
				ZZ = (byte) offsetY;
			}
			else throw new Exception("Значение адреса указано неверно!");
		}
		
		public override string ToString()
		{
			return string.Format("[{0}{1}{2}]", Str(XX), Str(YY), Str(ZZ));
		}
		
		private string Str(byte b)
		{
			return b.ToString("X2");
		}
		
		public Position AsPosition()
		{
			return new Position(YY, ZZ);
		}
	}
}
