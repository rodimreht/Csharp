using System;
using System.Collections.Generic;
using System.IO;

namespace Lextm.SharpSnmpLib.Mib
{
    /// <summary>
    /// Object tree class.
    /// </summary>
    internal sealed class ObjectTree : IObjectTree
    {
        private IDictionary<string, MibModule> _parsed = new SortedDictionary<string, MibModule>();
        private IList<MibModule> _pending = new List<MibModule>();
        private IDictionary<string, IDefinition> nameTable;
        private Definition root;
        private Lexer _lexer;
        
        /// <summary>
        /// Creates an <see cref="ObjectTree"/> instance.
        /// </summary>
        public ObjectTree()
        {
            _lexer = new Lexer();
            root = Definition.RootDefinition;
			IDefinition definition = Definition.ToDefinition(new OidValueAssignment("SNMPv2-SMI", "ccitt", null, 0), this.root);
			IDefinition definition2 = Definition.ToDefinition(new OidValueAssignment("SNMPv2-SMI", "iso", null, 1), this.root);
			IDefinition definition3 = Definition.ToDefinition(new OidValueAssignment("SNMPv2-SMI", "joint-iso-ccitt", null, 2), this.root);
			Dictionary<string, IDefinition> dictionary = new Dictionary<string, IDefinition>();
			dictionary.Add(definition2.TextualForm, definition2);
			dictionary.Add(definition.TextualForm, definition);
			dictionary.Add(definition3.TextualForm, definition3);
			this.nameTable = dictionary;
        }
        
        /// <summary>
        /// Root definition.
        /// </summary>
        public IDefinition Root
        {
            get
            {
                return root;
            }
        }

        internal IDefinition Find(string module, string name)
        {
            string full = module + "::" + name;
            if (nameTable.ContainsKey(full))
            {
                return nameTable[full];
            }
            
            return null;
        }

        internal IDefinition Find(uint[] numerical)
        {
            if (numerical == null)
            {
                throw new ArgumentNullException("numerical");
            }
            
            if (numerical.Length == 0)
            {
                throw new ArgumentException("numerical cannot be empty");
            }
            
            int i = 0;
            IDefinition result;
            IDefinition temp = root;
            do
            {
                result = temp[(int)numerical[i]];
                temp = result;
                i++;
            }
            while (i < numerical.Length);
            return result;
        }
        
        private bool ParseModule(MibModule module)
        {
            if (!MibModule.AllDependentsAvailable(module, _parsed))
            {
                return false;
            }
            
            if (_parsed.ContainsKey(module.Name)) 
            {
                return true;
            }
            
            _parsed.Add(module.Name, module);
            foreach (IEntity node in module.Entities)
            {
                IDefinition result = root.Add(node);
                if (result != null && !nameTable.ContainsKey(result.TextualForm))
                {
                    nameTable.Add(result.TextualForm, result);
                }
            }
            
            return true;
        }
        
        private int ParsePendings()
        {
            int previous;
            int current = _pending.Count;
            while (current != 0)
            {
                previous = current;
                for (int i = 0; i < _pending.Count; i++)
                {
                    bool succeeded = ParseModule(_pending[i]);
                    if (succeeded)
                    {
                        _pending.RemoveAt(i);
                    }
                }
                
                current = _pending.Count;
                if (current == previous) 
                {
                    // cannot parse more
                    break;
                }
            }
            
            return current;
        }

        internal int Parse(string file, TextReader stream)
        {
            _lexer.Parse(file, stream);
            MibDocument doc = new MibDocument(_lexer);
            IList<MibModule> modules = doc.Modules;
            foreach (MibModule module in modules)
            {
                _pending.Add(module);
            }
            
            ParsePendings();
            return _lexer.SymbolCount;
        }
        /// <summary>
        /// Loaded MIB documents.
        /// </summary>
        public IEnumerable<string> LoadedModules
        {
            get
            {
                return _parsed.Keys;
            }
        }
    }
}