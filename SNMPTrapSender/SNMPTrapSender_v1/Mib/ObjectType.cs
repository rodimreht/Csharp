﻿using System;
using System.Collections;
using System.Text;

namespace Lextm.SharpSnmpLib.Mib
{
    internal sealed class ObjectType : IEntity
    {
        private string _module;
        private string _parent;
        private int _value;
        private string _name;

        public ObjectType(string module, IList header, Lexer lexer)
        {
            _module = module;
            _name = header[0].ToString();
            ConstructHelper.ParseOidValue(lexer, out _parent, out _value);
        }

        public string Module
        {
            get { return _module; }
        }

        public string Parent
        {
            get { return _parent; }
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