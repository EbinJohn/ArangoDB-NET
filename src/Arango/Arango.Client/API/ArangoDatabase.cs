﻿using System.Collections.Generic;
using System.Net;
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

        #region Collection

        #region Create

        public ArangoCollection CreateCollection(string name, ArangoCollectionType type, bool waitForSync, long journalSize)
        {
            var collection = new Collection(_node);

            return collection.Post(name, type, waitForSync, journalSize);
        }

        #endregion

        #region Delete

        public long DeleteCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.Delete(id);
        }

        public long DeleteCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.Delete(name);
        }

        #endregion

        #region Get

        public ArangoCollection GetCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.Get(id);
        }

        public ArangoCollection GetCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.Get(name);
        }

        public ArangoCollection GetCollectionProperties(long id)
        {
            var collection = new Collection(_node);

            return collection.GetProperties(id);
        }

        public ArangoCollection GetCollectionProperties(string name)
        {
            var collection = new Collection(_node);

            return collection.GetProperties(name);
        }

        public ArangoCollection GetCollectionCount(long id)
        {
            var collection = new Collection(_node);

            return collection.GetCount(id);
        }

        public ArangoCollection GetCollectionCount(string name)
        {
            var collection = new Collection(_node);

            return collection.GetCount(name);
        }

        public ArangoCollection GetCollectionFigures(long id)
        {
            var collection = new Collection(_node);

            return collection.GetFigures(id);
        }

        public ArangoCollection GetCollectionFigures(string name)
        {
            var collection = new Collection(_node);

            return collection.GetFigures(name);
        }

        public List<ArangoCollection> GetCollections()
        {
            var collection = new Collection(_node);

            return collection.GetAll();
        }

        #endregion

        #region Truncate

        public bool TruncateCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.PutTruncate(id);
        }

        public bool TruncateCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.PutTruncate(name);
        }

        #endregion

        #region Load

        public ArangoCollection LoadCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.PutLoad(id);
        }

        public ArangoCollection LoadCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.PutLoad(name);
        }

        #endregion

        #region Unload

        public ArangoCollection UnloadCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.PutUnload(id);
        }

        public ArangoCollection UnloadCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.PutUnload(name);
        }

        #endregion

        #region Update properties

        public ArangoCollection UpdateCollectionProperties(long id, bool waitForSync)
        {
            var collection = new Collection(_node);

            return collection.PutProperties(id, waitForSync);
        }

        public ArangoCollection UpdateCollectionProperties(string name, bool waitForSync)
        {
            var collection = new Collection(_node);

            return collection.PutProperties(name, waitForSync);
        }

        #endregion

        #endregion

        #region Document

        public ArangoDocument GetDocument(string handle)
        {
            return GetDocument(handle, "");
        }

        public ArangoDocument GetDocument(string handle, string revision)
        {
            var document = new Document(_node);

            return document.Get(handle, revision);
        }

        #endregion
    }
}
