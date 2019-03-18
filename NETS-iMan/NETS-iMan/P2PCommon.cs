using System;
using System.Text;

namespace NETS_iMan
{
	class P2PCommon
	{
		public const int BLOCKSIZE = 64000; /* maximum size: 65507 */
		//public const int BLOCKSIZE = 1400; /* maximum size: 65507 */
		public const int SOCKET_TCP_BUFF_SIZE = 65536 * 4;

		public static string ToString(byte[] bytes)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
				sb.Append(bytes[i].ToString("x2"));

			return sb.ToString();
		}

		public static string GetStringHex(byte[] bytes)
		{
			return "\r\n" + GetStringHex(bytes, bytes.Length);
		}

		public static string GetStringHex(byte[] bytes, int length)
		{
			StringBuilder sb = new StringBuilder();
			StringBuilder sb2 = new StringBuilder();

			byte prevB = 0x00;
			bool isContinued = false;
			int contPos = -1;
			for (int i = 0; i < bytes.Length; i++)
			{
				if (i >= length) break;
				if (i > 0) prevB = bytes[i - 1];

				// 16진수 표현
				if (i % 16 != 0) sb.Append(" ");
				sb.Append(bytes[i].ToString("X2"));

				// 문자열 표현
				if ((bytes[i] >= 0x20) && (bytes[i] <= 0x7e))
					sb2.Append(Encoding.Default.GetString(bytes, i, 1));
				else
					sb2.Append(".");

				if ((i > 0) && ((i + 1) % 16 == 0))
				{
					sb.Append("\t" + sb2);
					if (i < length - 1) sb.AppendLine();
					sb2 = new StringBuilder();
				}

				if ((prevB == 0x55) && (bytes[i] == 0xAA))
				{
					isContinued = true;
					contPos = i + 1;
					break;
				}
			}

			if (sb2.Length > 0)
			{
				for (int i = sb2.Length; i < 16; i++)
					sb.Append("   ");

				sb.Append("\t" + sb2);
			}

			if (isContinued && (bytes.Length - contPos > 0))
			{
				byte[] newBytes = new byte[bytes.Length - contPos];
				Array.Copy(bytes, contPos, newBytes, 0, newBytes.Length);
				sb.AppendLine();
				sb.Append(GetStringHex(newBytes, length - contPos));
			}

			return sb.ToString();
		}
	}
}
