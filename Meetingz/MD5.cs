using System;
using System.IO;
using System.Security.Cryptography;

namespace Meetingz
{
	/// <summary>
	/// Hashes the Password to store in DB
	/// for Privacy.Even System Administrators cant know Users Password
	/// </summary>
	public class HashFx
	{	
		public HashFx()
		{
			
		}
		/// <summary>
		/// returns MD5 hash of a string :-)
		/// </summary>
		/// <param name="strToEncrypt"></param>
		/// <returns></returns>
		public string EncryptString(string strToEncrypt)
		{
			var ue = new System.Text.UTF8Encoding();
			var bytes = ue.GetBytes(strToEncrypt);
  
			// encrypt bytes
			var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			var hashBytes = md5.ComputeHash(bytes);
  
			// Convert the encrypted bytes back to a string (base 16)
			var hashString = "";
  
			for(var i=0;i<hashBytes.Length;i++)
			{
				hashString += Convert.ToString(hashBytes[i],16).PadLeft(2,'0');
			}
  
			return hashString.PadLeft(32,'0');
		}
	
		/// <param name="strToEncryp">the string to hash</param>
		/// <param name="Algorithm">the Algorithm of choice. 0 for MD5, 1 for SHA-1</param>
		/// <returns></returns>
		public string EncryptString(string strToEncryp, int Algorithm)
		{
			if(Algorithm ==1)
			{
				var ue = new System.Text.UTF8Encoding();
				var bytes = ue.GetBytes(strToEncryp);
  
				// encrypt bytes
				var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				var sha = new SHA1CryptoServiceProvider();
				var hashBytes = sha.ComputeHash(bytes);
  
				// Convert the encrypted bytes back to a string (base 16)
				var hashString = "";
  
				for(var i=0;i<hashBytes.Length;i++)
				{
					hashString += Convert.ToString(hashBytes[i],16).PadLeft(2,'0');
				}
  
				return hashString.PadLeft(32,'0');
			}
			else
			{
				return EncryptString(strToEncryp);
			}
			return null;
		}
		/// <summary>
		/// converts the MD5 of SHA-1 result byte[] to a string representative
		/// </summary>
		/// <param name="RawStringBytes">the Raw bytes to hash</param>
		/// <returns></returns>
		public string EncryptString(byte[] RawStringBytes)
		{	
			 
				var hashString ="";
				for(var i=0;i<RawStringBytes.Length;i++)
				{
					hashString += Convert.ToString(RawStringBytes[i],16).PadLeft(2,'0');
				}
  
				return hashString.PadLeft(32,'0');
		
		}
		/// <summary>
		/// For MD5 hashing of files
		/// </summary>
		/// <param name="filepath">the file path of the file to hash</param>
		/// <returns></returns>
		public string Md5File(string filepath)
		{
			var filestrm = new FileStream(filepath,FileMode.Open);
			var md5Byte = new byte[filestrm.Length];
				
			filestrm.Read(md5Byte,0,Convert.ToInt32(filestrm.Length.ToString()));
			var resultHash = HashByte(md5Byte);
			
			
			var hashString = "";
  
			for(var i=0;i<resultHash.Length;i++)
			{
				hashString += Convert.ToString(resultHash[i],16).PadLeft(2,'0');
			}
			filestrm.Close(); 
			return hashString.PadLeft(32,'0');

		}

	


		public byte[] HashByte(byte[] bytes)
		{
			 
			// encrypt bytes
			var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			var hashBytes = md5.ComputeHash(bytes);
  
			// Convert the encrypted bytes back to a string (base 16)
			
			return hashBytes;
		}
		
		public byte[] HashByte(byte[] bytes, int Algorithm)
		{
				byte[] hashBytes=null;

			if(Algorithm ==0)//MD5
			{
			
				var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				hashBytes = md5.ComputeHash(bytes);
			}
			else if (Algorithm == 1)//SHA-1
			{//SHA-1
				// Convert the encrypted bytes back to a string (base 16)
				var shs = new SHA1CryptoServiceProvider();
				hashBytes = shs.ComputeHash(bytes);
			}
			else
			{
				return null;
			}
			return hashBytes;
		}
		
	}
}
