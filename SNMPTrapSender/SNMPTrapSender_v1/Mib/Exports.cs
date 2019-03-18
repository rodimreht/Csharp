/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/6/7
 * Time: 17:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Collections.Specialized;

namespace Lextm.SharpSnmpLib.Mib
{
    /// <summary>
    /// Description of Exports.
    /// </summary>
    internal sealed class Exports
    {
        private IList _types = new ArrayList();
        
        public Exports(Lexer lexer)
        {
            Symbol previous = null;
            Symbol temp;
            while ((temp = lexer.NextSymbol) != Symbol.Semicolon) 
            {
                if (temp == Symbol.EOL) 
                {
                    continue;
                }
                
                if (temp == Symbol.Comma && previous != null) 
                {
                    ConstructHelper.ValidateIdentifier(previous);
                    _types.Add(previous.ToString());
                }
                
                previous = temp;
            }
        }
    }
}