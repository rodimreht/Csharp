/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/5/17
 * Time: 16:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lextm.SharpSnmpLib.Mib
{
    /// <summary>
    /// Lexer class that parses MIB files into symbol list.
    /// </summary>
    internal sealed class Lexer
    {
        private IList<Symbol> _symbols = new List<Symbol>();
        
        internal void Parse(TextReader stream)
        {
            Parse(null, stream);
        }
        
        /// <summary>
        /// Parses MIB file to symbol list.
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="stream">File stream</param>
        public void Parse(string file, TextReader stream)
        {
            string line;
            int i = 0;
            while ((line = stream.ReadLine()) != null)
            {
                if (!stringSection && line.TrimStart().StartsWith("--", StringComparison.Ordinal))
                {
                    i++;
                    continue; // commented line
                }
                
                ParseLine(file, line, i);
                i++;
            }
        }
        
        private void ParseLine(string file, string line, int row)
        {
            line = line + '\n';
            int count = line.Length;
            int i = 0;
            for (i = 0; i < count; i++)
            {
                char current = line[i];
                bool moveNext = Parse(file, _symbols, current, row, i);
                if (moveNext) 
                {
                    break;    
                }
            }
        }
        
        private int index;
        
        /// <summary>
        /// Next <see cref="Symbol"/>.
        /// </summary>
        public Symbol NextSymbol
        {
            get
            {
                if (index < _symbols.Count) 
                {
                    return _symbols[index++];
                }
                
                return null;
            }
        }
        
        internal int SymbolCount
        {
            get
            {
                return _symbols.Count;
            }
        }
        
        private static StringBuilder temp = new StringBuilder();
        private static bool stringSection;
        
        /// <summary>
        /// Parses a list of <see cref="Char"/> to <see cref="Symbol"/>.
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="current">Current <see cref="Char"/></param>
        /// <param name="row">Row number</param>
        /// <param name="column">Column number</param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool Parse(string file, IList<Symbol> list, char current, int row, int column)
        {
            switch (current)
            {
                case '\n':    
                case '{': 
                case '}':    
                case '(': 
                case ')':
                case '[':
                case ']':
                case ';': 
                case ',':
                    if (!stringSection) 
                    {
                        bool moveNext = ParseLastSymbol(file, list, ref temp, row, column);
                        if (moveNext) 
                        {
                            list.Add(CreateSpecialSymbol(file, '\n', row, column));
                            return true;
                        }
                        
                        list.Add(CreateSpecialSymbol(file, current, row, column));
                        return false;
                    }
                    
                    break;
                case '"':
                    stringSection = !stringSection;
                    break;
                case '\r':
                    return false;
                default:
                    if (char.IsWhiteSpace(current) && !stringSection)
                    {
                        bool moveNext = ParseLastSymbol(file, list, ref temp, row, column);
                        if (moveNext)
                        {
                            list.Add(CreateSpecialSymbol(file, '\n', row, column));
                            return true;
                        }
                        
                        return false;
                    }
                    
                    break;
            }
            
            temp.Append(current);
            return false;
        }
        
        private static bool ParseLastSymbol(string file, IList<Symbol> list, ref StringBuilder builder, int row, int column)
        {
            if (builder.Length > 0)
            {
                string content = builder.ToString();
                builder.Length = 0;
                Symbol s = new Symbol(file, content, row, column);
                if (s == Symbol.Comment)
                {
                    return true;
                }
                
                list.Add(s);
            }
            
            return false;
        }
        
        private static Symbol CreateSpecialSymbol(string file, char value, int row, int column)
        {
            string str;
            switch (value) 
            {
                case '\n':
                    str = Environment.NewLine;
                    break;
                case '{':
                    str = "{";
                    break;
                case '}':
                    str = "}";
                    break;
                case '(':
                    str = "(";
                    break;
                case ')':
                    str = ")";
                    break;
                case '[':
                    str = "[";
                    break;
                case ']':
                    str = "]";
                    break;
                case ';':
                    str = ";";
                    break;
                case ',':
                    str = ",";
                    break;
                default:
                    throw new ArgumentException("value is not a special character");
            }
            
            return new Symbol(file, str, row, column);
        }
    }
}