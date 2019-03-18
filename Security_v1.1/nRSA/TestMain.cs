using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace PublicKey
{
	/// <summary>
	/// TestMain에 대한 요약 설명입니다.
	/// </summary>
	public class TestMain
	{
		static void Main()
		{
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512);
			rsa.PersistKeyInCsp = true;

			RSAParameters param = rsa.ExportParameters(false);
			string e = getHexFromByte(param.Exponent);
			string n = getHexFromByte(param.Modulus);
			/*
			string e = "10001";
			string n = "30db31542ace0f7d37a629ee5eba28cb";
			*/

			string s = "test:1111";

			nRSA nRsa = new nRSA(e, n);
			string encTemp = nRsa.Encrypt(nRsa, s);
			Console.WriteLine("encTemp: " + encTemp);

			RSAParameters param2 = rsa.ExportParameters(true);
			string d = getHexFromByte(param2.D);
			/*
			string d = "202700adbd85e2d7182720c3a0ee19c1";
			*/

			nRSA nRsa2 = new nRSA(e, d, n);
			string decTemp = nRsa2.Decrypt(nRsa2, encTemp);
			Console.WriteLine("decTemp: " + decTemp);

			byte[] dec = rsa.Decrypt(getHexArray(encTemp), false);
			decTemp = System.Text.Encoding.Default.GetString(dec);
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
