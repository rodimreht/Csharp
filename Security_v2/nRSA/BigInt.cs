using System;

namespace PublicKey
{
	/// <summary>
	/// BigInt에 대한 요약 설명입니다.
	/// </summary>
	public class BigInt
	{
		private const int biRadixBits = 16;
		private const int bitsPerDigit = biRadixBits;
		private const int biRadix = 1 << 16; // = 2^16 = 65536
		private const int biHalfRadix = biRadix >> 1;
		private const long biRadixSquared = ((long) biRadix) * ((long) biRadix);
		private const int maxDigitVal = biRadix - 1;

		// 128bit: 19, 384bit: 51, 512bit: 76, 1024bit: 130, 2048bit: 260
		private const int maxDigits = 76;
		private static int[] ZERO_ARRAY = getZeroArray();
		private static BigInt bigOne = getBigOne();

		public int[] digits;
		public bool isNeg;

		private static int[] highBitMasks = new int[] {
			0x0000, 0x8000, 0xC000, 0xE000, 0xF000, 0xF800,
			0xFC00, 0xFE00, 0xFF00, 0xFF80, 0xFFC0, 0xFFE0,
			0xFFF0, 0xFFF8, 0xFFFC, 0xFFFE, 0xFFFF};

		private static int[] lowBitMasks = new int[] {
			0x0000, 0x0001, 0x0003, 0x0007, 0x000F, 0x001F,
			0x003F, 0x007F, 0x00FF, 0x01FF, 0x03FF, 0x07FF,
			0x0FFF, 0x1FFF, 0x3FFF, 0x7FFF, 0xFFFF};

		private static char[] hexToChar = new char[]{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'a', 'b', 'c', 'd', 'e', 'f'};

		public BigInt()
		{
			this.digits = (int[]) ZERO_ARRAY.Clone();
			this.isNeg = false;
		}

		public BigInt(bool flag)
		{
			if (flag)
				this.digits = null;
			else
				this.digits = (int[]) ZERO_ARRAY.Clone();

			this.isNeg = false;
		}

		private static int[] getZeroArray()
		{
			int[] arr = new int[maxDigits];
			for (int iza = 0; iza < arr.Length; iza++)
				arr[iza] = 0;

			return arr;
		}

		private static BigInt getBigOne()
		{
			BigInt bi = new BigInt();
			bi.digits[0] = 1;
			return bi;
		}

		public static int charToHex(char c)
		{
			int result;

			if (c >= '0' && c <= '9') 
				result = c - '0';
			else if (c >= 'A' && c <= 'Z') 
				result = 10 + c - 'A';
			else 
				result = 0;

			return result;
		}

		public static int hexToDigit(string s)
		{
			int result = 0;
			int sl = Math.Min(s.Length, 4);
			char[] chs = s.ToUpper().ToCharArray();

			for (int i = 0; i < sl; ++i) 
			{
				result <<= 4;
				result |= charToHex(chs[i]);
			}
			return result;
		}

		public static BigInt biFromHex(string s)
		{
			BigInt result = new BigInt();
			int sl = s.Length;
			for (int i = sl, j = 0; i > 0; i -= 4, ++j) 
			{
				string part = s.Substring(Math.Max(i - 4, 0), Math.Min(i, 4));
				result.digits[j] = hexToDigit(part);
			}
			return result;
		}

		public static BigInt biFromNumber(long i)
		{
			BigInt result = new BigInt();
			result.isNeg = i < 0;
			i = Math.Abs(i);
			int j = 0;
			while (i > 0) 
			{
				result.digits[j++] = (int) (i & maxDigitVal);
				i = (long) Math.Floor((double)i / biRadix);
			}
			return result;
		}

		public static int biHighIndex(BigInt x)
		{
			int[] xDigits = x.digits;
			int result = xDigits.Length - 1;
			while (result > 0 && xDigits[result] == 0) --result;
			return result;
		}

		public static BigInt biCopy(BigInt bi)
		{
			BigInt result = new BigInt(true);
			result.digits = (int[]) bi.digits.Clone();
			result.isNeg = bi.isNeg;
			return result;
		}

