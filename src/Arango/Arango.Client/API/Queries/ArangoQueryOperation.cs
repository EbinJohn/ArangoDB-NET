﻿using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoQueryOperation
    {
        private CursorOperation _cursorOperation;
        
        private int _batchSize = 0;
        private Dictionary<string, object> _bindVars = new Dictionary<string, object>();
        private string _aql = "";
        
        // list of variables created with let keyword
        private List<string> _variables = new List<string>();
        // name of the last performed operation in query chain
        private string _lastOperation = AQL.None;
        // depth of nested operations
        private int _level = 0;
        
        internal ArangoQueryOperation(CursorOperation cursorOperation)
        {
            _cursorOperation = cursorOperation;
        }
        
        public ArangoQueryOperation()
        {
        }
        
        public ArangoQueryOperation Aql(string aql)
        {
            _aql += aql;
            
            return this;
        }
        
        public ArangoQueryOperation BatchSize(int batchSize)
        {
            _batchSize = batchSize;
            
            return this;
        }
        
        public ArangoQueryOperation AddParameter(string key, object value)
        {
            _bindVars.Add(key, value);
            
            return this;
        }
        
        #region AQL standard operations
        
        public ArangoQueryOperation Filter(string variable)
        {
            Join(AQL.Filter, variable);
            
            _lastOperation = AQL.Filter;
            
            return this;
        }
        
        public ArangoQueryOperation For(string variable, string expression)
        {
            // TODO: refactor query generation
            // - use something like dictionary<string, ?> to produce execution tree
            // - FOR and RETURN operations are crucial for the execution
            
            /*if (_level > 0)
            {
                _aql += "(";
            }*/
            
            switch (_lastOperation)
            {
                case AQL.None:
                    // FOR operation starts without initial space
                    _aql = AQL.For;
                
                    Join(variable, AQL.In, expression);
                    break;
                case AQL.Let:
                    // FOR within LET operation starts with brackets
                    _aql += " (" + AQL.For;
                    
                    Join(variable, AQL.In, expression);
                    break;
                case AQL.Return:
                    // 
                    _aql += ")";
                    
                    Join(AQL.For, variable, AQL.In, expression);
                    break;
                default:
                    Join(AQL.For, variable, AQL.In, expression);
                    break;
            }
            
            _lastOperation = AQL.For;
            _level++;
            
            return this;
        }
        
        public ArangoQueryOperation Let(string variable)
        {
            if (_variables.Contains(variable))
            {
                throw new ArangoException("Variable name is already used within the AQL query.");
            }
            
            _variables.Add(variable);
            
            if (_lastOperation == AQL.None)
            {
                _aql = AQL.Let;
                
                Join(variable, AQL.Equals);
            }
            else
            {
                Join(AQL.Let, variable, AQL.Equals);
            }
            
            _lastOperation = AQL.Let;
            
            return this;
        }
        
        public ArangoQueryOperation Return(string variable)
        {
            Join(AQL.Return, variable);
            
            _lastOperation = AQL.Return;
            _level--;
            
            return this;
        }
        
        #endregion
        
        #region AQL expression operators
        
        public ArangoQueryOperation Equals<T>(T conditionValue)
        {
            if ((conditionValue is string) && _variables.Contains(conditionValue.ToString()))
            {
                Join(AQL.DoubleEquals, conditionValue.ToString());
            }
            else
            {
                Join(AQL.DoubleEquals, ToString(conditionValue));
            }
            
            return this;
        }
        
        public ArangoQueryOperation And(string variable)
        {
            Join(AQL.And, variable);
            
            return this;
        }
        
        #endregion
        
        #region AQL support methods
        
        public ArangoQueryOperation From(string expression)
        {
            Join(AQL.From, expression);
            
            return this;
        }
        
        #endregion
        
        #region ToList
        
        public List<Document> ToList(out int count)
        {
            return _cursorOperation.Post(_aql.ToString(), true, out count, _batchSize, _bindVars);
        }
        
        public List<Document> ToList()
        {
            int count = 0;
            
            return _cursorOperation.Post(_aql.ToString(), false, out count, _batchSize, _bindVars);
        }
        
        public List<T> ToList<T>(out int count) where T : class, new()
        {
            List<Document> documents = _cursorOperation.Post(_aql.ToString(), true, out count, _batchSize, _bindVars);
            List<T> genericCollection = new List<T>();
            
            foreach (Document document in documents)
            {
                T genericObject = document.To<T>();
                document.MapAttributesTo(genericObject);
                
                genericCollection.Add(genericObject);
            }
            
            return genericCollection;
        }
        
        public List<T> ToList<T>() where T : class, new()
        {
            int count = 0;
            List<Document> documents = _cursorOperation.Post(_aql.ToString(), false, out count, _batchSize, _bindVars);
            List<T> genericCollection = new List<T>();
            
            foreach (Document document in documents)
            {
                T genericObject = document.To<T>();
                document.MapAttributesTo(genericObject);
                
                genericCollection.Add(genericObject);
            }
            
            return genericCollection;
        }
        
        #endregion
        
        #region ToString
        
        public override string ToString()
        {
            for (int i = 0; i < _level; i++)
            {
                _aql += ")";
                _level--;
            }
            
            return _aql;
        }
        
        private string ToString(object value)
        {
            if (value is string)
            {
                return "'" + value + "'";
            }
            else
            {
                return value.ToString();
            }
        }
        
        #endregion
        
        private void Join(params string[] parts)
        {
            _aql += " " + string.Join(" ", parts);
        }
    }
}
