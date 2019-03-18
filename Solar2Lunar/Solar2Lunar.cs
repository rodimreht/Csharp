using System;

namespace Solar2Lunar
{
	/// <summary>
	/// SolarToLunar에 대한 요약 설명입니다.
	/// </summary>
	public class Solar2Lunar
	{
		private String[] m_LunarArray = new String[171];
		private int[] m_LunarType = new int[5];

		/// <summary>
		/// 생성자
		/// </summary>
		public Solar2Lunar()
		{
			m_InitLunarArray();
		}

		/// <summary>
		/// 양력을 음력으로 변환하는 공개 함수
		/// </summary>
		/// <param name="aDate1">날짜 문자열 (예: 20020301)</param>
		public void SolarToLunar(ref String aDate1)
		{
			if (aDate1.Trim().Length > 0)
			{
				aDate1 = mf_ConvertLunar(aDate1);
			}
		}

		/// <summary>
		/// 음력을 양력으로 변환하는 공개 함수
		/// </summary>
		/// <param name="aDate1">날짜 문자열 (예: 20020229)</param>
		public void LunarToSolar(ref String aDate1)
		{
			if (aDate1.Trim().Length > 0)
			{
				aDate1 = mf_ConvertSolar(aDate1);
			}
		}

		/// <summary>
		/// 양력을 음력으로 변환하는 주 함수
		/// </summary>
		/// <param name="aDate1">날짜 문자열 (예: 20020301)</param>
		/// <returns>변환이 완료된 음력 날짜</returns>
		private String mf_ConvertLunar(String aDate1)
		{
			int xYear = 0, xMonth = 0, xDay = 0, xTotDay = 0;

			xYear = int.Parse(aDate1);
			if ((xYear < 18810130) || (xYear > 20501231))
			{
				return "범위를 벗어났습니다.  1881/01/30 ~ 2050/12/31";
			}

			xYear = int.Parse(aDate1.Substring(0, 4));
			xMonth = int.Parse(aDate1.Substring(4, 2));
			xDay = int.Parse(aDate1.Substring(6, 2));

			xTotDay = mf_NowTotalDay(xYear, xMonth, xDay);

			return mf_SolToLunar(xTotDay);
		}

		/// <summary>
		/// 음력을 양력으로 변환하는 주 함수
		/// </summary>
		/// <param name="aDate1">날짜 문자열 (예: 20020229)</param>
		/// <returns>변환이 완료된 양력 날짜</returns>
		private String mf_ConvertSolar(String aDate1)
		{
			int xYear = 0, xMonth = 0, xDay = 0, xTotDay = 0;

			xYear = int.Parse(aDate1);
			if ((xYear < 18810101) || (xYear > 20501118))
			{
				return "범위를 벗어났습니다.  1881/01/01 ~ 2050/11/18";
			}

			xYear = int.Parse(aDate1.Substring(0, 4));
			xMonth = int.Parse(aDate1.Substring(4, 2));
			xDay = int.Parse(aDate1.Substring(6, 2));

			xTotDay = mf_LunarToSol(xYear, xMonth, xDay);

			return mf_SolarDay(xTotDay);
		}

		/// <summary>
		/// 현재 일자까지의 총 일수 구하기
		/// </summary>
		/// <param name="aYY">년</param>
		/// <param name="aMM">월</param>
		/// <param name="aDD">일</param>
		/// <returns>총 일수</returns>
		private int mf_NowTotalDay(int aYY, int aMM, int aDD)
		{
			int xYear = 0, iTemp = 0;

			xYear = aYY - 1881;

			// 이전 해까지의 총 일수를 구한다.
			iTemp = (xYear * 365) + (xYear / 4) - ((aYY / 100) - (1881 / 100)) + ((aYY / 400) - (1881 / 400));

			// 해당 해의 해당 날짜까지의 일수를 구해 더한다.
			iTemp += mf_MonthDays(aYY, aMM) + aDD;

			return iTemp;
		}

