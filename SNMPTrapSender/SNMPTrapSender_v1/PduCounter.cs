﻿using System;
using System.Collections;
using System.Text;
namespace Lextm.SharpSnmpLib
{
    /// <summary>
    /// A counter that generates PDU sequence number.
    /// </summary>
    /// <remarks>The sequence number is used to identifier PDU sessions.</remarks>
    internal class PduCounter
    {
        internal static Integer32 NextCount
        {
            get
            {
                count += 10;
                return new Integer32(count);
            }
        }
        
        internal static void Clear()
        {
            count = 0;
        }

        private static int count;
    }
}
