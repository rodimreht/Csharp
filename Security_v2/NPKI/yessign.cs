using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;

namespace NPKI
{
	/// <summary>
	/// yessign에 대한 요약 설명입니다.
	/// </summary>
	class yessign
	{
		/// <summary>
		/// 공인인증서(안심클릭) 읽기
		/// </summary>
		[STAThread]
		static void Main()
		{
			string folder = Environment.GetEnvironmentVariable("ProgramFiles") + "\\NPKI\\yessign"; // XP
			if (Environment.OSVersion.Version.Major >= 6)
				folder = Environment.GetEnvironmentVariable("LOCALAPPDATA") + "Low\\NPKI\\KICA"; // > Windows 7

			//string certFile = folder + "\\4AFBBD332D8BB1D18C946BFFE042365F1C91CB08_10080.der";
			//string certFile = folder + "\\E2EC6D2CE57D9BC09EAC015379BA9A8F9A85D90B_10050.der";
			string certFile = folder + "\\B909F2B621489A2ABA025980862793166A77F559_10081.der";

			X509Certificate cert = X509Certificate.CreateFromCertFile(certFile);
			X509Certificate2 token = new X509Certificate2(cert);
			/*
			X509SecurityToken token = new X509SecurityToken(new X509Certificate2(cert));
			Console.WriteLine("IssuerName: " + token.Certificate.Issuer);
			Console.WriteLine("KeyAlgorithm: " + token.Certificate.GetKeyAlgorithm());
			Console.WriteLine("KeyAlgorithmParameters: " + token.Certificate.GetKeyAlgorithmParametersString());
			Console.WriteLine("Name: " + token.Certificate.Subject);
			Console.WriteLine("PublicKey: " + token.Certificate.GetPublicKeyString());
			Console.WriteLine("SerialNumber: " + token.Certificate.GetSerialNumberString());

			RSACryptoServiceProvider rsa = token.Certificate.PrivateKey as RSACryptoServiceProvider;
			*/
			Console.WriteLine("IssuerName: " + token.Issuer);
			Console.WriteLine("KeyAlgorithm: " + token.GetKeyAlgorithm());
			Console.WriteLine("KeyAlgorithmParameters: " + token.GetKeyAlgorithmParametersString());
			Console.WriteLine("Name: " + token.Subject);
			Console.WriteLine("PublicKey: " + token.GetPublicKeyString());
			Console.WriteLine("SerialNumber: " + token.GetSerialNumberString());

			RSACryptoServiceProvider rsa = token.PrivateKey as RSACryptoServiceProvider;
			if (rsa != null)
			{
				string keyfilepath = FindKeyLocation(rsa.CspKeyContainerInfo.UniqueKeyContainerName);
				FileInfo file = new FileInfo(keyfilepath + "\\" +
				                             rsa.CspKeyContainerInfo.UniqueKeyContainerName);
			}
			
			DirectoryInfo keyDir = new DirectoryInfo(folder + "\\USER");
			byte[] bytes = null;
			foreach (DirectoryInfo dir in keyDir.GetDirectories())
			{
				FileInfo[] files = dir.GetFiles("s*.key");
				FileStream stream = files[0].OpenRead();
				stream.Position = 0;

				bytes = new byte[stream.Length];
				stream.Read(bytes, 0, (int)stream.Length);
				stream.Close();

				//SEED seed = new SEED();
				//string a = seed.seedDecryptString("matthaeu", GetHexFromByte(bytes));
				//Console.WriteLine(a);
			}

			Console.WriteLine("KeyType: " + PKCS8.GetType(bytes));
			PKCS8.EncryptedPrivateKeyInfo encInfo = new PKCS8.EncryptedPrivateKeyInfo(bytes);
			Console.WriteLine("Algorithm: " + encInfo.Algorithm);
			
			nPKCS12 p12 = new nPKCS12();
			p12.Password = "matt4975";
			byte[] decrypted = p12.Decrypt(encInfo.Algorithm, encInfo.Salt, encInfo.IterationCount, encInfo.EncryptedData);

			if (decrypted != null)
			{
				PKCS8.PrivateKeyInfo keyInfo = new PKCS8.PrivateKeyInfo(decrypted);
				RSA rsa2 = PKCS8.PrivateKeyInfo.DecodeRSA(keyInfo.PrivateKey);
				RSACryptoServiceProvider provider = (RSACryptoServiceProvider)rsa2;
				byte[] buffer = Encoding.Default.GetBytes("1234567890");
				byte[] signed = provider.SignData(buffer, "SHA1");
				//provider.VerifyData(signed, "SHA1", signed);
			}
		}

		private static void getKeyFile()
		{
			string subject = "WSE2QuickStartServer";

			X509Store store = new X509Store(StoreName.AddressBook,
			                                StoreLocation.CurrentUser);
			store.Open(OpenFlags.ReadOnly);

			X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName, subject, false);

			X509Certificate2 wsecert = certs[0];

			RSACryptoServiceProvider rsa = wsecert.PrivateKey as RSACryptoServiceProvider;

			if (rsa != null)
			{
				string keyfilepath = FindKeyLocation(rsa.CspKeyContainerInfo.UniqueKeyContainerName);

				FileInfo file = new FileInfo(keyfilepath + "\\" +
				                             rsa.CspKeyContainerInfo.UniqueKeyContainerName);

				FileSecurity fs = file.GetAccessControl();

				NTAccount account = new NTAccount(@"machinename\username");
				fs.AddAccessRule(new FileSystemAccessRule(account,
				                                          FileSystemRights.FullControl, AccessControlType.Allow));

				file.SetAccessControl(fs);
			}

			store.Close();
		}

		private static string FindKeyLocation(string keyFileName)
		{
			string text1 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			string text2 = text1 + @"\Microsoft\Crypto\RSA\MachineKeys";
			string[] textArray1 = Directory.GetFiles(text2, keyFileName);
			if (textArray1.Length > 0)
			{
				return text2;
			}
			string text3 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string text4 = text3 + @"\Microsoft\Crypto\RSA\";
			textArray1 = Directory.GetDirectories(text4);
			if (textArray1.Length > 0)
			{
				foreach (string text5 in textArray1)
				{
					textArray1 = Directory.GetFiles(text5, keyFileName);
					if (textArray1.Length != 0)
					{
						return text5;
					}
				}
			}
			return "Private key exists but is not accessible";
		}
	}
}
