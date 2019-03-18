using System;
using System.Text;
using System.Security.Cryptography;

namespace Crypto
{
	class Program
	{
		static void Main()
		{
			Console.WriteLine("ASP.NET 1.1 MachineKey:");
			Console.WriteLine(Generator.GetASPNET11machinekey());
			Console.WriteLine("ASP.NET 2.0 MachineKey:");
			Console.WriteLine(Generator.GetASPNET20machinekey());
		}
	}

	class Generator
	{
		public static string GetASPNET20machinekey()
		{
			StringBuilder aspnet20machinekey = new StringBuilder();
			string key64byte = getRandomKey(64);
			string key32byte = getRandomKey(32);
			aspnet20machinekey.Append("<machineKey \r\n");
			aspnet20machinekey.Append("validationKey=\"" + key64byte + "\"\r\n");
			aspnet20machinekey.Append("decryptionKey=\"" + key32byte + "\"\r\n");
			aspnet20machinekey.Append("validation=\"SHA1\" decryption=\"AES\"\r\n");
			aspnet20machinekey.Append("/>\r\n");
			return aspnet20machinekey.ToString();
		}

		public static string GetASPNET11machinekey()
		{
			StringBuilder aspnet11machinekey = new StringBuilder();
			string key64byte = getRandomKey(64);
			string key24byte = getRandomKey(24);

			aspnet11machinekey.Append("<machineKey ");
			aspnet11machinekey.Append("validationKey=\"" + key64byte + "\"\r\n");
			aspnet11machinekey.Append("decryptionKey=\"" + key24byte + "\"\r\n");
			aspnet11machinekey.Append("validation=\"SHA1\"\r\n");
			aspnet11machinekey.Append("/>\r\n");
			return aspnet11machinekey.ToString();
		}

		private static string getRandomKey(int bytelength)
		{
			byte[] buff = new byte[bytelength];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(buff);
			StringBuilder sb = new StringBuilder(bytelength * 2);
			for (int i = 0; i < buff.Length; i++)
				sb.Append(string.Format("{0:X2}", buff[i]));
			return sb.ToString();
		}
	}
}
