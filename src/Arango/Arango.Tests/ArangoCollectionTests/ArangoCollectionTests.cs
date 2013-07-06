﻿using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.ArangoCollectionTests
{
    [TestFixture()]
    public class ArangoCollectionTests
    {
        [Test()]
        public void Should_create_and_delete_collection()
        {
            Database.DeleteTestCollection(Database.TestEdgeCollectionName);
            
            ArangoDatabase db = Database.GetTestDatabase();
            
            // set collection data
            ArangoCollection collection = new ArangoCollection();
            collection.Name = Database.TestEdgeCollectionName;
            collection.Type = ArangoCollectionType.Edge;
            collection.WaitForSync = true;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // check collection data retrieved from server
            Assert.AreEqual(false, string.IsNullOrEmpty(collection.Id));
            Assert.AreEqual(Database.TestEdgeCollectionName, collection.Name);
            Assert.AreEqual(ArangoCollectionType.Edge, collection.Type);
            Assert.AreEqual(ArangoCollectionStatus.Loaded, collection.Status);
            Assert.AreEqual(true, collection.WaitForSync);
            Assert.AreEqual(false, collection.IsSystem);
            Assert.AreEqual(false, collection.IsVolatile);
            
            // delete collection from database
            bool isDeleted = db.Collection.Delete(collection.Name);
            
            // check if the collection was deleted from database
            Assert.AreEqual(true, isDeleted);
        }
        
        [Test()]
        public void Should_create_and_get_and_delete_collection()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            
            ArangoDatabase db = Database.GetTestDatabase();
            
            // set collection data
            ArangoCollection collection = new ArangoCollection();
            collection.Name = Database.TestDocumentCollectionName;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // get collection from database
            ArangoCollection returnedCollection = db.Collection.Get(Database.TestDocumentCollectionName);
            
            // check collection data retrieved from server
            Assert.AreEqual(collection.Id, returnedCollection.Id);
            Assert.AreEqual(collection.Name, returnedCollection.Name);
            Assert.AreEqual(collection.Type, returnedCollection.Type);
            Assert.AreEqual(collection.Status, returnedCollection.Status);
            
            // delete collection from database
            bool isDeleted = db.Collection.Delete(collection.Name);
            
            // check if the collection was deleted from database
            Assert.AreEqual(true, isDeleted);
        }

        [Test()]
        public void Should_create_and_clear_and_delete_collection()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            
            ArangoDatabase db = Database.GetTestDatabase();
            
            // set collection data
            ArangoCollection collection = new ArangoCollection();
            collection.Name = Database.TestDocumentCollectionName;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // clear collection data
            var isCleared = db.Collection.Clear(Database.TestDocumentCollectionName);
            Assert.AreEqual(true, isCleared);
            
            // delete collection from database
            bool isDeleted = db.Collection.Delete(collection.Name);
            
            // check if the collection was deleted from database
            Assert.AreEqual(true, isDeleted);
        }
    }
}
