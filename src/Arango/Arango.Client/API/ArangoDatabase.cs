using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoDatabase
    {
        private Connection _connection;

        public ArangoDocumentOperation Document
        {
            get
            {
                return new ArangoDocumentOperation(new DocumentOperation(_connection));
            }
        }
        
        public ArangoCollectionOperation Collection
        {
            get
            {
                return new ArangoCollectionOperation(new CollectionOperation(_connection));
            }
        }

        public ArangoDatabase(string alias)
        {
            _connection = ArangoClient.GetConnection(alias);
        }
    }
}

