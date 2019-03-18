using System;
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace MD5
{
	/// <summary>
	/// SHA1 암호화 알고리즘 클래스
	/// </summary>
	public class CSHA1
	{
		public CSHA1()
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

			SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
			byte[] byteBuffer = sha1.ComputeHash(byteSource);

			sOut = CryptUtil.GetHexFromByte(byteBuffer);
		}
	}
}
