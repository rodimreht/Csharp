using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PEM
{
	/// <summary>
	/// CPEM에 대한 요약 설명입니다.
	/// </summary>
	public class CPEM
	{
		public CPEM()
		{
			//
			// TODO: 여기에 생성자 논리를 추가합니다.
			//
		}

		public static string ToPEM(string type, string data) 
		{ 
			string pem = Convert.ToBase64String(Encoding.ASCII.GetBytes(data)); 
			string header = String.Format("-----BEGIN {0}-----", type);  
			string footer = String.Format("-----END {0}-----", type);  
			return header + pem + footer; 
		} 

		public static byte[] FromPEM(string type, byte[] data) 
		{ 
			string pem = Encoding.ASCII.GetString(data); 
			string header = String.Format("-----BEGIN {0}-----", type);  
			string footer = String.Format("-----END {0}-----", type);  
			int start = pem.IndexOf(header) + header.Length;  
			int end = pem.IndexOf(footer, start);  
			string base64 = pem.Substring(start, (end - start));
			return Convert.FromBase64String(getSplitted(base64)); 
		} 

		private static string getSplitted(string str)
		{
			if (str.IndexOf("|") >= 0)
			{
				StringBuilder sb = new StringBuilder();
				string[] strs = str.Split(new char[] { '|' });
				for (int i = 0; i < strs.Length; i++)
					sb.Append(strs[i]);

				str = sb.ToString();
			}
			return str;
		}

		public static X509Certificate LoadCertificateFile(string filename) 
		{ 
			X509Certificate x509 = null; 
			using (FileStream fs = File.OpenRead(filename)) 
			{ 
				byte[] data = new byte[fs.Length]; 
				fs.Read (data, 0, data.Length); 
				if (data[0] != 0x30) 
				{ 
					// maybe it's ASCII PEM base64 encoded ?  
					data = FromPEM("CERTIFICATE", data);  
				} 
				if (data != null) 
					x509 = new X509Certificate(data);  
			} 
			return x509;  
		} 
	}
}
