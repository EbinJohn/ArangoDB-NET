﻿using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoDatabase
    {
        #region Properties

        private ArangoNode _node;

        #endregion

        public ArangoDatabase(string alias)
        {
            _node = ArangoClient.GetNode(alias);
        }

        public ArangoDocument GetDocument(string id)
        {
            return GetDocument(id, "");
        }

        public ArangoDocument GetDocument(string id, string revision)
        {
            Document document = new Document(_node);

            return document.Get(id, revision);
        }
    }
}