		public static BigInt[] biDivideModulo(BigInt x, BigInt y)
		{
			int nb = biNumBits(x);
			int tb = biNumBits(y);
			bool origYIsNeg = y.isNeg;
			BigInt q, r;
			if (nb < tb) 
			{
				// |x| < |y|
				if (x.isNeg) 
				{
					q = biCopy(bigOne);
					q.isNeg = !y.isNeg;
					x.isNeg = false;
					y.isNeg = false;
					r = biSubtract(y, x);
					// Restore signs, 'cause they're references.
					x.isNeg = true;
					y.isNeg = origYIsNeg;
				} 
				else 
				{
					q = new BigInt();
					r = biCopy(x);
				}
				return new BigInt[] {q, r};
			}

			q = new BigInt();
			r = x;

			// Normalize Y.
			int t = (int) Math.Ceiling((double)tb / bitsPerDigit) - 1;
			int lambda = 0;
			while (y.digits[t] < biHalfRadix) 
			{
				y = biShiftLeft(y, 1);
				++lambda;
				++tb;
				t = (int) Math.Ceiling((double)tb / bitsPerDigit) - 1;
			}
			// Shift r over to keep the quotient constant. We'll shift the
			// remainder back at the end.
			r = biShiftLeft(r, lambda);
			nb += lambda; // Update the bit count for x.
			int n = (int) Math.Ceiling((double)nb / bitsPerDigit) - 1;

			BigInt b = biMultiplyByRadixPower(y, n - t);
			while (biCompare(r, b) != -1) 
			{
				++q.digits[n - t];
				r = biSubtract(r, b);
			}
			for (int i = n; i > t; --i)
			{
				int[] qDigits = q.digits;
				int[] rDigits = r.digits;
				int[] yDigits = y.digits;
				int rLen = rDigits.Length;
				int yLen = yDigits.Length;
				int ri = (i >= rLen) ? 0 : rDigits[i];
				int ri1 = (i - 1 >= rLen) ? 0 : rDigits[i - 1];
				int ri2 = (i - 2 >= rLen) ? 0 : rDigits[i - 2];
				int yt = (t >= yLen) ? 0 : yDigits[t];
				int yt1 = (t - 1 >= yLen) ? 0 : yDigits[t - 1];
				if (ri == yt)
					qDigits[i - t - 1] = maxDigitVal;
				else 
					qDigits[i - t - 1] = (int) Math.Floor(((double)ri * biRadix + ri1) / yt);

				long c1 = (long)qDigits[i - t - 1] * (((long)yt * biRadix) + yt1);
				long c2 = ((long)ri * biRadixSquared) + (((long)ri1 * biRadix) + ri2);
				while (c1 > c2) 
				{
					--qDigits[i - t - 1];
					c1 = (long)qDigits[i - t - 1] * (((long)yt * biRadix) + yt1);
					c2 = ((long)ri * biRadixSquared) + (((long)ri1 * biRadix) + ri2);
				}

				b = biMultiplyByRadixPower(y, i - t - 1);
				r = biSubtract(r, biMultiplyDigit(b, qDigits[i - t - 1]));
				if (r.isNeg) 
				{
					r = biAdd(r, b);
					--qDigits[i - t - 1];
				}
			}
			r = biShiftRight(r, lambda);
			// Fiddle with the signs and stuff to make sure that 0 <= r < y.
			q.isNeg = x.isNeg != origYIsNeg;
			if (x.isNeg) 
			{
				if (origYIsNeg) 
					q = biAdd(q, bigOne);
				else 
					q = biSubtract(q, bigOne);

				y = biShiftRight(y, lambda);
				r = biSubtract(y, r);
			}
			// Check for the unbelievably stupid degenerate case of r == -0.
			if (r.digits[0] == 0 && biHighIndex(r) == 0) r.isNeg = false;

			return new BigInt[] {q, r};
		}

		public static BigInt biDivide(BigInt x, BigInt y)
		{
			return biDivideModulo(x, y)[0];
		}

		public static int biNumBits(BigInt x)
		{
			int n = biHighIndex(x);
			int d = x.digits[n];
			int m = (n + 1) * bitsPerDigit;
			int result;
			for (result = m; result > m - bitsPerDigit; --result) 
			{
				if ((d & 0x8000) != 0) break;
				d <<= 1;
			}
			return result;
		}