		/// <summary>
		/// 이전 달까지의 총 일수 구하기
		/// </summary>
		/// <param name="aYY">년</param>
		/// <param name="aMM">월</param>
		/// <returns>이전 달까지의 총 일수</returns>
		private int mf_MonthDays(int aYY, int aMM)
		{
			int iTemp = 0;

			switch (aMM)
			{
				case 1:		iTemp = 0;		break;
				case 2:		iTemp = 31;		break;
				case 3:		iTemp = 59;		break;
				case 4:		iTemp = 90;		break;
				case 5:		iTemp = 120;	break;
				case 6:		iTemp = 151;	break;
				case 7:		iTemp = 181;	break;
				case 8:		iTemp = 212;	break;
				case 9:		iTemp = 243;	break;
				case 10:	iTemp = 273;	break;
				case 11:	iTemp = 304;	break;
				case 12:	iTemp = 334;	break;
			}

			if (aMM > 2)
			{
				if ((((aYY / 4) == 0) && ((aYY / 100) != 0)) || ((aYY / 400) == 0))
				{
					iTemp++;	// 윤년일 경우 2월은 하루가 더 많음(즉 29일)
				}
			}

			return iTemp;
		}

		/// <summary>
		/// 양력을 음력으로 변환하는 주 보조 함수
		/// </summary>
		/// <param name="iTotal">현재까지의 총 일수</param>
		/// <returns>변환이 완료된 음력 날짜</returns>
		private String mf_SolToLunar(int iTotal)
		{
			int xRet = 0, xYear = 0, xMonth = 0;
			bool xYoon = false;
			String strYoon = "";

			iTotal -= 29;	// 1881년 양력과 음력의 차이

			for (xYear = 1; xYear <= 170; xYear++)	// 해당년도 까지의 날짜를 계속 뺀다
			{
				xRet = int.Parse(m_LunarArray[xYear].Substring(13));	// 해당되는 해의 날짜수

				if (iTotal <= xRet)
					break;

				iTotal -= xRet;
			}

			xRet = 0;
			for (xMonth = 1; xMonth <= 13; xMonth++)	// 해당되는 달 이전까지의 날짜를 뺀다.
			{
				xRet = int.Parse(m_LunarArray[xYear - 1].Substring(xMonth - 1, 1));	// 해당되는 달의 type
				if (xRet > 2)
					xYoon = true;
				else if (xRet == 0)
					break;

				xRet = m_LunarType[xRet];	// 해당되는 달의 날짜수

				if (iTotal <= xRet)
					break;

				iTotal -= xRet;
			}

			if (xYoon)
			{
				xMonth -= 1;
				strYoon = "(윤)";
			}
			else
				strYoon = "";

			xYear += 1880;

			return (xYear.ToString("0000") + xMonth.ToString("00") + iTotal.ToString("00") + strYoon);
		}

		/// <summary>
		/// 음력을 양력으로 바꾸는 주 보조 함수
		/// </summary>
		/// <param name="xYear">년</param>
		/// <param name="xMonth">월</param>
		/// <param name="iTotal">일</param>
		/// <returns>현재까지의 총 일수</returns>
		private int mf_LunarToSol(int xYear, int xMonth, int iTotal)
		{
			int xRet = 0, xM = 0, xY = 0;
			bool xYoon = false;
			String strYoon = "";

			xYear -= 1880;

			for (xM = 1; xM < xMonth; xM++)
			{
				xRet = int.Parse(m_LunarArray[xYear - 1].Substring(xM - 1, 1));

				if (xRet > 2)
					xYoon = true;
				else if (xRet == 0)
					break;
			}

			if (xYoon)
				xMonth++;

			for (xM = xMonth - 1; xM >= 1; xM--)
			{
				xRet = int.Parse(m_LunarArray[xYear - 1].Substring(xM - 1, 1));

				if (xRet == 0)
					break;

				xRet = m_LunarType[xRet];
				iTotal += xRet;
			}

			for (xY = xYear - 1; xY >= 1; xY--)
			{
				xRet = int.Parse(m_LunarArray[xYear - 1].Substring(13));

				iTotal += xRet;
			}

			return (iTotal + 29); // 1881년 양력과 음력의 차이
		}

