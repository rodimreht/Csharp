using System;
using System.Security.Cryptography;
using System.Text;

namespace NETS_iMan
{
	public class NISecurity
	{
		private static readonly char[] BASE64 = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
									 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
									 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
									 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/', '='};
		private static readonly  char[] ENHANCED64 = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
									 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
									 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
									 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '*', '.'};
		private static int KEY_BASE = 0;
		private static readonly object lockObject = new object();

		/// <summary>
		/// Base64로 인코딩한 결과를 Enhanced64로 다시 인코딩한 문자열을 얻는다.
		/// </summary>
		/// <param name="val">인코딩할 원본 문자열</param>
		/// <returns></returns>
		public static string Encode(string val)
		{
			string sTemp = Convert.ToBase64String(Encoding.Default.GetBytes(val));
			char[] cTemp = sTemp.ToCharArray();
			for (int i = 0, iend = cTemp.Length; i < iend; i++)
				cTemp[i] = getEnhanced64CharEnc(cTemp[i], 'A');

			return new string(cTemp);
		}

		/// <summary>
		/// Enhanced64 암호화 문자열을 얻는다.
		/// </summary>
		/// <param name="key">암호화 키(8바이트 이상, 4자리 이상 비동일문자)</param>
		/// <param name="val">암호화할 원본 문자열</param>
		/// <returns></returns>
		public static string Encrypt(string key, string val)
		{
			if (!IsStrongKey(key)) return "";

			char[] cTemp;
			lock (lockObject)
			{
				KEY_BASE = getKeyBase(key);

				string sTemp = Convert.ToBase64String(Encoding.Default.GetBytes(val));
				cTemp = sTemp.ToCharArray();
				char[] cKey = key.ToCharArray();
				int keyCount = 0;

				for (int i = 0, iend = cTemp.Length; i < iend; i++)
				{
					cTemp[i] = getEnhanced64CharEnc(cTemp[i], cKey[keyCount++]);
					if (keyCount >= cKey.Length) keyCount = 0;
				}
			}
			return new string(cTemp);
		}

		/// <summary>
		/// Enhanced64로 디코딩한 결과를 다시 Base64로 디코딩한 문자열을 얻는다.
		/// </summary>
		/// <param name="val">인코딩된 문자열</param>
		/// <returns></returns>
		public static string Decode(string val)
		{
			char[] cTemp = val.ToCharArray();
			for (int i = 0, iend = cTemp.Length; i < iend; i++)
				cTemp[i] = getBase64CharDec(cTemp[i], 'A');

			return Encoding.Default.GetString(Convert.FromBase64String(new string(cTemp)));
		}

		/// <summary>
		/// Enhanced64 복호화 문자열을 얻는다.
		/// </summary>
		/// <param name="key">암호화 키(8바이트 이상, 4자리 이상 비동일문자)</param>
		/// <param name="val">복호화할 암호화된 문자열</param>
		/// <returns></returns>
		public static string Decrypt(string key, string val)
		{
			if (!IsStrongKey(key)) return "";
			
			char[] cTemp;
			lock (lockObject)
			{
				KEY_BASE = getKeyBase(key);

				cTemp = val.ToCharArray();
				char[] cKey = key.ToCharArray();
				int keyCount = 0;

				for (int i = 0, iend = cTemp.Length; i < iend; i++)
				{
					cTemp[i] = getBase64CharDec(cTemp[i], cKey[keyCount++]);
					if (keyCount >= cKey.Length) keyCount = 0;
				}
			}
			return Encoding.Default.GetString(Convert.FromBase64String(new string(cTemp)));
		}

		// 키 문자열의 Base64 인덱스의 전체 합을 구한다.
		private static int getKeyBase(string key)
		{
			int keyBase = 0;

			char[] cArr = key.ToCharArray();
			for (int i = 0, iend = cArr.Length; i < iend; i++)
			{
				for (int k = 0, kend = BASE64.Length; k < kend; k++)
				{
					if (cArr[i].Equals(BASE64[k]))
					{
						keyBase += k;
						break;
					}
				}
			}

			return keyBase % BASE64.Length;
		}

		/// <summary>
		/// 해킹에 취약한 키를 사용했는지의 여부를 체크한다.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsStrongKey(string key)
		{
			// 8자리 이상이어야 한다.
			if (key.Trim().Length < 8) return false;

			// Base64에 포함된 문자이어야 하며, 같은 문자가 4자리 이상 연속되면 안된다.
			char[] cArr = key.ToCharArray();
			char c = '\0';
			int count = 1;

			for (int i = 0, iend = cArr.Length; i < iend; i++)
			{
				if (!isInBase64(cArr[i])) return false;

				if (i > 0)  c = cArr[i - 1];
				if (c.Equals(cArr[i]))
				{
					count++;
					if (count > 3) return false;
				}
				else
					count = 1;
			}
			if (count > 3)
				return false;
			else
				return true;
		}

		// Base64에 포함된 문자인지 체크한다.
		private static bool isInBase64(char c)
		{
			for (int i = 0, iend = BASE64.Length; i < iend; i++)
				if (BASE64[i].Equals(c)) return true;

			return false;
		}

		// Base64 문자열 인덱스를 구한다.
		private static int getBase64Index(char c)
		{
			for (int i = 0, iend = BASE64.Length; i < iend; i++)
				if (BASE64[i].Equals(c)) return i;

			return -1;
		}

		// Enhanced64 문자열 인덱스를 구한다.
		private static int getEnhc64Index(char c)
		{
			for (int i = 0, iend = ENHANCED64.Length; i < iend; i++)
				if (ENHANCED64[i].Equals(c)) return i;

			return -1;
		}

		// Base64 문자를 암호화하여 Enhanced64 문자로 변환한다.
		private static char getEnhanced64CharEnc(char baseChar, char keyChar)
		{
			int length = ENHANCED64.Length;
			int idx = getBase64Index(baseChar) + getBase64Index(keyChar);
			if (idx >= length) idx -= length;
			idx += KEY_BASE;
			if (idx >= length) idx -= length;

			return ENHANCED64[idx];
		}

		// Enhanced64 문자를 복호화하여 Base64 문자로 변환한다.
		private static char getBase64CharDec(char enhcChar, char keyChar)
		{
			int length = ENHANCED64.Length;
			int idx = getEnhc64Index(enhcChar) - getBase64Index(keyChar);
			if (idx < 0) idx += length;
			idx -= KEY_BASE;
			if (idx < 0) idx += length;

			return BASE64[idx];
		}
		
		/// <summary>
		/// 문자열 해시 처리 함수
		/// </summary>
		/// <param name="sIn">입력 문자열</param>
		public static string Hash(string sIn)
		{
			byte[] byteSource = Encoding.Default.GetBytes(sIn);

			SHA256 sha256 = new SHA256Managed();
			byte[] byteBuffer = sha256.ComputeHash(byteSource);

			return getHexFromByte(byteBuffer);
		}

		// 바이트 배열을 16진수 문자열로 변환한다.
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
	}
}
