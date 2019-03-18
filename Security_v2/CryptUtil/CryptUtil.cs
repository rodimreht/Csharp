using System.Globalization;
using System.Text;

namespace System.Security.Cryptography
{
	public class CryptUtil
	{
		/// <summary>
		/// 암호화된 문자열을 바이트 배열로 변환한다.
		/// </summary>
		/// <param name="sOrg"></param>
		/// <returns></returns>
		public static byte[] GetHexArray(string sOrg)
		{
			byte[] bArray;

			// 기존 '+' 구분 대문자 암호화 문자열과의 호환성을 위해 유지
			if (sOrg.ToUpper().Replace(" ", "+").Replace("%2B", "+").IndexOf("+") > 0)
			{
				string[] sArray = sOrg.ToUpper().Split('+');
				bArray = new byte[sArray.Length];

				for (int i = 0; i < sArray.Length; i++)
					bArray[i] = GetByteFromHex(sArray[i]);
			}
			else
			{
				int len = sOrg.Length/2;
				bArray = new byte[len];
				for (int i = 0; i < len; i++)
					bArray[i] = GetByteFromHex(sOrg.Substring(i*2, 2));
			}
			return bArray;
		}

		/// <summary>
		/// 바이트 배열을 16진수 문자열로 변환한다.
		/// </summary>
		/// <param name="bBytes">입력 바이트 배열</param>
		/// <returns>2자리 16진수 문자열이 나열된 문자열</returns>
		public static string GetHexFromByte(byte[] bBytes)
		{
			StringBuilder sb = new StringBuilder(bBytes.Length);
			for (int i = 0; i < bBytes.Length; i++)
				sb.Append(bBytes[i].ToString("x2"));

			return sb.ToString();
		}


		/// <summary>
		/// 16진수 문자열(2자리 값)을 byte로 변환
		/// </summary>
		/// <param name="sHex">16진수 문자열 값</param>
		/// <returns>바이트 값</returns>
		public static byte GetByteFromHex(string sHex)
		{
			return byte.Parse(sHex, NumberStyles.HexNumber);
		}

		/// <summary>
		/// Decodes the specified Base64 encoded string.
		/// </summary>
		/// <param name="enc">The Base64 encoded string.</param>
		/// <returns></returns>
		public static byte[] Decode(string enc)
		{
			return Convert.FromBase64String(enc);
		}

		/// <summary>
		/// Base64 encodes the specified original byte array.
		/// </summary>
		/// <param name="org">The original byte array.</param>
		/// <returns></returns>
		public static string Encode(byte[] org)
		{
			return Convert.ToBase64String(org);

		}
	}
}