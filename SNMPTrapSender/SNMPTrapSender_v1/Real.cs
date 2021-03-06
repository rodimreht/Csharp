using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Globalization;

// ASN.1 BER encoding library by Malcolm Crowe at the University of the West of Scotland
// See http://cis.paisley.ac.uk/crow-ci0
// This is version 0 of the library, please advise me about any bugs
// mailto:malcolm.crowe@paisley.ac.uk

// Restrictions: It is assumed that no encoding has index length greater than 2^31-1.
// UNIVERSAL TYPES
// Some of the more unusual Universal encodings are supported but not fully implemented
// Should you require these types, as an alternative to changing this code
// you can catch the exception that is thrown and examine the contents yourself.
// APPLICATION TYPES
// If you want to handle Application types systematically, you can derive index class from
// Universal, and provide the Creator and Creators methods for your class
// You will see an example of how to do this in the Snmplib
// CONTEXT AND PRIVATE TYPES
// Ad hoc coding can be used for these, as an alterative to derive index class as above.
namespace Lextm.SharpSnmpLib
{
    /// <summary>
    /// Real type.
    /// </summary>
    public struct Real : ISnmpData
    {
        private byte[] _raw;
        private byte[] _bytes;
        
        /// <summary>
        /// Creates a <see cref="Real"/> from raw bytes.
        /// </summary>
        /// <param name="raw">Raw bytes</param>
        public Real(byte[] raw)
        {
            _raw = raw;
            _bytes = ByteTool.ToBytes(SnmpType.Real, _raw);
        }
        
        /// <summary>
        /// Creates a <see cref="Real"/> with a specific <see cref="Double"/>.
        /// </summary>
        /// <param name="value"></param>
        public Real(double value)
        {
            if (value == 0D)
            {
                _raw = new byte[0];
            }
            else
            {
                string s = value.ToString("E", CultureInfo.InvariantCulture); // hope this is acceptable..
                _raw = new byte[s.Length + 1];
                _raw[0] = 0x0;
                ASCIIEncoding.ASCII.GetBytes(s, 0, s.Length, _raw, 1);
            }
            
            _bytes = ByteTool.ToBytes(SnmpType.Real, _raw);
        }
        
        private double ToDouble()
        {
            if (_raw.Length == 0)
            {
                return 0.0;
            }
            
            // 8.5.5 binary encoding
            if ((_raw[0] & 0x80) != 0) 
            {
                byte c = _raw[0];
                int s = ((c & 0x40) != 0) ? -1 : 1;
                int t = c & 0x30;
                int b = (t == 0) ? 2 : (t == 1) ? 8 : 16;
                if (t == 3)
                {
                    throw (new SharpSnmpException("X690:8.5.5.2 reserved encoding"));
                }
                
                int f = (c & 0xc) >> 2;
                f = 1 << f;
                int p = 1;
                int h = (c & 0x3) + 1;
                if (h == 4)
                {
                    h = _raw[p++];
                }
                
                byte[] g = new byte[h];
                for (int j = 0; j < h; j++)
                {
                    g[j] = _raw[p++];
                }
                
                bool eb = CheckTwosComp(g);
                long e = new Integer64(g).ToInt64();
                if (eb)
                {
                    e = -e;
                }
                
                byte[] m = new byte[_raw.Length - p];
                for (int k = 0; p < _raw.Length; k++, p++)
                {
                    m[k] = _raw[p];
                }
                
                long n = new Integer64(m).ToInt64();
                return ((double)(s * n * f)) * Math.Pow((double)b, (double)e);
            }
            
            // 8.5.6 decimal encoding
            if ((_raw[0] & 0x40) == 0) 
            {
                return double.Parse(ASCIIEncoding.ASCII.GetString(_raw, 0, _raw.Length), CultureInfo.InvariantCulture);
            }
            
            // 8.5.7 special real encoding
            switch (_raw[0])
            {
                    case 0x40: return double.PositiveInfinity;
                    case 0x41: return double.NegativeInfinity;
                    default: throw (new SharpSnmpException("X690:8.5.7 reserved encoding"));
            }
        }

        private static bool CheckTwosComp(byte[] b)
        {
            if (b[0] < 128)
            {
                return false;
            }
            
            int c = 1;
            for (int j = b.Length - 1; j >= 0; j--)
            {
                if (b[j] == 0 && c > 0)
                {
                    continue;
                }
                
                b[j] = (byte)(255 - b[j] + c);
                c = 0;
            }
            
            return true;
        }
        
        /// <summary>
        /// Returns a <see cref="String"/> that represents this <see cref="Real"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToDouble().ToString(CultureInfo.CurrentCulture);
        }
        
        /// <summary>
        /// Type code.
        /// </summary>
        public SnmpType TypeCode 
        {
            get 
            {
                return SnmpType.Real;
            }
        }
        
        /// <summary>
        /// Converts to byte format.
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            return _bytes;
        }
        
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><value>true</value> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <value>false</value>.
        /// </returns>
        public bool Equals(Real other)
        {
            return Math.Abs(ToDouble() - other.ToDouble()) < double.Epsilon;
        }
        
        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Real"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Real"/>. </param>
        /// <returns><value>true</value> if the specified <see cref="Object"/> is equal to the current <see cref="Real"/>; otherwise, <value>false</value>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) 
            {
                return false;
            }
            
            if (object.ReferenceEquals(this, obj)) 
            {
                return true;
            }
            
            if (GetType() != obj.GetType()) 
            {
                return false;
            }
            
            return Equals((Real)obj);
        }
        
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Real"/>.</returns>
        public override int GetHashCode()
        {
            return ToDouble().GetHashCode();
        }
        
        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="left">Left <see cref="Real"/> object</param>
        /// <param name="right">Right <see cref="Real"/> object</param>
        /// <returns>
        /// Returns <c>true</c> if the values of its operands are equal, <c>false</c> otherwise.</returns>
        public static bool operator ==(Real left, Real right)
        {
            return left.Equals(right);
        }
        
        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="left">Left <see cref="Real"/> object</param>
        /// <param name="right">Right <see cref="Real"/> object</param>
        /// <returns>
        /// Returns <c>true</c> if the values of its operands are not equal, <c>false</c> otherwise.</returns>
        public static bool operator !=(Real left, Real right)
        {
            return !(left == right);
        }
    }
    
    // all references here are to ITU-X.690-12/97
}
