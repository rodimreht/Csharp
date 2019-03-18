using System.Security.Cryptography;
using System.Text;
using Microsoft.Web.Services2.Security.Cryptography;

namespace nAES
{
	/// <summary>
	/// CAES에 대한 요약 설명입니다.
	/// </summary>
	public class CAES
	{
		private const int AES128_BIT_LENGTH = 128;
		private const int AES192_BIT_LENGTH = 192;
		private const int AES256_BIT_LENGTH = 256;

		public CAES()
		{
		}

		// AES 클래스에서는 초기화 벡터(IV)를 자동으로 생성하여 암호화 블럭 최초 16바이트에
		// 삽입하므로 앞의 Rijndael 클래스의 결과와 다르게 나온다.
		private Rijndael getRijndael(string sKey, int bitLength)
		{
			byte[] bRjnKey = GetKey(sKey, bitLength);
			byte[] bRjnIV = GetIV(sKey); // 초기화 벡터도 똑같은 키를 사용한다.

			Rijndael r = RijndaelManaged.Create();
			r.BlockSize = bitLength;
			r.KeySize = bitLength;
			r.Mode = CipherMode.CBC;
			r.Padding = PaddingMode.PKCS7;
			r.Key = bRjnKey;
			r.IV = bRjnIV;

			return r;
		}

		/// <summary>
		/// 암호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 원본 문자열</param>
		/// <returns>암호화된 문자열</returns>
		public string aesEncryptString(string sKey, string sOrg)
		{
			return aes128EncryptString(sKey, sOrg);
		}

		/// <summary>
		/// 복호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 암호화 문자열</param>
		/// <returns>복호화된 문자열</returns>
		public string aesDecryptString(string sKey, string sOrg)
		{
			return aes128DecryptString(sKey, sOrg);
		}

		/// <summary>
		/// 암호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 원본 문자열</param>
		/// <returns>암호화된 문자열</returns>
		public string aes128EncryptString(string sKey, string sOrg)
		{
			byte[] bTemp = Encoding.Default.GetBytes(sOrg);

			AES128 aes = new AES128();
			aes.Key = getRijndael(sKey, AES128_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Encrypt(bTemp);

			return GetHexFromByte(buffer);
		}

		/// <summary>
		/// 복호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 암호화 문자열</param>
		/// <returns>복호화된 문자열</returns>
		public string aes128DecryptString(string sKey, string sOrg)
		{
			byte[] bTemp = GetHexArray(sOrg);

			AES128 aes = new AES128();
			aes.Key = getRijndael(sKey, AES128_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Decrypt(bTemp);

			return Encoding.Default.GetString(buffer);
		}

		/// <summary>
		/// 암호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 원본 문자열</param>
		/// <returns>암호화된 문자열</returns>
		public string aes192EncryptString(string sKey, string sOrg)
		{
			byte[] bTemp = Encoding.Default.GetBytes(sOrg);

			AES192 aes = new AES192();
			aes.Key = getRijndael(sKey, AES192_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Encrypt(bTemp);

			return GetHexFromByte(buffer);
		}

		/// <summary>
		/// 복호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 암호화 문자열</param>
		/// <returns>복호화된 문자열</returns>
		public string aes192DecryptString(string sKey, string sOrg)
		{
			byte[] bTemp = GetHexArray(sOrg);

			AES192 aes = new AES192();
			aes.Key = getRijndael(sKey, AES192_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Decrypt(bTemp);

			return Encoding.Default.GetString(buffer);
		}

		/// <summary>
		/// 암호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 원본 문자열</param>
		/// <returns>암호화된 문자열</returns>
		public string aes256EncryptString(string sKey, string sOrg)
		{
			byte[] bTemp = Encoding.Default.GetBytes(sOrg);

			AES256 aes = new AES256();
			aes.Key = getRijndael(sKey, AES256_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Encrypt(bTemp);

			return GetHexFromByte(buffer);
		}

		/// <summary>
		/// 복호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 암호화 문자열</param>
		/// <returns>복호화된 문자열</returns>
		public string aes256DecryptString(string sKey, string sOrg)
		{
			byte[] bTemp = GetHexArray(sOrg);

			AES256 aes = new AES256();
			aes.Key = getRijndael(sKey, AES256_BIT_LENGTH);
			byte[] buffer = aes.EncryptionFormatter.Decrypt(bTemp);

			return Encoding.Default.GetString(buffer);
		}

		/// <summary>
		/// 문자열 키 값을 정해진 길이의 바이트 배열로 변환한다.
		/// </summary>
		/// <param name="sKey">입력 문자열</param>
		/// <returns>출력 바이트 배열</returns>
		public byte[] GetKey(string sKey, int bitLength)
		{
			int size = bitLength / 8;
			byte[] byteTemp = new byte[size];

			// 길이가 16자가 아니면 오른쪽을 여백을 채워서 16자(128비트)를 맞춤.
			if (sKey.Length > size) sKey = sKey.Substring(0, size);
			sKey = sKey.PadRight(size);

			// sPassword를 ASCII 코드에 해당하는 Integer로 Encoding 한후, byteTemp 대입.
			byteTemp = Encoding.ASCII.GetBytes(sKey);
		
			return byteTemp;
		}

		/// <summary>
		/// 문자열 키 값에서 초기화 벡터 값을 얻는다.
		/// </summary>
		/// <param name="sKey">입력 문자열</param>
		/// <returns>출력 바이트 배열</returns>
		public byte[] GetIV(string sKey)
		{
			int size = 16;
			byte[] byteTemp = new byte[size];

			// 길이가 16자가 아니면 오른쪽을 여백을 채워서 16자(128비트)를 맞춤.
			if (sKey.Length > size) sKey = sKey.Substring(0, size);
			sKey = sKey.PadRight(size);

			// sPassword를 ASCII 코드에 해당하는 Integer로 Encoding 한후, byteTemp 대입.
			byteTemp = Encoding.ASCII.GetBytes(sKey);
		
			return byteTemp;
		}

		/// <summary>
		/// 암호화된 문자열을 바이트 배열로 변환한다.
		/// </summary>
		/// <param name="sOrg"></param>
		/// <returns></returns>
		private byte[] GetHexArray(string sOrg)
		{
			byte[] bArray = null;

			// 기존 '+' 구분 대문자 암호화 문자열과의 호환성을 위해 유지
			if (sOrg.ToUpper().Replace(" ", "+").Replace("%2B", "+").IndexOf("+") > 0)
			{
				string[] sArray = sOrg.ToUpper().Split('+');
				bArray = new byte[sArray.Length];

				for (int i = 0; i < sArray.Length; i++)
				{
					bArray[i] = GetByteFromHex(sArray[i]);
				}
			}
			else
			{
				int len = sOrg.Length / 2;
				bArray = new byte[len];
				for (int i = 0; i < len; i++)
				{
					bArray[i] = GetByteFromHex(sOrg.Substring(i * 2, 2));
				}
			}
			return bArray;
		}

		/// <summary>
		/// 바이트 배열을 16진수 문자열로 변환한다.
		/// </summary>
		/// <param name="bBytes">입력 바이트 배열</param>
		/// <returns>':'로 구분되는 16진수 문자열</returns>
		public string GetHexFromByte(byte[] bBytes)
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
		public byte GetByteFromHex(string sHex)
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
