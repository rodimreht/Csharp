using System;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace MD5
{
	/// <summary>
	/// CSHA256에 대한 요약 설명입니다.
	/// </summary>
	public class CSHA256
	{
		public CSHA256()
		{
			//
			// TODO: 여기에 생성자 논리를 추가합니다.
			//
		}

		/// <summary>
		/// 문자열 암호화 / 복호화 처리 함수
		/// </summary>
		/// <param name="sIn">입력 문자열</param>
		/// <param name="sOut">출력 문자열</param>
		public void EncryptDecryptString(string sIn, out string sOut)
		{
			byte[] byteSource;
			byteSource = Encoding.Default.GetBytes(sIn);

			SHA256 sha256 = new SHA256Managed();
			byte[] byteBuffer = sha256.ComputeHash(byteSource);

			sOut = GetHexFromByte(byteBuffer);
		}

		/// <summary>
		/// 바이트 배열을 16진수 문자열로 변환한다.
		/// </summary>
		/// <param name="bBytes">입력 바이트 배열</param>
		/// <returns>2자리 16진수 문자열이 나열된 문자열</returns>
		private string GetHexFromByte(byte[] bBytes)
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


		/// <summary>
		/// 16진수 문자열(2자리 값)을 byte로 변환
		/// </summary>
		/// <param name="sHex">16진수 문자열 값</param>
		/// <returns>바이트 값</returns>
		private byte GetByteFromHex(string sHex)
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
