namespace QwertyToKorean
{
	public class QwertyToKorean
	{
		private static string[] hF = new string[] { "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ" };
		private static string[] eF = new string[] { "r", "R", "s", "e", "E", "f", "a", "q", "Q", "t", "T", "d", "w", "W", "c", "z", "x", "v", "g" };
		private static string[] hM = new string[] { "ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ", "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ" };
		private static string[] eM = new string[] { "k", "o", "i", "O", "j", "p", "u", "P", "h", "hk", "ho", "hl", "y", "n", "nj", "np", "nl", "b", "m", "ml", "l" };
		private static string[] hL = new string[] { " ", "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄹ", "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅄ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ" };
		private static string[] eL = new string[] { " ", "r", "R", "rt", "s", "sw", "sg", "e", "f", "fr", "fa", "fq", "ft", "fx", "fv", "fg", "a", "q", "qt", "t", "T", "d", "w", "c", "z", "x", "v", "g" };

		public static string Convert(string src)
		{
			string rs = "";

			for (int i = 0; i < src.Length;/*i++*/)
			{
				if (src[i] == ' ')
				{
					rs += src[i++];
					continue;
				}

				int f = -1;

				for (int j = 0; j < eF.Length; j++)
					if (src[i].ToString() == eF[j])
					{
						i++;
						f = j;
						break;
					}

				if (i >= src.Length)
				{
					if (f >= 0)
						rs += hF[f];
					break;
				}

				int m = -1;

				for (int j = 0; j < eM.Length; j++)
				{
					if (eM[j].Length == 2)
					{
						if ((src.Substring(i).Length >= eM[j].Length) && (src.Substring(i, eM[j].Length) == eM[j]))
						{
							i += 2;
							m = j;
							break;
						}
					}
				}

				if (m < 0)
					for (int j = 0; j < eM.Length; j++)
						if (src[i].ToString() == eM[j])
						{
							i++;
							m = j;
							break;
						}

				if (f < 0 && m >= 0)
				{
					rs += hM[m];
					continue;
				}

				if (f >= 0 && m < 0)
				{
					rs += hF[f];
					continue;
				}

				if (i >= src.Length)
				{
					int ch = combine(f, m, 0);
					rs += (char)ch;
					break;
				}

				if (src[i] == ' ')
				{
					int ch = combine(f, m, 0);
					rs += (char)ch;
					continue;
				}

				int l = -1;

				if (src.Length > i + 2)
				{
					for (int j = 0; j < eM.Length; j++)
					{
						if (src[i + 2].ToString() == eM[j])
						{
							l = -2;
							break;
						}
					}
				}

				if (l == -1)
				{
					for (int j = 0; j < eL.Length; j++)
					{
						if (eL[j].Length == 2)
						{
							if ((src.Substring(i).Length >= eL[j].Length) && src.Substring(i, eL[j].Length) == eL[j])
							{
								i += 2;
								l = j;
								break;
							}
						}
					}
				}

				if (l < 0)
				{
					l = -1;
					if (src.Length > i + 1)
					{
						for (int j = 0; j < eM.Length; j++)
						{
							if (src[i + 1].ToString() == eM[j])
							{
								l = -2;
								break;
							}
						}
					}

					if (l == -1)
					{
						for (int j = 0; j < eL.Length; j++)
						{
							if (src[i].ToString() == eL[j])
							{
								i++;
								l = j;
								break;
							}
						}
					}
				}

				if (f < 0 && m < 0 && l < 0)
				{
					rs += src[i++];
					continue;
				}

				l = (l < 0) ? 0 : l;
				int ch2 = combine(f, m, l);
				rs += (char)ch2;
			}

			return rs;
		}

		private static int combine(int f, int m, int l)
		{
			return 0xAC00 + (f * 21 * 28) + (m * 28) + l;
		}

		public static string Revert(string src)
		{
			string rs = "";

			for (int i = 0; i < src.Length; i++)
			{
				if (src[i] == ' ')
				{
					rs += src[i];
					continue;
				}

				bool isSkip = false;
				// 초성만 있는 경우
				for (int j = 0; j < eF.Length; j++)
					if (src[i].ToString() == hF[j])
					{
						rs += eF[j];
						isSkip = true;
						break;
					}

				if (isSkip) continue;

				// 중성만 있는 경우
				for (int j = 0; j < eM.Length; j++)
					if (src[i].ToString() == hM[j])
					{
						rs += eM[j];
						isSkip = true;
						break;
					}

				if (isSkip) continue;

				// 종성(복자음)만 있는 경우
				for (int j = 0; j < eL.Length; j++)
					if (src[i].ToString() == hL[j])
					{
						rs += eL[j];
						isSkip = true;
						break;
					}

				if (isSkip) continue;

				int f, m, l;
				separate(src[i], out f, out m, out l);

				if (f < 0 && m < 0 && l <= 0)
				{
					rs += src[i];
					continue;
				}

				if (l <= 0)
				{
					if (m < 0)
						rs += eF[f];
					else
						rs += eF[f] + eM[m];
				}
				else
					rs += eF[f] + eM[m] + eL[l];
			}

			return rs;
		}

		private static void separate(int k, out int f, out int m, out int l)
		{
			int i = k - 0xAC00;
			l = i % 28;
			if (l == 0)
			{
				f = (i/28)/21;
				m = (i/28)%21;
			}
			else
			{
				i -= l;
				f = (i / 28) / 21;
				m = (i / 28) % 21;
			}
		}
	}
}