		private String mf_SolarDay(int iTotal)
		{
			int ii = 0, xYear = 0, xMod = 0;

			xYear = (iTotal / 365);
			xMod = (iTotal % 365);
			xMod -= ((xYear / 4) - (((xYear + 1881) / 100) - (1881 / 100)) + (((xYear + 1881) / 400) - (1881 / 400)));

			ii = 1;

			while (true)
			{
				if (xMod > 31)
				{
					xMod -= 31;
					ii++;
				}
				else
					break;

				if (xMod > 28)
				{
					xMod -= 28;
					ii++;
				}
				else
					break;

				if (xMod > 31)
				{
					xMod -= 31;
					ii++;
				}
				else
					break;

				if (xMod > 30)
				{
					xMod -= 30;
					ii++;
				}
				else
					break;

				if (xMod > 31)
				{
					xMod -= 31;
					ii++;
				}
				else
					break;

				if (xMod > 30)
				{
					xMod -= 30;
					ii++;
				}
				else
					break;

				if (xMod > 31)
				{
					xMod -= 31;
					ii++;
				}
				else
					break;

				if (xMod > 31)
				{
					xMod -= 31;
					ii++;
				}
				else
					break;

				if (xMod > 30)
				{
					xMod -= 30;
					ii++;
				}
				else
					break;

				if (xMod > 31)
				{
					xMod -= 31;
					ii++;
				}
				else
					break;

				if (xMod > 30)
				{
					xMod -= 30;
					ii++;
				}
				else
					break;

				if (xMod > 31)
				{
					xMod -= 31;
					ii++;
				}

				break;
			}
    
			xYear += 1881;
    
			if ((ii > 2) && 
				((((xYear % 4) == 0) && ((xYear % 100) != 0)) || ((xYear % 400) == 0)))
			{
				xMod--;
			}
    
			if (xMod < 1)
			{
				ii--;
				if (ii < 1)
				{
					xYear--;
					ii = 12;
				}
        
				switch (ii)
				{
					case 1: xMod += 31;	break;
					case 2:
						if ((((xYear % 4) == 0) && ((xYear % 100) != 0)) || ((xYear % 400) == 0))
							xMod += 29;
						else
							xMod += 28;
				
						break;
					case 3: xMod += 31;		break;
					case 4: xMod += 30;		break;
					case 5: xMod += 31;		break;
					case 6: xMod += 30;		break;
					case 7: xMod += 31;		break;
					case 8: xMod += 31;		break;
					case 9: xMod += 30;		break;
					case 10: xMod += 31;	break;
					case 11: xMod += 30;	break;
					case 12: xMod += 31;	break;
				}
			}
    
			return (xYear.ToString() + ii.ToString("00") + xMod.ToString("00"));
		}

