﻿using System;
using System.Collections;
using System.Text;

namespace Lextm.SharpSnmpLib.Mib
{
    internal sealed class Macro : ITypeAssignment
    {
        public Macro(string module, IList header, Lexer lexer)
        {
            Symbol temp;
            while ((temp = lexer.NextSymbol) != Symbol.Begin)
            {                
            }
            
            while ((temp = lexer.NextSymbol) != Symbol.End)
            {
            }
        }
    }
}
