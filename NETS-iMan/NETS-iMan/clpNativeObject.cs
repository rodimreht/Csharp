using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NETS_iMan
{
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct clpNativeObject
	{
		public short m_header; //= 0x0002;
		public string m_fileName;
		public string m_filePath;
		public short m_question1; //= 0x0000;
		public short m_question2; //= 0x0003;
		public int m_length;
		public string m_fullPath;
		public int m_fileSize;
		public byte[] m_data;
		public short m_terminates; //= 0x0000;
	}

	public class NativeObject
	{
		[DllImport("wsock32.dll")]
		public static extern int htonl(int hLong);

		[DllImport("wsock32.dll")]
		public static extern int ntohl(int nLong);

		public static clpNativeObject GetObject(MemoryStream ms)
		{
			byte[] bytes = ms.ToArray();
			int pos = 0, pos2 = 0;

			clpNativeObject obj = new clpNativeObject();
			obj.m_header = BitConverter.ToInt16(bytes, 0);
			pos += 2;
			pos2 = getNullPosition(bytes, pos);
			obj.m_fileName = Encoding.Default.GetString(bytes, pos, pos2 - pos);
			pos = pos2;
			pos2 = getNullPosition(bytes, ++pos);
			obj.m_filePath = Encoding.Default.GetString(bytes, pos, pos2 - pos);
			pos = pos2;
			obj.m_question1 = BitConverter.ToInt16(bytes, ++pos);
			pos += 2;
			obj.m_question2 = BitConverter.ToInt16(bytes, pos);
			pos += 2;
			obj.m_length = BitConverter.ToInt32(bytes, pos);
			pos += 4;
			pos2 = getNullPosition(bytes, pos);
			obj.m_fullPath = Encoding.Default.GetString(bytes, pos, pos2 - pos);
			pos = pos2;
			obj.m_fileSize = BitConverter.ToInt32(bytes, ++pos);
			pos += 4;
			obj.m_data = new byte[obj.m_fileSize];
			Array.Copy(bytes, pos, obj.m_data, 0, obj.m_fileSize);
			obj.m_terminates = BitConverter.ToInt16(bytes, pos + obj.m_fileSize);

			return obj;
		}

		private static int getNullPosition(byte[] bytes, int startPos)
		{
			for (int i = startPos; i < bytes.Length; i++)
			{
				if (bytes[i] == 0) return i;
			}
			return -1;
		}
	}
}
