using System.Collections;
using System.Text;

namespace PublicKey
{
	/// <summary>
	/// nRSA에 대한 요약 설명입니다.
	/// </summary>
	public class nRSA
	{
		private BigInt e;
		private BigInt d;
		private BigInt m;
		public int chunkSize;
		private BarrettMu barrett;

        /// <summary>
        /// 공개키만 있는 경우: 암호화만 가능
        /// </summary>
        /// <param name="encExponent"></param>
        /// <param name="modulus"></param>
		public nRSA(string encExponent, string modulus)
		{
			this.e = BigInt.biFromHex(encExponent);
			this.d = null;
			this.m = BigInt.biFromHex(modulus);
			// We can do two bytes per digit, so
			// chunkSize = 2 * (number of digits in modulus - 1).
			// Since biHighIndex returns the high index, not the number of digits, 1 has
			// already been subtracted.
			this.chunkSize = 2 * BigInt.biHighIndex(this.m);
			this.barrett = new BarrettMu(this.m);
		}

	    /// <summary>
	    /// 개인키, 공개키 모두 있는 경우: 암/복호화 가능
	    /// </summary>
	    /// <param name="encExponent"></param>
	    /// <param name="decExponent"></param>
	    /// <param name="modulus"></param>
	    /// <remarks>
	    /// encExponent가 null인 경우 개인키만 있는 경우, 복호화만 가능
	    /// </remarks>
	    public nRSA(string encExponent, string decExponent, string modulus)
	    {
            if (encExponent != null) this.e = BigInt.biFromHex(encExponent);
	        this.d = BigInt.biFromHex(decExponent);
	        this.m = BigInt.biFromHex(modulus);
	        // We can do two bytes per digit, so
	        // chunkSize = 2 * (number of digits in modulus - 1).
	        // Since biHighIndex returns the high index, not the number of digits, 1 has
	        // already been subtracted.
	        this.chunkSize = 2 * BigInt.biHighIndex(this.m);
	        this.barrett = new BarrettMu(this.m);
	    }

        // Altered by Rob Saunders (rob@robsaunders.net). New routine pads the
        // string after it has been converted to an array. This fixes an
        // incompatibility with Flash MX's ActionScript.
        public string Encrypt(string s)
		{
			int sl = s.Length;
			ArrayList a = new ArrayList();
			int i = 0;

			char[] chs = s.ToCharArray();
			while (i < sl) 
			{
				a.Add((int) chs[i]);
				i++;
			}

			while (a.Count % this.chunkSize != 0) 
				a.Add(0);

			int al = a.Count;
			string result = "";
			int j, k;
			for (i = 0; i < al; i += this.chunkSize) 
			{
				BigInt block = new BigInt();
				j = 0;
				for (k = i; k < i + this.chunkSize; ++j) 
				{
					block.digits[j] = (int) a[k++];
					block.digits[j] += ((int) a[k++]) << 8;
				}
				BigInt crypt = this.barrett.powMod(block, this.e);
				string text = BigInt.biToHex(crypt);
				result += text + " ";
			}
			return result.Substring(0, result.Length - 1); // Remove last space.
		}

		public string Decrypt(string s)
		{
			string[] blocks = s.Split(new char[]{' '});
			StringBuilder result = new StringBuilder();
			int i, j;
			BigInt block;
			for (i = 0; i < blocks.Length; ++i) 
			{
				BigInt bi = BigInt.biFromHex(blocks[i]);
				block = this.barrett.powMod(bi, this.d);
				for (j = 0; j <= BigInt.biHighIndex(block); ++j) 
				{
					char[] chars = new char[]{(char) (block.digits[j] & 255), (char) (block.digits[j] >> 8)};
					result.Append(chars) ;
				}
			}
			// Remove trailing null, if any.
			if (result[result.Length - 1] == 0) 
				return result.ToString(0, result.Length - 1);
			else
				return result.ToString();
		}
	}
}
