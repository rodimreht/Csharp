﻿using System;
using System.Collections;

namespace Lextm.SharpSnmpLib.Mib
{
    /// <summary>
    /// Object identifier node.
    /// </summary>
    internal sealed class ObjectIdentity : IEntity
    {
        private string _module;
        private string _name;
        private string _parent;
        private int _value;
        
        /// <summary>
        /// Creates a <see cref="ObjectIdentity"/>.
        /// </summary>
        /// <param name="module">Module name</param>
        /// <param name="header">Header</param>
        /// <param name="lexer">Lexer</param>
        public ObjectIdentity(string module, IList header, Lexer lexer)
        {
            _module = module;
            _name = header[0].ToString();
            ConstructHelper.ParseOidValue(lexer, out _parent, out _value);
            if (_parent == "0")
            {
                _parent = "ccitt";
            }
        }

        /// <summary>
        /// Module name.
        /// </summary>
        public string Module
        {
            get
            {
                return _module;
            }
        }
        
        /// <summary>
        /// Name.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }
        
        /// <summary>
        /// Parent name.
        /// </summary>
        public string Parent
        {
            get
            {
                return _parent;
            }
        }
        
        /// <summary>
        /// Value.
        /// </summary>
        public int Value
        {
            get
            {
                return _value;
            }
        }
    }
}