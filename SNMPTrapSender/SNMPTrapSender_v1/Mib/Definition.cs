﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace Lextm.SharpSnmpLib.Mib
{
    /// <summary>
    /// Definition class.
    /// </summary>
    internal sealed class Definition : IDefinition
    {
        private uint[] _id;
        private string _name;
        private string _module;
        private int _value;
        private DefinitionType _type;
        private IDictionary _children = new Hashtable();

        private Definition() 
        { 
            _type = DefinitionType.Unknown; 
        }
        
        /// <summary>
        /// Creates a <see cref="Definition"/> instance.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="entity"></param>
        internal Definition(Definition parent, IEntity entity)
        {
            uint[] id = (parent.Name == null || parent.Name == "") ?                
                null : parent.GetNumericalForm(); // null for root node
            _id = AppendTo(id, (uint)entity.Value);
            _name = entity.Name;
            _module = entity.Module;
            _value = entity.Value;
            parent.Add(this);
            if (entity.GetType() == typeof(OidValueAssignment)) 
            {
                _type = DefinitionType.OidValueAssignment;
                return;
            }
            
            if (entity.GetType() != typeof(ObjectType))
            {
                _type = DefinitionType.Unknown;
                return;
            }
            
            if (_name.EndsWith("Table"))
            {
                _type = DefinitionType.Table;
                return;
            }
            
            if (_name.EndsWith("Entry"))
            {
                _type = DefinitionType.Entry;
                return;
            }
            
            if (parent.Type == DefinitionType.Entry)
            {
                _type = DefinitionType.Column;
                return;
            }
            
            _type = DefinitionType.Scalar;
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
        
        /// <summary>
        /// Children definitions.
        /// </summary>
        public IEnumerable Children
        {
            get
            {
                return _children.Values;
            }
        }

        public DefinitionType Type
        {
            get
            {
                return _type;
            }
        }

        internal static Definition RootDefinition
        {
            get
            {
                return new Definition();
            }
        }
        
        /// <summary>
        /// Returns the textual form.
        /// </summary>
        public string TextualForm
        {
            get
            {
                return _module + "::" + _name;
            }
        }

        /// <summary>
        /// Indexer.
        /// </summary>
        public IDefinition this[int index]
        {
            get
            {
                foreach (IDefinition d in _children.Values)
                {
                    if (d.GetNumericalForm()[d.GetNumericalForm().Length - 1] == index)
                    {
                        return d;
                    }                    
                }
                
                throw new ArgumentOutOfRangeException("index");
            }
        }

        /// <summary>
        /// Module name.
        /// </summary>
        public string ModuleName
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
        /// Gets the numerical form.
        /// </summary>
        /// <returns></returns>
        public uint[] GetNumericalForm()
        {
            return (uint[])_id.Clone();
        }
        
        /// <summary>
        /// Add an <see cref="IEntity"/> node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDefinition Add(IEntity node)
        {
            if (_name == node.Parent) 
            {
                return ToDefinition(node, this);
            }
            
            foreach (Definition d in _children.Values)
            {
                IDefinition result = d.Add(node);
                if (result != null) 
                {
                    return result;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Adds a <see cref="Definition"/> child to this <see cref="Definition"/>.
        /// </summary>
        /// <param name="def"></param>
        private void Add(IDefinition def)
        {
            if (!_children.Contains(def.Value))
            {
                _children.Add(def.Value, def);
            }
        }

        internal static IDefinition ToDefinition(IEntity entity, Definition parent)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            
            return new Definition(parent, entity);
        }
        
        internal static uint[] AppendTo(uint[] parentId, uint value)
        {
			ArrayList n = parentId == null ? new ArrayList() : new ArrayList(parentId);
            n.Add(value);
            return (uint[])n.ToArray(typeof(uint));
        }
    }
}