		/// <summary>
		/// 음력 데이터 (1881 ~ 2050)
		/// </summary>
		private void m_InitLunarArray()
		{
			m_LunarArray[1] = "1212122322121384";             //1881/1/30
			m_LunarArray[2] = "1212121221220355";
			m_LunarArray[3] = "1121121222120354";
			m_LunarArray[4] = "2112132122122384";
			m_LunarArray[5] = "2112112121220354";
			m_LunarArray[6] = "2121211212120354";
			m_LunarArray[7] = "2212321121212384";
			m_LunarArray[8] = "2122121121210354";
			m_LunarArray[9] = "2122121212120355";
			m_LunarArray[10] = "1232122121212384";

			m_LunarArray[11] = "1212121221220355";            //1891
			m_LunarArray[12] = "1121123221222384";
			m_LunarArray[13] = "1121121212220354";
			m_LunarArray[14] = "1212112121220354";
			m_LunarArray[15] = "2121231212121383";
			m_LunarArray[16] = "2221211212120355";
			m_LunarArray[17] = "1221212121210354";
			m_LunarArray[18] = "2123221212121384";
			m_LunarArray[19] = "2121212212120355";
			m_LunarArray[20] = "1211212232212384";

			m_LunarArray[21] = "1211212122210354";            //1901
			m_LunarArray[22] = "2121121212220355";
			m_LunarArray[23] = "1212132112212383";
			m_LunarArray[24] = "2212112112210354";
			m_LunarArray[25] = "2212211212120355";
			m_LunarArray[26] = "1221412121212384";
			m_LunarArray[27] = "1212122121210354";
			m_LunarArray[28] = "2112212122120355";
			m_LunarArray[29] = "1231212122212384";
			m_LunarArray[30] = "1211212122210354";

			m_LunarArray[31] = "2121123122122384";            //1911
			m_LunarArray[32] = "2121121122120354";
			m_LunarArray[33] = "2212112112120354";
			m_LunarArray[34] = "2212231212112384";
			m_LunarArray[35] = "2122121212120355";
			m_LunarArray[36] = "1212122121210354";
			m_LunarArray[37] = "2132122122121384";
			m_LunarArray[38] = "2112121222120355";
			m_LunarArray[39] = "1211212322122384";
			m_LunarArray[40] = "1211211221220354";

			m_LunarArray[41] = "2121121121220354";            //1921
			m_LunarArray[42] = "2122132112122384";
			m_LunarArray[43] = "1221212121120354";
			m_LunarArray[44] = "2121221212110354";
			m_LunarArray[45] = "2122321221212385";
			m_LunarArray[46] = "1121212212210354";
			m_LunarArray[47] = "2112121221220355";
			m_LunarArray[48] = "1231211221222384";
			m_LunarArray[49] = "1211211212220354";
			m_LunarArray[50] = "1221123121221383";

			m_LunarArray[51] = "2221121121210354";            //1931
			m_LunarArray[52] = "2221212112120355";
			m_LunarArray[53] = "1221241212112384";
			m_LunarArray[54] = "1212212212120355";
			m_LunarArray[55] = "1121212212210354";
			m_LunarArray[56] = "2114121212221384";
			m_LunarArray[57] = "2112112122210354";
			m_LunarArray[58] = "2211211412212384";
			m_LunarArray[59] = "2211211212120354";
			m_LunarArray[60] = "2212121121210354";

			m_LunarArray[61] = "2212214112121384";            //1941
			m_LunarArray[62] = "2122122121120355";
			m_LunarArray[63] = "1212122122120355";
			m_LunarArray[64] = "1121412122122384";
			m_LunarArray[65] = "1121121222120354";
			m_LunarArray[66] = "2112112122120354";
			m_LunarArray[67] = "2231211212122384";
			m_LunarArray[68] = "2121211212120354";
			m_LunarArray[69] = "2212121321212384";
			m_LunarArray[70] = "2122121121210354";

			m_LunarArray[71] = "2122121212120355";            //1951
			m_LunarArray[72] = "1212142121212384";
			m_LunarArray[73] = "1211221221220355";
			m_LunarArray[74] = "1121121221220354";
			m_LunarArray[75] = "2114112121222384";
			m_LunarArray[76] = "1212112121220354";
			m_LunarArray[77] = "2121211232122384";
			m_LunarArray[78] = "1221211212120354";
			m_LunarArray[79] = "1221212121210354";
			m_LunarArray[80] = "2121223212121384";

			m_LunarArray[81] = "2121212212120355";            //1961
			m_LunarArray[82] = "1211212212210354";
			m_LunarArray[83] = "2121321212221384";
			m_LunarArray[84] = "2121121212220355";
			m_LunarArray[85] = "1212112112210353";
			m_LunarArray[86] = "2223211211221384";
			m_LunarArray[87] = "2212211212120355";
			m_LunarArray[88] = "1221212321212384";
			m_LunarArray[89] = "1212122121210354";
			m_LunarArray[90] = "2112212122120355";

			m_LunarArray[91] = "1211232122212384";            //1971
			m_LunarArray[92] = "1211212122210354";
			m_LunarArray[93] = "2121121122210354";
			m_LunarArray[94] = "2212312112212384";
			m_LunarArray[95] = "2212112112120354";
			m_LunarArray[96] = "2212121232112384";
			m_LunarArray[97] = "2122121212110354";
			m_LunarArray[98] = "2212122121210355";
			m_LunarArray[99] = "2112124122121384";
			m_LunarArray[100] = "2112121221220355";

			m_LunarArray[101] = "1211211221220354";          //1981
			m_LunarArray[102] = "2121321122122384";
			m_LunarArray[103] = "2121121121220354";
			m_LunarArray[104] = "2122112112322384";
			m_LunarArray[105] = "1221212112120354";
			m_LunarArray[106] = "1221221212110354";
			m_LunarArray[107] = "2122123221212385";
			m_LunarArray[108] = "1121212212210354";
			m_LunarArray[109] = "2112121221220355";
			m_LunarArray[110] = "1211231212222384";

			m_LunarArray[111] = "1211211212220354";          //1991
			m_LunarArray[112] = "1221121121220354";
			m_LunarArray[113] = "1223212112121383";
			m_LunarArray[114] = "2221212112120355";
			m_LunarArray[115] = "1221221232112384";
			m_LunarArray[116] = "1212212122120355";
			m_LunarArray[117] = "1121212212210354";
			m_LunarArray[118] = "2112132212221384";
			m_LunarArray[119] = "2112112122210354";
			m_LunarArray[120] = "2211211212210354";

			m_LunarArray[121] = "2221321121212384";          //2001
			m_LunarArray[122] = "2212121121210354";
			m_LunarArray[123] = "2212212112120355";
			m_LunarArray[124] = "1232212122112384";
			m_LunarArray[125] = "1212122122120355";
			m_LunarArray[126] = "1121212322122384";
			m_LunarArray[127] = "1121121222120354";
			m_LunarArray[128] = "2112112122120354";
			m_LunarArray[129] = "2211231212122384";
			m_LunarArray[130] = "2121211212120354";

			m_LunarArray[131] = "2122121121210354";          //2011
			m_LunarArray[132] = "2124212112121384";
			m_LunarArray[133] = "2122121212120355";
			m_LunarArray[134] = "1212121223212384";
			m_LunarArray[135] = "1211212221220355";
			m_LunarArray[136] = "1121121221220354";
			m_LunarArray[137] = "2112132121222384";
			m_LunarArray[138] = "1212112121220354";
			m_LunarArray[139] = "2121211212120354";
			m_LunarArray[140] = "2122321121212384";

			m_LunarArray[141] = "1221212121210354";          //2021
			m_LunarArray[142] = "2121221212120355";
			m_LunarArray[143] = "1232121221212384";
			m_LunarArray[144] = "1211212212210354";
			m_LunarArray[145] = "2121123212221384";
			m_LunarArray[146] = "2121121212220355";
			m_LunarArray[147] = "1212112112220354";
			m_LunarArray[148] = "1221231211221383";
			m_LunarArray[149] = "2212211211220355";
			m_LunarArray[150] = "1212212121210354";

			m_LunarArray[151] = "2123212212121384";          //2031
			m_LunarArray[152] = "2112122122120355";
			m_LunarArray[153] = "1211212322212384";
			m_LunarArray[154] = "1211212122210354";
			m_LunarArray[155] = "2121121122120354";
			m_LunarArray[156] = "2212114112122384";
			m_LunarArray[157] = "2212112112120354";
			m_LunarArray[158] = "2212121211210354";
			m_LunarArray[159] = "2212232121211384";
			m_LunarArray[160] = "2122122121210355";

			m_LunarArray[161] = "2112122122120355";          //2041
			m_LunarArray[162] = "1231212122212384";
			m_LunarArray[163] = "1211211221220354";
			m_LunarArray[164] = "2121121321222384";
			m_LunarArray[165] = "2121121121220354";
			m_LunarArray[166] = "2122112112120354";
			m_LunarArray[167] = "2122141211212384";
			m_LunarArray[168] = "1221221212110354";
			m_LunarArray[169] = "2121221221210355";
			m_LunarArray[170] = "2114121221222385";          //2050

			m_LunarType[0] = 0;            /* 윤달 없음     */
			m_LunarType[1] = 29;           /* 윤달 없음     */
			m_LunarType[2] = 30;           /* 윤달 없음     */
			m_LunarType[3] = 29;           /* 윤달 TYPE 1   */
			m_LunarType[4] = 30;           /* 윤달 TYPE 2   */
		}
	}
}
