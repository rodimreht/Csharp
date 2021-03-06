/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/5/1
 * Time: 12:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.IO;

namespace Lextm.SharpSnmpLib
{
    /// <summary>
    /// Description of ByteTool.
    /// </summary>
    internal class ByteTool
    {
        internal static bool CompareRaw(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
            {
                return false;
            }
            
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i]) 
                {
                    return false;
                }
            }
            
            return true;
        }
        
        internal static byte[] ParseItems(params ISnmpData[] items)
        {
            MemoryStream result = new MemoryStream();
            foreach (ISnmpData item in items)
            {
                byte[] data = item.ToBytes();
                result.Write(data, 0, data.Length);
            }
            
            return result.ToArray();
        }
        
        internal static byte[] ParseItems(IEnumerable items)
        {
            MemoryStream result = new MemoryStream();
            foreach (ISnmpData item in items)
            {
                byte[] data = item.ToBytes();
                result.Write(data, 0, data.Length);
            }
            
            return result.ToArray();
        }        
        
		/// <summary>
		/// Thermidor 수정: SNMP에서는 실제로 크기에 2바이트를 사용하지 않음
		/// </summary>
		/// <param name="s"></param>
		/// <param name="a"></param>
        internal static void WriteMultiByteLength(Stream s, int a) // excluding initial octet
        {
            if (a <= 0)
            {
                s.WriteByte(0);
                return;
            }
			else if (a > 0xff)
			{
				byte[] buffer = new byte[0x10];
				int num = 0;
				while (a > 0)
				{
					buffer[num++] = (byte)(a & 0xff);
					a = a >> 8;
				}
				s.WriteByte((byte)(0x80 | num));
				while (num > 0)
				{
					int num2 = buffer[--num];
					s.WriteByte((byte)num2);
				}
			}
			else
			{
				s.WriteByte((byte)a);
			}
		}
        
        internal static int ReadMultiByteLength(MemoryStream m)
        {
            int current = m.ReadByte();
            return ReadLength(m, (byte)current);
        }
        
        // copied from universal
        private static int ReadLength(Stream s, byte x) // x is initial octet
        {
            if ((x & 0x80) == 0)
            {
                return (int)x;
            }
            
            int u = 0;
            int n = (int)(x & 0x7f);
            for (int j = 0; j < n; j++)
            {
                x = ReadByte(s);
                u = (u << 8) + (int)x;
            }
            
            return u;
        }
        
        // copied from universal
        private static byte ReadByte(Stream s)
        {
            int n = s.ReadByte();
            if (n == -1)
            {   
                throw (new SharpSnmpException("BER end of file"));
            }
            
            return (byte)n;
        }
        
        internal static byte[] ToBytes(SnmpType typeCode, byte[] raw)
        {
            MemoryStream result = new MemoryStream();
            result.WriteByte((byte)typeCode);
            ByteTool.WriteMultiByteLength(result, raw.Length);
            result.Write(raw, 0, raw.Length);
            return result.ToArray();
        }

        internal static Sequence PackMessage(VersionCode version, string community, ISnmpPdu pdu)
        {
            return new Sequence(new Integer32((int)version), new OctetString(community), pdu);
        }
    }
}
