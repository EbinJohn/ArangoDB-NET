﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arango.Client;

namespace Arango.Test
{
    [TestClass]
    public class DocumentTests : IDisposable
    {
        private ArangoDatabase _database;
        private ArangoCollection _collection;

        public DocumentTests()
        {
            string alias = "test";
            string testCollectionName = "tempUnitTestCollectionForDocumentTests001xyz";
            string[] connectionString = File.ReadAllText(@"..\..\..\..\..\ConnectionString.txt").Split(';');

            // create aliased arango node
            ArangoNode node = new ArangoNode(
                connectionString[0],
                int.Parse(connectionString[1]),
                connectionString[2],
                connectionString[3],
                alias
            );
            ArangoClient.Nodes.Add(node);

            _database = new ArangoDatabase(alias);

            // delete test collection if it already exist
            /*ArangoCollection col = _database.GetCollection(testCollectionName);

            if ((col != null) && !string.IsNullOrEmpty(col.Name))
            {
                _database.DeleteCollection(testCollectionName);
            }*/

            // create test collection
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = testCollectionName;
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            _collection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);
        }

        #region Json de/serialization

        [TestMethod]
        public void JsonDeserialization()
        {
            string jsonString = "{\"stringKey\":\"string value\",\"numericKey\":12321,\"objectKey\":{\"stringKey\":\"string value 2\",\"numericKey\":123,\"arrayKey\":[{\"foo\":123},{\"foo\":456}],\"innerObjectKey\":{\"bar\":\"wh\\\"o\\\"a\",\"baz\":123}}}";

            Json json = new Json(jsonString);

            Assert.AreEqual(json.Get("stringKey"), "string value");
            Assert.AreEqual(json.Get<int>("numericKey"), 12321);
            Assert.AreEqual(json.Get("stringKey"), "string value");
            Assert.AreEqual(json.Get("objectKey.stringKey"), "string value 2");
            Assert.AreEqual(json.Get<int>("objectKey.numericKey"), 123);

            List<Json> arrayKeys = json.Get<List<Json>>("objectKey.arrayKey");
            Assert.AreEqual(arrayKeys[0].Get<int>("foo"), 123);
            Assert.AreEqual(arrayKeys[1].Get<int>("foo"), 456);

            Assert.AreEqual(json.Get("objectKey.innerObjectKey.bar"), "wh\"o\"a");
            Assert.AreEqual(json.Get<int>("objectKey.innerObjectKey.baz"), 123);

            Assert.AreEqual(json.Stringify(), jsonString);
        }

        [TestMethod]
        public void JsonSerialization()
        {
            string jsonString = "{\"stringKey\":\"string value\",\"numericKey\":12321,\"arrayKey\":[{\"foo\":123},{\"foo\":456}]}";

            Json json = new Json();
            json.Set("stringKey", "string value");
            json.Set("numericKey", 12321);

            List<Json> arrayKey = new List<Json>();
            Json temp1 = new Json();
            temp1.Set<int>("foo", 123);
            arrayKey.Add(temp1);
            Json temp2 = new Json();
            temp2.Set<int>("foo", 456);
            arrayKey.Add(temp2);

            json.Set<List<Json>>("arrayKey", arrayKey);

            string serialized = json.Stringify();

            Assert.AreEqual(serialized, jsonString);
        }

        #endregion

        #region Create, get

