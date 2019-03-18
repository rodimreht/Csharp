using System;
using System.Security.Cryptography;
using System.Text;

namespace PublicKey
{
	/// <summary>
	/// PKTest에 대한 요약 설명입니다.
	/// </summary>
	public class PKTest
	{
		public static void Run()
		{
			string exponent = "";
			string modulus = "";

			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			//rsa.PersistKeyInCsp = true;

			RSAParameters param = rsa.ExportParameters(false);
			exponent = getHexFromByte(param.Exponent);
			modulus = getHexFromByte(param.Modulus);
			Console.WriteLine("exponent: " + exponent);
			Console.WriteLine("modulus: " + modulus);

			//------------
			RSACryptoServiceProvider rsa2 = new RSACryptoServiceProvider();
			rsa2.ImportParameters(param);
			//rsa2.PersistKeyInCsp = true;

			byte[] data;
			byte[] enc;
			//------------
			RSAParameters param2 = rsa2.ExportParameters(false);
			exponent = getHexFromByte(param2.Exponent);
			modulus = getHexFromByte(param2.Modulus);
			Console.WriteLine("exponent: " + exponent);
			Console.WriteLine("modulus: " + modulus);

			string org = "test:1111";
			data = Encoding.Default.GetBytes(org);
			enc = rsa2.Encrypt(data, false);
			Console.WriteLine("encTemp: " + getHexFromByte(enc));
			enc = rsa2.Encrypt(data, false);
			Console.WriteLine("encTemp: " + getHexFromByte(enc));
			enc = rsa2.Encrypt(data, false);
			Console.WriteLine("encTemp: " + getHexFromByte(enc));
			enc = rsa2.Encrypt(data, false);
			Console.WriteLine("encTemp: " + getHexFromByte(enc));
			enc = rsa2.Encrypt(data, false);
			Console.WriteLine("encTemp: " + getHexFromByte(enc));
			//------------

			byte[] dec = rsa.Decrypt(enc, false);
			string decrypted = Encoding.Default.GetString(dec);
			Console.WriteLine("decrypted: " + decrypted);

			//------------
			RSAParameters param3 = rsa.ExportParameters(true);
			RSACryptoServiceProvider rsa3 = new RSACryptoServiceProvider();
			rsa3.ImportParameters(param3);

			dec = rsa3.Decrypt(enc, false);
			decrypted = Encoding.Default.GetString(dec);
			Console.WriteLine("decrypted: " + decrypted);
			//------------
			Console.WriteLine("done.");
		}

		public static void Run2()
		{
			try
			{        //initialze the byte arrays to the public key information.
				byte[] PublicKey = {214,46,220,83,160,73,40,39,201,155,19,202,3,11,191,178,56,
									   74,90,36,248,103,18,144,170,163,145,87,54,61,34,220,222,
									   207,137,149,173,14,92,120,206,222,158,28,40,24,30,16,175,
									   108,128,35,230,118,40,121,113,125,216,130,11,24,90,48,194,
									   240,105,44,76,34,57,249,228,125,80,38,9,136,29,117,207,139,
									   168,181,85,137,126,10,126,242,120,247,121,8,100,12,201,171,
									   38,226,193,180,190,117,177,87,143,242,213,11,44,180,113,93,
									   106,99,179,68,175,211,164,116,64,148,226,254,172,147};

				byte[] Exponent = {1,0,1};
      
				//Values to store encrypted data.
				byte[] data;
				byte[] encrypted;

				//Create a new instance of RSACryptoServiceProvider.
				RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

				//Create a new instance of RSAParameters.
				RSAParameters RSAKeyInfo = new RSAParameters();

				//Set RSAKeyInfo to the public key values. 
				RSAKeyInfo.Modulus = PublicKey;
				RSAKeyInfo.Exponent = Exponent;

				//Import key parameters into RSA.
				RSA.ImportParameters(RSAKeyInfo);

				//Encrypt the data.
				string org = "test:1111";
				data = Encoding.Default.GetBytes(org);

				for (int i = 0; i < 10; i++)
				{
					encrypted = RSA.Encrypt(data, false);
					Console.WriteLine("encrypted: " + getHexFromByte(encrypted)); 
				}
			}
			//Catch and display a CryptographicException  
			//to the console.
			catch(CryptographicException e)
			{
				Console.WriteLine(e.Message);
			}
			Console.WriteLine("done.");
		}

		private static string getHexFromByte(byte[] bBytes)
		{
			StringBuilder sb = new StringBuilder(bBytes.Length);
			for (int i = 0; i < bBytes.Length; i++)
			{
				if (bBytes[i].ToString("x").Length < 2)
					sb.Append("0" + bBytes[i].ToString("x"));
				else
					sb.Append(bBytes[i].ToString("x"));
			}
			return sb.ToString();
		}

		private static byte[] getHexArray(string sOrg)
		{
			byte[] bArray = null;

			// 기존 '+' 구분 대문자 암호화 문자열과의 호환성을 위해 유지
			if (sOrg.ToUpper().Replace(" ", "+").Replace("%2B", "+").IndexOf("+") > 0)
			{
				string[] sArray = sOrg.ToUpper().Split('+');
				bArray = new byte[sArray.Length];

				for (int i = 0; i < sArray.Length; i++)
				{
					bArray[i] = getByteFromHex(sArray[i]);
				}
			}
			else
			{
				int len = sOrg.Length / 2;
				bArray = new byte[len];
				for (int i = 0; i < len; i++)
				{
					bArray[i] = getByteFromHex(sOrg.Substring(i * 2, 2));
				}
			}
			return bArray;
		}

		private static byte getByteFromHex(string sHex)
		{
			char[] cTemp = sHex.ToUpper().ToCharArray();
			byte bTemp = 0;
			for (int k = 0; k < cTemp.Length; k++)
			{
				switch (k)
				{
					case 0:
						if (cTemp[k] >= 'A')
						{
							bTemp += (byte) ((cTemp[k] - 'A' + 10) * 16);
						}
						else
						{
							bTemp += (byte) (int.Parse(cTemp[k].ToString()) * 16);
						}
						break;

					case 1:
						if (cTemp[k] >= 'A')
						{
							bTemp += (byte) (cTemp[k] - 'A' + 10);
						}
						else
						{
							bTemp += (byte) int.Parse(cTemp[k].ToString());
						}
						break;

					default:
						bTemp = 0;
						break;
				}
			}
			return bTemp;
		}
	}
}
