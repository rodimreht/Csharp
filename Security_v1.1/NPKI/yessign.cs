using System;
using System.IO;
using System.Text;
using Microsoft.Web.Services2.Security.Tokens;
using Microsoft.Web.Services2.Security.X509;
using Nets.Security;

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
		/// 바이트 배열을 16진수 문자열로 변환한다.
		/// </summary>
		/// <param name="bBytes">입력 바이트 배열</param>
		/// <returns>2자리 16진수 문자열이 나열된 문자열</returns>
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