        [TestMethod]
        public void CreateDocumentUsingCollectionID_AND_GetDocumentByID()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.ID, document.ID);
            Assert.AreEqual(loadedDocument.Revision, document.Revision);
            Assert.AreEqual(loadedDocument.JsonObject.Get("foo"), jsonObject.Get("foo"));
            Assert.AreEqual(loadedDocument.JsonObject.Get<int>("bar"), jsonObject.Get<int>("bar"));
        }

        [TestMethod]
        public void CreateDocumentUsingCollectionID_AND_GetDocumentByIDAndOutdatedRevision()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument loadedDocument = _database.GetDocument(document.ID, "whoa");
            Assert.AreEqual(loadedDocument.ID, document.ID);
            Assert.AreEqual(loadedDocument.Revision, document.Revision);
            Assert.AreEqual(loadedDocument.JsonObject.Get("foo"), jsonObject.Get("foo"));
            Assert.AreEqual(loadedDocument.JsonObject.Get<int>("bar"), jsonObject.Get<int>("bar"));
        }

        [TestMethod]
        public void CreateDocumentUsingCollectionID_AND_GetDocumentByIDAndActualRevision()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument loadedDocument = _database.GetDocument(document.ID, document.Revision);
            Assert.IsTrue(string.IsNullOrEmpty(loadedDocument.ID));
            Assert.AreEqual(loadedDocument.Revision, document.Revision);
        }

        [TestMethod]
        public void CreateDocumentUsingCollectionNameWithoutCreateProcess_AND_GetDocumentByID()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document = _database.CreateDocument(_collection.Name, false, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.ID, document.ID);
            Assert.AreEqual(loadedDocument.Revision, document.Revision);
            Assert.AreEqual(loadedDocument.JsonObject.Get("foo"), jsonObject.Get("foo"));
            Assert.AreEqual(loadedDocument.JsonObject.Get<int>("bar"), jsonObject.Get<int>("bar"));
        }

        [TestMethod]
        public void CreateDocumentUsingCollectionNameWithCreateProcess_AND_GetDocumentByID()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            string tempCollection = "tempUnitTestCollectionForDocumentTests002xyz";

            ArangoDocument document = _database.CreateDocument(tempCollection, true, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoCollection collection = _database.GetCollection(tempCollection);
            Assert.AreEqual(collection.Name, tempCollection);

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.ID, document.ID);
            Assert.AreEqual(loadedDocument.Revision, document.Revision);
            Assert.AreEqual(loadedDocument.JsonObject.Get("foo"), jsonObject.Get("foo"));
            Assert.AreEqual(loadedDocument.JsonObject.Get<int>("bar"), jsonObject.Get<int>("bar"));

            _database.DeleteCollection(tempCollection);
        }

        #endregion

        #region Create, get all

        [TestMethod]
        public void CreateDocument_AND_GetAllDocumentsByCollectionID()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document1 = _database.CreateDocument(_collection.ID, jsonObject, false);
            ArangoDocument document2 = _database.CreateDocument(_collection.ID, jsonObject, false);

            List<ArangoDocument> documents = _database.GetAllDocuments(_collection.ID);
            Assert.IsTrue(documents.Count == 2);
            Assert.AreEqual(documents.Where(doc => doc.ID == document1.ID).First().ID, document1.ID);
            Assert.AreEqual(documents.Where(doc => doc.ID == document2.ID).First().ID, document2.ID);
        }

        [TestMethod]
        public void CreateDocument_AND_GetAllDocumentsByCollectionName()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document1 = _database.CreateDocument(_collection.ID, jsonObject, false);
            ArangoDocument document2 = _database.CreateDocument(_collection.ID, jsonObject, false);

            List<ArangoDocument> documents = _database.GetAllDocuments(_collection.Name);
            Assert.IsTrue(documents.Count == 2);
            Assert.AreEqual(documents.Where(doc => doc.ID == document1.ID).First().ID, document1.ID);
            Assert.AreEqual(documents.Where(doc => doc.ID == document2.ID).First().ID, document2.ID);
        }

        #endregion

        #region Create, replace

        [TestMethod]
        public void CreateDocument_AND_ReplaceWithDefaultPolicy()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);
            jsonObject.Set("baz", "new field");

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            Json newJsonObject = new Json();
            newJsonObject.Set("foo", "new johny");
            newJsonObject.Set("bar", 67890);

            string revision = _database.ReplaceDocument(document.ID, document.Revision, DocumentUpdatePolicy.Default, newJsonObject, false);
            Assert.AreNotEqual(revision, document.Revision);

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.Revision, revision);
            Assert.AreEqual(loadedDocument.JsonObject.Get("foo"), newJsonObject.Get("foo"));
            Assert.AreEqual(loadedDocument.JsonObject.Get<int>("bar"), newJsonObject.Get<int>("bar"));
        }

        #endregion

        #region Create, update

        [TestMethod]
        public void CreateDocument_AND_UpdateWithKeepNullValues()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            Json update = new Json();
            update.Set("baz", "new field");

            string revision = _database.UpdateDocument(document.ID, document.Revision, DocumentUpdatePolicy.Default, update, true, false);
            Assert.AreNotEqual(revision, document.Revision);

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.Revision, revision);
            Assert.AreEqual(loadedDocument.JsonObject.Get("foo"), jsonObject.Get("foo"));
            Assert.AreEqual(loadedDocument.JsonObject.Get<int>("bar"), jsonObject.Get<int>("bar"));
            Assert.AreEqual(loadedDocument.JsonObject.Get("baz"), update.Get("baz"));
        }

        [TestMethod]
        public void CreateDocument_AND_UpdateWithoutKeepNullValues()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            Json update = new Json();
            update.Set("baz", "new field");

            string revision = _database.UpdateDocument(document.ID, document.Revision, DocumentUpdatePolicy.Default, update, false, false);
            Assert.AreNotEqual(revision, document.Revision);

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.Revision, revision);
            Assert.AreEqual(loadedDocument.JsonObject.Get("foo"), jsonObject.Get("foo"));
            Assert.AreEqual(loadedDocument.Has("Bar"), false);
            Assert.AreEqual(loadedDocument.JsonObject.Get("baz"), update.Get("baz"));
        }

        #endregion

        #region Create, delete

        [TestMethod]
        public void CreateDocument_AND_Delete()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            string deleteDocumentID = _database.DeleteDocument(document.ID, document.Revision, DocumentUpdatePolicy.Default, false);
            Assert.IsTrue(!string.IsNullOrEmpty(deleteDocumentID));
        }

        #endregion

        #region Create, check

        [TestMethod]
        public void CreateDocument_AND_CheckExisting()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument checkedDocument = _database.CheckDocument(document.ID);
            Assert.AreEqual(checkedDocument.ID, document.ID);
            Assert.AreEqual(checkedDocument.Revision, document.Revision);
        }

        [TestMethod]
        public void CreateDocument_AND_CheckNotExisting()
        {
            Json jsonObject = new Json();
            jsonObject.Set("foo", "bravo");
            jsonObject.Set("bar", 12345);

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument checkedDocument = _database.CheckDocument("whoa" + document.ID);
            Assert.IsTrue(string.IsNullOrEmpty(checkedDocument.ID));
            Assert.IsTrue(string.IsNullOrEmpty(checkedDocument.Revision));
        }

        #endregion

        public void Dispose()
        {
            _database.DeleteCollection(_collection.ID);
        }
    }
}
