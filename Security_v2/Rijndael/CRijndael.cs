using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace nAES
{
	/// <summary>
	/// CRijndael에 대한 요약 설명입니다.
	/// </summary>
	public class CRijndael
	{
		private const int RIJNDAEL_BIT_LENGTH = 128;

		public CRijndael()
		{
		}

		/// <summary>
		/// 암호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 원본 문자열</param>
		/// <returns>암호화된 문자열</returns>
		public string rijndaelEncryptString(string sKey, string sOrg)
		{
			// 복호화 프로세스 중 필요한 변수
			MemoryStream msIn;
			MemoryStream msOut = new MemoryStream();
			msOut.SetLength(0);

			byte[] bTemp = Encoding.Default.GetBytes(sOrg);
			msIn = new MemoryStream(bTemp, false);

			long nLength = msIn.Length;
			byte[] byteBuffer;
			long nBytesProcessed = 0;
			int iBytesInCurrentBlock = 0;

			byte[] bRjnKey = GetKey(sKey);
			byte[] bRjnIV = GetIV(sKey); // 초기화 벡터도 똑같은 키를 사용한다.

			Rijndael r = Rijndael.Create();
			r.BlockSize = RIJNDAEL_BIT_LENGTH;
			r.KeySize = RIJNDAEL_BIT_LENGTH;
			r.Mode = CipherMode.CBC;
			r.Padding = PaddingMode.PKCS7;
			CryptoStream cs = new CryptoStream(msOut, r.CreateEncryptor(bRjnKey, bRjnIV), CryptoStreamMode.Write);
			
			try
			{
				do
				{
					byteBuffer = new byte[4096];
					iBytesInCurrentBlock = msIn.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());

				}while (nBytesProcessed < nLength);

				// 최종 버퍼링된 인코딩 바이트를 스트림에 기록한다.
				cs.FlushFinalBlock();

				msOut.Position = 0;
				byte[] byteBuffer2 = new byte[msOut.Length];
				msOut.Read(byteBuffer2, 0, (int) msOut.Length);
				
				cs.Close();
				msIn.Close();
				msOut.Close();

				return CryptUtil.GetHexFromByte(byteBuffer2);
			}
			catch(Exception ex)
			{
				cs.Close();
				msIn.Close();
				msOut.Close();

				throw ex;
			}
		}

		/// <summary>
		/// 복호화 처리 함수
		/// </summary>
		/// <param name="sKey">비밀키</param>
		/// <param name="sOrg">입력 암호화 문자열</param>
		/// <returns>복호화된 문자열</returns>
		public string rijndaelDecryptString(string sKey, string sOrg)
		{
			// 복호화 프로세스 중 필요한 변수
			MemoryStream msIn;
			MemoryStream msOut = new MemoryStream();
			msOut.SetLength(0);

			byte[] bTemp = CryptUtil.GetHexArray(sOrg);
			msIn = new MemoryStream(bTemp, false);

			long nLength = msIn.Length;
			byte[] byteBuffer;
			long nBytesProcessed = 0;
			int iBytesInCurrentBlock = 0;

			byte[] bRjnKey = GetKey(sKey);
			byte[] bRjnIV = GetIV(sKey); // 초기화 벡터도 똑같은 키를 사용한다.

			Rijndael r = Rijndael.Create();
			r.BlockSize = RIJNDAEL_BIT_LENGTH;
			r.KeySize = RIJNDAEL_BIT_LENGTH;
			r.Mode = CipherMode.CBC;
			r.Padding = PaddingMode.PKCS7;
			CryptoStream cs = new CryptoStream(msOut, r.CreateDecryptor(bRjnKey, bRjnIV), CryptoStreamMode.Write);
			
			try
			{
				do
				{
					byteBuffer = new byte[4096];
					iBytesInCurrentBlock = msIn.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());

				}while (nBytesProcessed < nLength);

				// 최종 버퍼링된 인코딩 바이트를 스트림에 기록한다.
				cs.FlushFinalBlock();

				msOut.Position = 0;
				byte[] byteBuffer2 = new byte[msOut.Length];
				msOut.Read(byteBuffer2, 0, (int) msOut.Length);

				cs.Close();
				msIn.Close();
				msOut.Close();

				return Encoding.Default.GetString(byteBuffer2);
			}
			catch(Exception ex)
			{
				cs.Close();
				msIn.Close();
				msOut.Close();

				throw ex;
			}
		}

		/// <summary>
		/// 문자열 키 값을 정해진 길이의 바이트 배열로 변환한다.
		/// </summary>
		/// <param name="sKey">입력 문자열</param>
		/// <returns>출력 바이트 배열</returns>
		public byte[] GetKey(string sKey)
		{
			int size = RIJNDAEL_BIT_LENGTH / 8;
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
		private byte[] GetIV(string sKey)
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
	}
}
