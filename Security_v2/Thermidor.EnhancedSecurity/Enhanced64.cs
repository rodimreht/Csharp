using System;
using System.Text;
using Nets.IM.Common;

namespace Thermidor.EnhancedSecurity
{
	/// <summary>
	/// Enhanced64 ��ȣȭ Ŭ���� (by Thermidor on 2006-04-12)
	/// </summary>
	/// <remarks>
	/// Base64 ���ڵ��� Ȯ���Ͽ� ��ĪŰ ��� ��ȣȭ/��ȣȭ�� ������ Ŭ����<br/>
	/// (Base64�� URL �� HTTP ����� ���ԵǸ� �ȵǴ� ���ڵ��� ���ԵǾ� �־�
	/// �̸� URL�� ���� ������ �ٸ� ���ڷ� ġȯ�ϴ� �⺻ ���ڵ� �޼����,<br/>
	/// ���ڿ� Ű�� �Է¹޾� ��ĪŰ ��ȣȭ �˰����� ������ Ȯ�� �޼��带 �����Ѵ�.)
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
		/// Base64�� ���ڵ��ϰ� �� ����� Enhanced64�� �ٽ� ���ڵ��� ���ڿ��� ��´�.
		/// </summary>
		/// <param name="val">���ڵ��� ���� ���ڿ�</param>
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
		/// Base64�� ���ڵ��ϰ� �� ����� Enhanced64�� �ٽ� ���ڵ��� ���ڿ��� ��´�.
		/// </summary>
		/// <param name="sValue">���ڵ��� ���� ���ڿ�</param>
		public override string Encrypt(string sValue)
		{
			return Encode(sValue);
		}

		/// <summary>
		/// Enhanced64 ��ȣȭ ���ڿ��� ��´�.
		/// </summary>
		/// <param name="sKey">��ȣȭ Ű(8����Ʈ �̻�, 4�ڸ� �̻� ���Ϲ���)</param>
		/// <param name="sValue">��ȣȭ�� ���� ���ڿ�</param>
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
		/// Enhanced64�� ���ڵ��ϰ� �� ����� �ٽ� Base64�� ���ڵ��� ���ڿ��� ��´�.
		/// </summary>
		/// <param name="val">���ڵ��� ���ڿ�</param>
		/// <returns></returns>
		public string Decode(string val)
		{
			char[] cTemp = val.ToCharArray();
			for (int i = 0, iend = cTemp.Length; i < iend; i++)
				cTemp[i] = getBase64CharDec(cTemp[i], 'A');

			return Encoding.Default.GetString(Convert.FromBase64String(new string(cTemp)));
		}

		/// <summary>
		/// Enhanced64�� ���ڵ��ϰ� �� ����� �ٽ� Base64�� ���ڵ��� ���ڿ��� ��´�.
		/// </summary>
		/// <param name="sValue">���ڵ��� ���ڿ�</param>
		public override string Decrypt(string sValue)
		{
			return Decode(sValue);
		}

		/// <summary>
		/// Enhanced64 ��ȣȭ ���ڿ��� ��´�.
		/// </summary>
		/// <param name="sKey">��ȣȭ Ű(8����Ʈ �̻�, 4�ڸ� �̻� ���Ϲ���)</param>
		/// <param name="sValue">��ȣȭ�� ��ȣȭ�� ���ڿ�</param>
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

		// Ű ���ڿ��� Base64 �ε����� ��ü ���� ���Ѵ�.
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

		// ��ŷ�� ����� Ű�� ����ߴ����� ���θ� üũ�Ѵ�.
		private bool isStrongKey(string key)
		{
			// 8�ڸ� �̻��̾�� �Ѵ�.
			if (key.Trim().Length < 8) return false;

			// Base64�� ���Ե� �����̾�� �ϸ�, ���� ���ڰ� 4�ڸ� �̻� ���ӵǸ� �ȵȴ�.
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

		// Base64�� ���Ե� �������� üũ�Ѵ�.
		private bool isInBase64(char c)
		{
			for (int i = 0, iend = cBase64.Length; i < iend; i++)
				if (cBase64[i].Equals(c)) return true;

			return false;
		}

		// Base64 ���ڿ� �ε����� ���Ѵ�.
		private int getBase64Index(char c)
		{
			for (int i = 0, iend = cBase64.Length; i < iend; i++)
				if (cBase64[i].Equals(c)) return i;

			return -1;
		}

		// Enhanced64 ���ڿ� �ε����� ���Ѵ�.
		private int getEnhc64Index(char c)
		{
			for (int i = 0, iend = cEnhc64.Length; i < iend; i++)
				if (cEnhc64[i].Equals(c)) return i;

			return -1;
		}

		// Base64 ���ڸ� ��ȣȭ�Ͽ� Enhanced64 ���ڷ� ��ȯ�Ѵ�.
		private char getEnhanced64CharEnc(char baseChar, char keyChar)
		{
			int length = cEnhc64.Length;
			int idx = getBase64Index(baseChar) + getBase64Index(keyChar);
			if (idx >= length) idx -= length;
			idx += this.keyBase;
			if (idx >= length) idx -= length;

			return cEnhc64[idx];
		}

		// Enhanced64 ���ڸ� ��ȣȭ�Ͽ� Base64 ���ڷ� ��ȯ�Ѵ�.
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
