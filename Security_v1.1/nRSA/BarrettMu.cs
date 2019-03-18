namespace PublicKey
{
	/// <summary>
	/// Barrett에 대한 요약 설명입니다.
	/// </summary>
	public class BarrettMu
	{
		private BigInt modulus;
		private int k;
		private BigInt mu;
		private BigInt bkplus1;

		public BarrettMu(BigInt m)
		{
			this.modulus = BigInt.biCopy(m);
			this.k = BigInt.biHighIndex(this.modulus) + 1;
			
			BigInt b2k = new BigInt();
			b2k.digits[2 * this.k] = 1; // b2k = b^(2k)
			
			this.mu = BigInt.biDivide(b2k, this.modulus);
			
			this.bkplus1 = new BigInt();
			this.bkplus1.digits[this.k + 1] = 1; // bkplus1 = b^(k+1)
		}

		public BigInt modulo(BigInt x)
		{
			BigInt q1 = BigInt.biDivideByRadixPower(x, this.k - 1);
			BigInt q2 = BigInt.biMultiply(q1, this.mu);
			BigInt q3 = BigInt.biDivideByRadixPower(q2, this.k + 1);
			BigInt r1 = BigInt.biModuloByRadixPower(x, this.k + 1);
			BigInt r2term = BigInt.biMultiply(q3, this.modulus);
			BigInt r2 = BigInt.biModuloByRadixPower(r2term, this.k + 1);
			BigInt r = BigInt.biSubtract(r1, r2);
			if (r.isNeg) 
				r = BigInt.biAdd(r, this.bkplus1);

			bool rgtem = BigInt.biCompare(r, this.modulus) >= 0;
			while (rgtem) 
			{
				r = BigInt.biSubtract(r, this.modulus);
				rgtem = BigInt.biCompare(r, this.modulus) >= 0;
			}
			return r;
		}

		public BigInt multiplyMod(BigInt x, BigInt y)
		{
			BigInt xy = BigInt.biMultiply(x, y);
			return this.modulo(xy);
		}

		public BigInt powMod(BigInt x, BigInt y)
		{
			BigInt result = new BigInt();
			result.digits[0] = 1;
			BigInt a = x;
			BigInt k = y;
			while (true) 
			{
				if ((k.digits[0] & 1) != 0) result = this.multiplyMod(result, a);
				k = BigInt.biShiftRight(k, 1);
				if (k.digits[0] == 0 && BigInt.biHighIndex(k) == 0) break;
				a = this.multiplyMod(a, a);
			}
			return result;
		}
	}
}
