/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/5/31
 * Time: 12:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Text;

namespace Lextm.SharpSnmpLib.Mib
{
    internal sealed class TrapType : IConstruct
    {
        private string _module;
        private int _value;
        private string _name;

        public TrapType(string module, IList header, Lexer lexer)
        {
            _module = module;
            _name = header[0].ToString();
            Symbol temp;
            while ((temp = lexer.NextSymbol) == Symbol.EOL)
            {
            }
            
			try
			{
				_value = int.Parse(temp.ToString());
			}
			catch
			{
				ConstructHelper.Validate(temp, true, "not a decimal");
			}
        }

        public string Module
        {
            get { return _module; }
        }

        public int Value
        {
            get { return _value; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}