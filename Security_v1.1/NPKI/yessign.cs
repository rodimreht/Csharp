using System;
using System.IO;
using System.Text;
using Microsoft.Web.Services2.Security.Tokens;
using Microsoft.Web.Services2.Security.X509;
using Nets.Security;

namespace NPKI
{
	/// <summary>
	/// yessign�� ���� ��� �����Դϴ�.
	/// </summary>
	class yessign
	{
		/// <summary>
		/// ����������(�Ƚ�Ŭ��) �б�
		/// </summary>
		[STAThread]
		static void Main()
		{
			string folder = Environment.GetEnvironmentVariable("ProgramFiles") + "\\NPKI\\yessign";
			string certFile = folder + "\\yessign.der";
			X509Certificate cert = X509Certificate.CreateCertFromFile(certFile);
			X509SecurityToken token = new X509SecurityToken(cert);
			Console.WriteLine("IssuerName: " + token.Certificate.GetIssuerName());
			Console.WriteLine("KeyAlgorithm: " + token.Certificate.GetKeyAlgorithm());
			Console.WriteLine("KeyAlgorithmParameters: " + token.Certificate.GetKeyAlgorithmParametersString());
			Console.WriteLine("Name: " + token.Certificate.GetName());
			Console.WriteLine("PublicKey: " + token.Certificate.GetPublicKeyString());
			Console.WriteLine("SerialNumber: " + token.Certificate.GetSerialNumberString());

			DirectoryInfo keyDir = new DirectoryInfo(folder + "\\USER");
			foreach (DirectoryInfo dir in keyDir.GetDirectories())
			{
				FileInfo[] files = dir.GetFiles("*.key");
				FileStream stream = files[0].OpenRead();
				stream.Position = 0;

				byte[] bytes = new byte[stream.Length];
				stream.Read(bytes, 0, (int)stream.Length);
				stream.Close();

				SEED seed = new SEED();
				string a = seed.seedDecryptString("matthaeu", GetHexFromByte(bytes));
				Console.WriteLine(a);
			}
		}

		/// <summary>
		/// ����Ʈ �迭�� 16���� ���ڿ��� ��ȯ�Ѵ�.
		/// </summary>
		/// <param name="bBytes">�Է� ����Ʈ �迭</param>
		/// <returns>2�ڸ� 16���� ���ڿ��� ������ ���ڿ�</returns>
		private static string GetHexFromByte(byte[] bBytes)
		{
			StringBuilder sb = new StringBuilder(bBytes.Length);
			for (int i = 0; i < bBytes.Length; i++)
			{
				if (bBytes[i].ToString("x").Length < 2)
					sb.Append("0" + bBytes[i].ToString("x"));
				else
					sb.Append(bBytes[i].ToString("x"));
			}
			//sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}
	}
}
