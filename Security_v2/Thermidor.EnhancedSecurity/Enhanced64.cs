using System;
using System.Text;
using Nets.IM.Common;

namespace Thermidor.EnhancedSecurity
{
	/// <summary>
	/// Enhanced64 암호화 클래스 (by Thermidor on 2006-04-12)
	/// </summary>
	/// <remarks>
	/// Base64 인코딩을 확장하여 대칭키 기반 암호화/복호화를 구현한 클래스<br/>
	/// (Base64는 URL 및 HTTP 헤더에 포함되면 안되는 문자들이 포함되어 있어
	/// 이를 URL에 포함 가능한 다른 문자로 치환하는 기본 인코딩 메서드와,<br/>
	/// 문자열 키를 입력받아 대칭키 암호화 알고리즘을 구현한 확장 메서드를 제공한다.)
	/// </remarks>
	public class Enhanced64 : ICryptoBase
	{
		private char[] cBase64 = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
									 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
									 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
									 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/', '='};
		private char[] cEnhc64 = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
									 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
									 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 
									 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '*', '.'};
		private int keyBase = 0;

		/// <summary>
		/// Base64로 인코딩하고 그 결과를 Enhanced64로 다시 인코딩한 문자열을 얻는다.
		/// </summary>
		/// <param name="val">인코딩할 원본 문자열</param>
		/// <returns></returns>
		public string Encode(string val)
		{
			string sTemp = Convert.ToBase64String(Encoding.Default.GetBytes(val));
			char[] cTemp = sTemp.ToCharArray();
			for (int i = 0, iend = cTemp.Length; i < iend; i++)
				cTemp[i] = getEnhanced64CharEnc(cTemp[i], 'A');

			return new string(cTemp);
		}

		/// <summary>
		/// Base64로 인코딩하고 그 결과를 Enhanced64로 다시 인코딩한 문자열을 얻는다.
		/// </summary>
		/// <param name="sValue">인코딩할 원본 문자열</param>
		public override string Encrypt(string sValue)
		{
			return Encode(sValue);
		}

		/// <summary>
		/// Enhanced64 암호화 문자열을 얻는다.
		/// </summary>
		/// <param name="sKey">암호화 키(8바이트 이상, 4자리 이상 비동일문자)</param>
		/// <param name="sValue">암호화할 원본 문자열</param>
		/// <returns></returns>
		public override string Encrypt(string sKey, string sValue)
		{
			if (!isStrongKey(sKey)) return "";
			this.keyBase = getKeyBase(sKey);

			string sTemp = Convert.ToBase64String(Encoding.Default.GetBytes(sValue));
			char[] cTemp = sTemp.ToCharArray();
			char[] cKey = sKey.ToCharArray();
			int keyCount = 0;

			for (int i = 0, iend = cTemp.Length; i < iend; i++)
			{
				cTemp[i] = getEnhanced64CharEnc(cTemp[i], cKey[keyCount++]);
				if (keyCount >= cKey.Length) keyCount = 0;
			}
			return new string(cTemp);
		}

		/// <summary>
		/// Enhanced64로 디코딩하고 그 결과를 다시 Base64로 디코딩한 문자열을 얻는다.
		/// </summary>
		/// <param name="val">인코딩된 문자열</param>
		/// <returns></returns>
		public string Decode(string val)
		{
			char[] cTemp = val.ToCharArray();
			for (int i = 0, iend = cTemp.Length; i < iend; i++)
				cTemp[i] = getBase64CharDec(cTemp[i], 'A');

			return Encoding.Default.GetString(Convert.FromBase64String(new string(cTemp)));
		}

		/// <summary>
		/// Enhanced64로 디코딩하고 그 결과를 다시 Base64로 디코딩한 문자열을 얻는다.
		/// </summary>
		/// <param name="sValue">인코딩된 문자열</param>
		public override string Decrypt(string sValue)
		{
			return Decode(sValue);
		}

		/// <summary>
		/// Enhanced64 복호화 문자열을 얻는다.
		/// </summary>
		/// <param name="sKey">암호화 키(8바이트 이상, 4자리 이상 비동일문자)</param>
		/// <param name="sValue">복호화할 암호화된 문자열</param>
		/// <returns></returns>
		public override string Decrypt(string sKey, string sValue)
		{
			if (!isStrongKey(sKey)) return "";
			this.keyBase = getKeyBase(sKey);

			char[] cTemp = sValue.ToCharArray();
			char[] cKey = sKey.ToCharArray();
			int keyCount = 0;

			for (int i = 0, iend = cTemp.Length; i < iend; i++)
			{
				cTemp[i] = getBase64CharDec(cTemp[i], cKey[keyCount++]);
				if (keyCount >= cKey.Length) keyCount = 0;
			}
			return Encoding.Default.GetString(Convert.FromBase64String(new string(cTemp))).Trim();
		}

		// 키 문자열의 Base64 인덱스의 전체 합을 구한다.
		private int getKeyBase(string key)
		{
			int keyBase = 0;

			char[] cArr = key.ToCharArray();
			for (int i = 0, iend = cArr.Length; i < iend; i++)
			{
				for (int k = 0, kend = cBase64.Length; k < kend; k++)
				{
					if (cArr[i].Equals(cBase64[k]))
					{
						keyBase += k;
						break;
					}
				}
			}

			return keyBase % cBase64.Length;
		}

		// 해킹에 취약한 키를 사용했는지의 여부를 체크한다.
		private bool isStrongKey(string key)
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
		private bool isInBase64(char c)
		{
			for (int i = 0, iend = cBase64.Length; i < iend; i++)
				if (cBase64[i].Equals(c)) return true;

			return false;
		}

		// Base64 문자열 인덱스를 구한다.
		private int getBase64Index(char c)
		{
			for (int i = 0, iend = cBase64.Length; i < iend; i++)
				if (cBase64[i].Equals(c)) return i;

			return -1;
		}

		// Enhanced64 문자열 인덱스를 구한다.
		private int getEnhc64Index(char c)
		{
			for (int i = 0, iend = cEnhc64.Length; i < iend; i++)
				if (cEnhc64[i].Equals(c)) return i;

			return -1;
		}

		// Base64 문자를 암호화하여 Enhanced64 문자로 변환한다.
		private char getEnhanced64CharEnc(char baseChar, char keyChar)
		{
			int length = cEnhc64.Length;
			int idx = getBase64Index(baseChar) + getBase64Index(keyChar);
			if (idx >= length) idx -= length;
			idx += this.keyBase;
			if (idx >= length) idx -= length;

			return cEnhc64[idx];
		}

		// Enhanced64 문자를 복호화하여 Base64 문자로 변환한다.
		private char getBase64CharDec(char enhcChar, char keyChar)
		{
			int length = cBase64.Length;
			int idx = getEnhc64Index(enhcChar) - getBase64Index(keyChar);
			if (idx < 0) idx += length;
			idx -= this.keyBase;
			if (idx < 0) idx += length;

			return cBase64[idx];
		}
	}
}
