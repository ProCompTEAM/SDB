using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyDB.Segmentation.Obj
{
	public class Structure : ObjectDB
	{
		public Structure()
		{
			Identifier = 0x03;
		}
		
		/*
		 * Argument1 > [Serializable] OBJECT : Content
		*/
		
		public override void Insert(params object[] args)
		{
			object data = args[0];
			
			byte[] content = ObjectToByteArray(data);
			
			base.Content = content;
		}
		
		/*
		 * Argument1 > [Serializable] OBJECT : New Content
		*/
		
		public override void Update(params object[] args)
		{
			Insert(args);
		}
		
		public Object ContentData
		{
			get { return ByteArrayToObject(Content); }
		}
		
		public override string Execute(string[] args)
		{
			return "";
		}
		
		byte[] ObjectToByteArray(object obj)
		{
			if(obj == null) return new byte[1] { 0x00 };
			
		    BinaryFormatter bf = new BinaryFormatter();
		    
		    using (MemoryStream ms = new MemoryStream())
		    {
		        bf.Serialize(ms, obj);
		        return ms.ToArray();
		    }
		}
		
		Object ByteArrayToObject(byte[] arrBytes)
		{
		    MemoryStream memStream = new MemoryStream();
		    BinaryFormatter binForm = new BinaryFormatter();
		    
		    memStream.Write(arrBytes, 0, arrBytes.Length);
		    memStream.Seek(0, SeekOrigin.Begin);
		    
		    Object obj = (Object) binForm.Deserialize(memStream);
		
		    return obj;
		}
	}
}