		public static BigInt biSubtract(BigInt x, BigInt y)
		{
			BigInt result;
			if (x.isNeg != y.isNeg) 
			{
				y.isNeg = !y.isNeg;
				result = biAdd(x, y);
				y.isNeg = !y.isNeg;
			} 
			else 
			{
				result = new BigInt();
				int n, c;
				int xLen = x.digits.Length;
				c = 0;
				int[] xDigits = x.digits;
				int[] yDigits = y.digits;
				int[] resultDigits = result.digits;
				for (int i = 0; i < xLen; ++i) 
				{
					n = xDigits[i] - yDigits[i] + c;
					resultDigits[i] = n % biRadix;
					// Stupid non-conforming modulus operation.
					if (resultDigits[i] < 0) resultDigits[i] += biRadix;
					c = (n < 0) ? -1 : 0;
				}
				// Fix up the negative sign, if any.
				if (c == -1) 
				{
					c = 0;
					for (int i = 0; i < xLen; ++i) 
					{
						n = 0 - resultDigits[i] + c;
						resultDigits[i] = n % biRadix;
						// Stupid non-conforming modulus operation.
						if (resultDigits[i] < 0) resultDigits[i] += biRadix;
						c = (n < 0) ? -1 : 0;
					}
					// Result is opposite sign of arguments.
					result.isNeg = !x.isNeg;
				} 
				else
				{
					// Result is same sign.
					result.isNeg = x.isNeg;
				}
			}
			return result;
		}

		public static BigInt biAdd(BigInt x, BigInt y)
		{
			BigInt result;

			if (x.isNeg != y.isNeg) 
			{
				y.isNeg = !y.isNeg;
				result = biSubtract(x, y);
				y.isNeg = !y.isNeg;
			}
			else
			{
				result = new BigInt();
				int c = 0;
				int n;
				int xLen = x.digits.Length;
				int[] xDigits = x.digits;
				int[] yDigits = y.digits;
				int[] resultDigits = result.digits;
				for (int i = 0; i < xLen; ++i) 
				{
					n = xDigits[i] + yDigits[i] + c;
					resultDigits[i] = n % biRadix;
					c = (n >= biRadix) ? 1 : 0;
				}
				result.isNeg = x.isNeg;
			}
			return result;
		}

		public static BigInt biShiftLeft(BigInt x, int n)
		{
			int digitCount = (int) Math.Floor((double)n / bitsPerDigit);
			BigInt result = new BigInt();
			int[] resultDigits = result.digits;
			int resultLen = resultDigits.Length;
			arrayCopy(x.digits, 0, resultDigits, digitCount,
				resultLen - digitCount);
			int bits = n % bitsPerDigit;
			int rightBits = bitsPerDigit - bits;
			int i = 0, i1 = 0;
			for (i = resultLen - 1, i1 = i - 1; i > 0; --i, --i1) 
			{
				resultDigits[i] = ((resultDigits[i] << bits) & maxDigitVal) |
					((resultDigits[i1] & highBitMasks[bits]) >> (rightBits));
			}
			resultDigits[0] = ((resultDigits[i] << bits) & maxDigitVal);
			result.isNeg = x.isNeg;
			return result;
		}

		private static void arrayCopy(int[] src, int srcStart, int[] dest, int destStart, int n)
		{
			int m = Math.Min(srcStart + n, src.Length);
			for (int i = srcStart, j = destStart; i < m; ++i, ++j) 
				dest[j] = src[i];
		}

		private static BigInt biMultiplyByRadixPower(BigInt x, int n)
		{
			BigInt result = new BigInt();
			arrayCopy(x.digits, 0, result.digits, n, result.digits.Length - n);
			return result;
		}

