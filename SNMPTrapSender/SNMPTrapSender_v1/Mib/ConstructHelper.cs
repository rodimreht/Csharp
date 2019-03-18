using System;
using System.Collections;
using System.Text;

namespace Lextm.SharpSnmpLib.Mib
{
    internal class ConstructHelper
    {
        internal static void ParseOidValue(Lexer lexer, out string parent, out int value)
        {
            parent = null;
            Symbol temp = IgnoreEOL(lexer);
            Expect(temp, Symbol.OpenBracket);
            string lastParent = parent;
            parent = lexer.NextSymbol.ToString();
            Symbol previous = null;
            value = 0;
            while ((temp = lexer.NextSymbol) != null) 
            {
                if (temp == Symbol.CloseBracket)
                {
                    parent = lastParent;
                    return;
                }

				try
				{
					value = int.Parse(temp.ToString());
					temp = lexer.NextSymbol;
					Expect(temp, Symbol.CloseBracket);
					return;
				}
				catch
				{}

                lastParent = parent;
                parent = temp.ToString();
                temp = lexer.NextSymbol;
                Expect(temp, Symbol.OpenParentheses);
                temp = lexer.NextSymbol;
				try
				{
					value = int.Parse(temp.ToString());
					temp = lexer.NextSymbol;
					Expect(temp, Symbol.CloseParentheses);
					previous = temp;
				}
				catch
				{
					Validate(temp, true, "not a decimal");
				}
            }
            
            throw SharpMibException.Create("end of file reached", previous);
        }
        
        internal static Symbol IgnoreEOL(Lexer lexer)
        {
            Symbol result;
            while ((result = lexer.NextSymbol) == Symbol.EOL)
            {
            }
            
            return result;
        }
        
        internal static void Expect(Symbol current, Symbol expected)
        {
            Validate(current, current != expected, expected + " expected");
        }
        
        internal static void Validate(Symbol current, bool condition, string message)
        {
            if (condition)
            {
                throw SharpMibException.Create(message, current);
            }
        }
        
        internal static void ValidateIdentifier(Symbol current)
        {
            string message;
            bool condition = !IsValidIdentifier(current.ToString(), out message);
            Validate(current, condition, message);
        }
        
        internal static bool IsValidIdentifier(string name, out string message)
        {
            if (name.Length < 1 || name.Length > 64) 
            {
                message = "an identifier must consist of 1 to 64 letters, digits, and hyphens";
                return false;
            }
            
            if (!char.IsLetter(name[0]))
            {
                message = "the initial character must be a letter";
                return false;
            }
            
            if (name.EndsWith("-"))
            {
                message = "a hyphen cannot be the last character of an identifier";
                return false;
            }
            
            if (name.IndexOf("--") >= 0) 
            {
                message = "a hyphen cannot be immediately followed by another hyphen in an identifier";
                return false;
            }
            
            if (name.IndexOf("_") >= 0)
            {
                message = "underscores are not allowed in identifiers";
                return false;
            }
            
            message = null;
            
            // TODO: SMIv2 forbids "-" except in module names and keywords
            return true;
        }
    }
}
