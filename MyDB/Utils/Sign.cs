using System;
using System.Security.Cryptography;
using System.Text;

namespace MyDB.Utils
{
	public static class Sign
	{
		static readonly SHA256 Crypt = new SHA256Managed();
		
		public static string CreateSectionSignature(string name, byte penId)
		{
			int pid = (int) penId;
			string x = name + penId.ToString();
				
			return Sha256(x);
		}
		
		static string Sha256(string randomString)
		{
		    string hash = String.Empty;
		    byte[] crypto = Crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
		    
		    foreach (byte theByte in crypto)
		    {
		        hash += theByte.ToString("x2");
		    }
		    return hash;
		}
	}
}