		public static int biCompare(BigInt x, BigInt y)
		{
			if (x.isNeg != y.isNeg) 
			{
				return (x.isNeg ? -1 : 1);
			}
			int[] xDigits = x.digits;
			int[] yDigits = y.digits;
			int xLen = x.digits.Length;
			for (int i = xLen - 1; i >= 0; --i) 
			{
				if (xDigits[i] != yDigits[i]) 
				{
					if (x.isNeg) 
						return ((xDigits[i] > yDigits[i]) ? -1 : 1);
					else 
						return ((xDigits[i] < yDigits[i]) ? -1 : 1);
				}
			}
			return 0;
		}

		private static BigInt biMultiplyDigit(BigInt x, int y)
		{
			int n, c;
			long uv;

			BigInt result = new BigInt();
			int[] resultDigits = result.digits;
			n = biHighIndex(x);
			c = 0;
			for (int j = 0; j <= n; ++j) 
			{
				uv = resultDigits[j] + ((long)x.digits[j] * y) + c;
				resultDigits[j] = (int) (uv & maxDigitVal);
				c = (int) (uv >> biRadixBits);
				//c = Math.floor(uv / biRadix);
			}
			resultDigits[1 + n] = c;
			return result;
		}

		public static BigInt biShiftRight(BigInt x, int n)
		{
			int digitCount = (int) Math.Floor((double)n / bitsPerDigit);
			BigInt result = new BigInt();
			int[] resultDigits = result.digits;
			int resultLen = resultDigits.Length;
			int xLen = x.digits.Length;
			arrayCopy(x.digits, digitCount, resultDigits, 0,
				xLen - digitCount);
			int bits = n % bitsPerDigit;
			int leftBits = bitsPerDigit - bits;
			for (int i = 0, i1 = i + 1; i < resultLen - 1; ++i, ++i1) 
			{
				resultDigits[i] = (resultDigits[i] >> bits) |
					((resultDigits[i1] & lowBitMasks[bits]) << leftBits);
			}
			resultDigits[resultLen - 1] >>= bits;
			result.isNeg = x.isNeg;
			return result;
		}

		public static string biToHex(BigInt x)
		{
			string result = "";
			int n = biHighIndex(x);
			for (int i = n; i > -1; --i) 
			{
				result += digitToHex(x.digits[i]);
			}
			return result;
		}

		private static string digitToHex(int n)
		{
			int mask = 0xf;
			string result = "";
			for (int i = 0; i < 4; ++i) 
			{
				result += hexToChar[n & mask].ToString();
				n >>= 4;
			}
			return reverseStr(result);
		}

		private static string reverseStr(string s)
		{
			string result = "";
			for (int i = s.Length - 1; i > -1; --i) 
				result += s.Substring(i, 1);

			return result;
		}

		public static BigInt biDivideByRadixPower(BigInt x, int n)
		{
			BigInt result = new BigInt();
			arrayCopy(x.digits, n, result.digits, 0, result.digits.Length - n);
			return result;
		}

		public static BigInt biMultiply(BigInt x, BigInt y)
		{
			BigInt result = new BigInt();
			int c;
			int n = biHighIndex(x);
			int t = biHighIndex(y);
			int k;
			long uv;
			int[] xDigits = x.digits;
			int[] yDigits = y.digits;
			int[] resultDigits = result.digits;

			for (int i = 0; i <= t; ++i) 
			{
				c = 0;
				k = i;
				for (int j = 0; j <= n; ++j, ++k) 
				{
					uv = resultDigits[k] + ((long)xDigits[j] * yDigits[i]) + c;
					resultDigits[k] = (int) (uv & maxDigitVal);
					c = (int) (uv >> biRadixBits);
					//c = Math.floor(uv / biRadix);
				}
				resultDigits[i + n + 1] = c;
			}
			// Someone give me a logical xor, please.
			result.isNeg = (x.isNeg != y.isNeg);
			return result;
		}

		/*
		private static BigInt biMultiplyMod(BigInt x, BigInt y, BigInt m)
		{
			return biModulo(biMultiply(x, y), m);
		}

		private static BigInt biModulo(BigInt x, BigInt y)
		{
			return biDivideModulo(x, y)[1];
		}
		*/

		public static BigInt biModuloByRadixPower(BigInt x, int n)
		{
			BigInt result = new BigInt();
			arrayCopy(x.digits, 0, result.digits, 0, n);
			return result;
		}
	}
}
