using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoDatabase
    {
        private Connection _connection;

        public ArangoCollectionOperation Collection
        {
            get
            {
                return new ArangoCollectionOperation(new CollectionOperation(_connection));
            }
        }
        
        public ArangoDocumentOperation Document
        {
            get
            {
                return new ArangoDocumentOperation(new DocumentOperation(_connection));
            }
        }
        
        public ArangoEdgeOperation Edge
        {
            get
            {
                return new ArangoEdgeOperation(new EdgeOperation(_connection));
            }
        }
        
        public ArangoQueryOperation Query
        {
            get
            {
                return new ArangoQueryOperation(new CursorOperation(_connection));
            }
        }

        public ArangoDatabase(string alias)
        {
            _connection = ArangoClient.GetConnection(alias);
        }
        
        /*#region Query
        
        public List<Document> Query(string aql, out int count, Dictionary<string, object> bindVars = null, int batchSize = 0)
        {
            CursorOperation cursorOperation = new CursorOperation(_connection);
            
            return cursorOperation.Post(aql, true, out count, batchSize, bindVars);
        }
        
        public List<Document> Query(string aql, Dictionary<string, object> bindVars = null, int batchSize = 0)
        {
            CursorOperation cursorOperation = new CursorOperation(_connection);
            int count = 0;
            
            return cursorOperation.Post(aql, false, out count, batchSize, bindVars);
        }
        
        public List<T> Query<T>(string aql, Dictionary<string, object> bindVars = null, int batchSize = 0) where T : class, new()
        {
            List<Document> documents = Query(aql, bindVars, batchSize);
            List<T> genericCollection = new List<T>();
            
            foreach (Document document in documents)
            {
                T genericObject = document.To<T>();
                document.MapAttributesTo(genericObject);
                
                genericCollection.Add(genericObject);
            }
            
            return genericCollection;
        }
        
        #endregion*/
    }
}

