/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/5/31
 * Time: 12:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace Lextm.SharpSnmpLib.Mib
{
    internal sealed class ImportsFrom
    {
        private string _module;
        private IList _types = new ArrayList();
        
        public ImportsFrom(Symbol last, Lexer lexer)
        {
            Symbol previous = last;
            Symbol temp;
            while ((temp = lexer.NextSymbol) != Symbol.From)
            {
                if (temp == Symbol.EOL) 
                {
                    continue;
                }
                
                if (temp == Symbol.Comma)
                {
                    ConstructHelper.ValidateIdentifier(previous);
                    _types.Add(previous.ToString());
                }
                
                previous = temp;
            }
            
            _module = lexer.NextSymbol.ToString();
        }
        
        public string Module
        {
            get { return _module; }
        }
    }
}