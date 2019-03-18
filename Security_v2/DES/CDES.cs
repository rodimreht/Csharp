using System;
using System.Data;
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace ex_Security
{
	/// <summary>
	/// DES 암호화 알고리즘 클래스
	/// </summary>
	public class CDES
	{
		private const int DES_BIT_LENGTH = 64;	// 64비트 암호화 (56bit)

		public CDES()
		{
			//
			// TODO: 여기에 생성자 논리를 추가합니다.
			//
		}

		/// <summary>
		/// 파일 암호화 / 복호화 처리 함수
		/// </summary>
		/// <param name="sInputFile">입력 파일 경로</param>
		/// <param name="sOutputFile">출력 파일 경로</param>
		/// <param name="bDESKey">비밀 키</param>
		/// <param name="bDESIV">초기화 벡터</param>
		/// <param name="strDirection">암호화 / 복호화 구분</param>
		public void EncryptDecryptFile(string sInputFile, string sOutputFile, byte[] bDESKey, byte[] bDESIV, string strDirection)
		{
			//파일 스트림을 만들어 입력 및 출력 파일을 처리
			FileStream fsInput = new FileStream(sInputFile, FileMode.Open, FileAccess.Read);
			FileStream fsOutput = new FileStream(sOutputFile, FileMode.OpenOrCreate, FileAccess.Write);
			fsOutput.SetLength(0);

			//암호화/복호화 프로세스 중 필요한 변수
			byte[] byteBuffer = new byte[4096];
			long nBytesProcessed = 0;
			long nFileLength = fsInput.Length;
			int iBytesInCurrentBlock;
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			CryptoStream cs = null;
			
			// 암호화나 복호화를 위한 설정
			switch (strDirection)
			{
				case "E" :
					cs = new CryptoStream(fsOutput, des.CreateEncryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;

				case "D" :
					cs = new CryptoStream(fsOutput, des.CreateDecryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;
			}
			
			try
			{
				//입력 파일에서 읽은 다음 암호화하거나 암호를 해독하고 출력 파일에 씀.
				do
				{
					iBytesInCurrentBlock = fsInput.Read(byteBuffer, 0, 4096);
					cs.Write(byteBuffer, 0, iBytesInCurrentBlock);
					nBytesProcessed = nBytesProcessed + long.Parse(iBytesInCurrentBlock.ToString());

				}while (nBytesProcessed < nFileLength);
			}
			finally
			{
				cs.Close();
				fsInput.Close();
				fsOutput.Close();
			}
		}

		/// <summary>
		/// 문자열 암호화 / 복호화 처리 함수
		/// </summary>
		/// <param name="sIn">입력 문자열</param>
		/// <param name="sOut">출력 문자열</param>
		/// <param name="bDESKey">비밀 키</param>
		/// <param name="bDESIV">초기화 벡터</param>
		/// <param name="sMethod">암호화 / 복호화 구분</param>
		public void EncryptDecryptString(string sIn, out string sOut, byte[] bDESKey, byte[] bDESIV, string sMethod)
		{
			// 암호화/복호화 프로세스 중 필요한 변수
			MemoryStream msIn;
			MemoryStream msOut = new MemoryStream();
			msOut.SetLength(0);

			if (sMethod == "E")
			{
				byte[] bTemp = Encoding.Default.GetBytes(sIn);
				msIn = new MemoryStream(bTemp, false);
			}
			else
			{
				string[] sTemp = sIn.ToUpper().Split('+');
				byte[] bTemp = new byte[sTemp.Length];

				for (int i = 0; i < sTemp.Length; i++)
				{
					bTemp[i] = CryptUtil.GetByteFromHex(sTemp[i]);
				}
				msIn = new MemoryStream(bTemp, false);
			}

			long nLength = msIn.Length;
			byte[] byteBuffer;
			long nBytesProcessed = 0;
			int iBytesInCurrentBlock = 0;

			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			CryptoStream cs = null;
			
			switch (sMethod)
			{
				case "E":
					// 암호화를 위한 설정
					cs = new CryptoStream(msOut, des.CreateEncryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;

				case "D":
					// 복호화를 위한 설정
					cs = new CryptoStream(msOut, des.CreateDecryptor(bDESKey, bDESIV), CryptoStreamMode.Write);
					break;

				default:
					break;
			}
			
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

				if (sMethod == "E")
				{
					msOut.Position = 0;
					byte[] byteBuffer2 = new byte[msOut.Length];
					msOut.Read(byteBuffer2, 0, (int) msOut.Length);

					sOut = CryptUtil.GetHexFromByte(byteBuffer2);
				}
				else
				{
					msOut.Position = 0;
					byte[] byteBuffer2 = new byte[msOut.Length];
					msOut.Read(byteBuffer2, 0, (int) msOut.Length);

					sOut = Encoding.Default.GetString(byteBuffer2);
				}
			}
			finally
			{
				cs.Close();
				msIn.Close();
				msOut.Close();
			}
		}

		/// <summary>
		/// 문자열 키 값을 정해진 길이의 바이트 배열로 변환한다.
		/// </summary>
		/// <param name="sKey">입력 문자열</param>
		/// <returns>출력 바이트 배열</returns>
		public byte[] GetKeyByteArray(string sKey)
		{
			byte[] byteTemp = new byte[(int) (DES_BIT_LENGTH / 8)];

			// 길이가 8자가 아니면 왼쪽을 자르거나,
			// 오른쪽을 여백을 채워서 8자(64비트)를 맞춤.
			if (sKey.Length > 8) sKey = sKey.Substring(0, 8);
			sKey = sKey.PadRight((int) (DES_BIT_LENGTH / 8));

			// sPassword를 ASCII 코드에 해당하는 Integer로 Encoding 한후, byteTemp 대입.
			byteTemp = Encoding.ASCII.GetBytes(sKey);
		
			return byteTemp;
		}
	}
}
