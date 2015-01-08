﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class DocumentOperationsTests : IDisposable
    {
        public DocumentOperationsTests()
		{
			Database.CreateTestDatabase(Database.TestDatabaseGeneral);
			Database.CreateTestCollection(Database.TestDocumentCollectionName, ACollectionType.Document);
		}
        
        #region Create operations
        
        [Test()]
        public void Should_create_document()
        {
        	Database.ClearTestCollection(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Document
                .Create(Database.TestDocumentCollectionName, document);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }
        
        [Test()]
        public void Should_create_document_with_waitForSync()
        {
        	Database.ClearTestCollection(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Document
                .WaitForSync(true)
                .Create(Database.TestDocumentCollectionName, document);
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }

        [Test()]
        public void Should_create_document_from_generic_object()
        {
        	Database.ClearTestCollection(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            
            var dummy = new Dummy();
            dummy.Foo = "foo string value";
            dummy.Bar = 12345;
            
            var createResult = db.Document
                .Create(Database.TestDocumentCollectionName, dummy);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
            
            var getResult = db.Document
                .Get(createResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
            Assert.AreEqual(getResult.Value.String("foo"), dummy.Foo);
            Assert.AreEqual(getResult.Value.Int("bar"), dummy.Bar);
            Assert.AreEqual(0, dummy.Baz);
        }
        
        #endregion
        
        #region Check operations
        
        [Test()]
        public void Should_check_document()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            
            var checkResult = db.Document
                .Check(documents[0].String("_id"));
            
            Assert.AreEqual(200, checkResult.StatusCode);
            Assert.IsTrue(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_check_document_with_ifMatch()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            
            var checkResult = db.Document
                .IfMatch(documents[0].String("_rev"))
                .Check(documents[0].String("_id"));
            
            Assert.AreEqual(200, checkResult.StatusCode);
            Assert.IsTrue(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_check_document_with_ifMatch_and_return_412()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            
            var checkResult = db.Document
                .IfMatch("123456789")
                .Check(documents[0].String("_id"));
            
            Assert.AreEqual(412, checkResult.StatusCode);
            Assert.IsFalse(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_check_document_with_ifNoneMatch()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            
            var checkResult = db.Document
                .IfNoneMatch("123456789")
                .Check(documents[0].String("_id"));
            
            Assert.AreEqual(200, checkResult.StatusCode);
            Assert.IsTrue(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_check_document_with_ifNoneMatch_and_return_304()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            
            var checkResult = db.Document
                .IfNoneMatch(documents[0].String("_rev"))
                .Check(documents[0].String("_id"));
            
            Assert.AreEqual(304, checkResult.StatusCode);
            Assert.IsFalse(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, documents[0].String("_rev"));
        }
        
        #endregion
        
        #region Get operations
        
        [Test()]
        public void Should_get_document()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var getResult = db.Document
                .Get(documents[0].String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), documents[0].String("_rev"));
            Assert.AreEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("bar"), documents[0].String("bar"));
        }
        
        [Test()]
        public void Should_get_document_with_ifMatch()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var getResult = db.Document
                .IfMatch(documents[0].String("_rev"))
                .Get(documents[0].String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), documents[0].String("_rev"));
            Assert.AreEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("bar"), documents[0].String("bar"));
        }
        
        [Test()]
        public void Should_get_document_with_ifMatch_and_return_412()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var getResult = db.Document
                .IfMatch("123456789")
                .Get(documents[0].String("_id"));
            
            Assert.AreEqual(412, getResult.StatusCode);
            Assert.IsFalse(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_get_document_with_ifNoneMatch()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var getResult = db.Document
                .IfNoneMatch("123456789")
                .Get(documents[0].String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), documents[0].String("_rev"));
            Assert.AreEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("bar"), documents[0].String("bar"));
        }
        
        [Test()]
        public void Should_get_document_with_ifNoneMatch_and_return_304()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var getResult = db.Document
                .IfNoneMatch(documents[0].String("_rev"))
                .Get(documents[0].String("_id"));
            
            Assert.AreEqual(304, getResult.StatusCode);
            Assert.IsFalse(getResult.Success);
            Assert.IsFalse(getResult.HasValue);
        }
        
        [Test()]
        public void Should_get_document_as_generic_object()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var getResult = db.Document
                .Get<Dummy>(documents[0].String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(documents[0].String("foo"), getResult.Value.Foo);
            Assert.AreEqual(documents[0].Int("bar"), getResult.Value.Bar);
            Assert.AreEqual(0, getResult.Value.Baz);
        }
        
        #endregion
        
        #region Update operations
        
        [Test()]
        public void Should_update_document()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("bar", 54321)
                .Int("baz", 12345);
            
            var updateResult = db.Document
                .Update(documents[0].String("_id"), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), documents[0].String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.AreNotEqual(getResult.Value.Int("bar"), documents[0].Int("bar"));
            Assert.AreEqual(getResult.Value.Int("bar"), document.Int("bar"));
            Assert.AreEqual(getResult.Value.Int("baz"), document.Int("baz"));
        }
        
        [Test()]
        public void Should_update_document_with_waitForSync()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("bar", 54321)
                .Int("baz", 12345);
            
            var updateResult = db.Document
                .WaitForSync(true)
                .Update(documents[0].String("_id"), document);
            
            Assert.AreEqual(201, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), documents[0].String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.AreNotEqual(getResult.Value.Int("bar"), documents[0].Int("bar"));
            Assert.AreEqual(getResult.Value.Int("bar"), document.Int("bar"));
            Assert.AreEqual(getResult.Value.Int("baz"), document.Int("baz"));
        }
        
        [Test()]
        public void Should_update_document_with_ifMatch()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("bar", 54321)
                .Int("baz", 12345);
            
            var updateResult = db.Document
                .IfMatch(documents[0].String("_rev"))
                .Update(documents[0].String("_id"), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), documents[0].String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.AreNotEqual(getResult.Value.Int("bar"), documents[0].Int("bar"));
            Assert.AreEqual(getResult.Value.Int("bar"), document.Int("bar"));
            Assert.AreEqual(getResult.Value.Int("baz"), document.Int("baz"));
        }
        
        [Test()]
        public void Should_update_document_with_ifMatch_and_return_412()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("bar", 54321)
                .Int("baz", 12345);
            
            var updateResult = db.Document
                .IfMatch("123456789")
                .Update(documents[0].String("_id"), document);
            
            Assert.AreEqual(412, updateResult.StatusCode);
            Assert.IsFalse(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(updateResult.Value.String("_rev"), documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_update_document_with_ifMatch_and_lastUpdatePolicy()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("bar", 54321)
                .Int("baz", 12345);
            
            var updateResult = db.Document
                .IfMatch("123456789", AUpdatePolicy.Last)
                .Update(documents[0].String("_id"), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), documents[0].String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.AreNotEqual(getResult.Value.Int("bar"), documents[0].Int("bar"));
            Assert.AreEqual(getResult.Value.Int("bar"), document.Int("bar"));
            Assert.AreEqual(getResult.Value.Int("baz"), document.Int("baz"));
        }
        
        [Test()]
        public void Should_update_document_with_keepNull()
        {
            var db = new ADatabase(Database.Alias);

            var newDocument = new Dictionary<string, object>()
                .String("foo", "some string")
                .Object("bar", null);
            
            var createResult = db.Document
                .Create(Database.TestDocumentCollectionName, newDocument);
            
            newDocument.Merge(createResult.Value);
            
            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Object("baz", null);
            
            var updateResult = db.Document
                .KeepNull(false)
                .Update(newDocument.String("_id"), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), newDocument.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), newDocument.String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), newDocument.String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.IsTrue(getResult.Value.Has("bar"));
            
            Assert.IsFalse(getResult.Value.Has("baz"));
        }
        
        [Test()]
        public void Should_update_document_with_mergeArrays_set_to_true()
        {
            var db = new ADatabase(Database.Alias);

            var newDocument = new Dictionary<string, object>()
                .String("foo", "some string")
                .Document("bar", new Dictionary<string, object>().String("foo", "string value"));
            
            var createResult = db.Document
                .Create(Database.TestDocumentCollectionName, newDocument);
            
            newDocument.Merge(createResult.Value);
            
            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Document("bar", new Dictionary<string, object>().String("bar", "other string value"));
            
            var updateResult = db.Document
                .MergeArrays(true) // this is also default behavior
                .Update(newDocument.String("_id"), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), newDocument.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), newDocument.String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), newDocument.String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.IsTrue(getResult.Value.Has("bar.foo"));
            
            Assert.IsTrue(getResult.Value.Has("bar.bar"));
        }
        
        [Test()]
        public void Should_update_document_with_mergeArrays_set_to_false()
        {
            var db = new ADatabase(Database.Alias);

            var newDocument = new Dictionary<string, object>()
                .String("foo", "some string")
                .Document("bar", new Dictionary<string, object>().String("foo", "string value"));
            
            var createResult = db.Document
                .Create(Database.TestDocumentCollectionName, newDocument);
            
            newDocument.Merge(createResult.Value);
            
            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Document("bar", new Dictionary<string, object>().String("bar", "other string value"));
            
            var updateResult = db.Document
                .MergeArrays(false)
                .Update(newDocument.String("_id"), document);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), newDocument.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), newDocument.String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), newDocument.String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.IsFalse(getResult.Value.Has("bar.foo"));
            
            Assert.IsTrue(getResult.Value.Has("bar.bar"));
        }
        
        [Test()]
        public void Should_update_document_with_generic_object()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var dummy = new Dummy();
            dummy.Foo = "some other new string";
            dummy.Bar = 54321;
            dummy.Baz = 12345;
            
            var updateResult = db.Document
                .Update(documents[0].String("_id"), dummy);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), documents[0].String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), dummy.Foo);
            
            Assert.AreNotEqual(getResult.Value.Int("bar"), documents[0].Int("bar"));
            Assert.AreEqual(getResult.Value.Int("bar"), dummy.Bar);
            Assert.AreEqual(getResult.Value.Int("baz"), dummy.Baz);
        }
        
        #endregion
        
        #region Replace operations
        
        [Test()]
        public void Should_replace_document()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("baz", 54321);
            
            var replaceResult = db.Document
                .Replace(documents[0].String("_id"), document);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreNotEqual(replaceResult.Value.String("_rev"), documents[0].String("_rev"));
            
            var getResult = db.Document
                .Get(replaceResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), replaceResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), replaceResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), replaceResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.AreEqual(getResult.Value.Int("baz"), document.Int("baz"));
            
            Assert.IsFalse(getResult.Value.Has("bar"));
        }
        
        [Test()]
        public void Should_replace_document_with_waitForSync()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("baz", 54321);
            
            var replaceResult = db.Document
                .WaitForSync(true)
                .Replace(documents[0].String("_id"), document);
            
            Assert.AreEqual(201, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreNotEqual(replaceResult.Value.String("_rev"), documents[0].String("_rev"));
            
            var getResult = db.Document
                .Get(replaceResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), replaceResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), replaceResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), replaceResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.AreEqual(getResult.Value.Int("baz"), document.Int("baz"));
            
            Assert.IsFalse(getResult.Value.Has("bar"));
        }
        
        [Test()]
        public void Should_replace_document_with_ifMatch()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("baz", 54321);
            
            var replaceResult = db.Document
                .IfMatch(documents[0].String("_rev"))
                .Replace(documents[0].String("_id"), document);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreNotEqual(replaceResult.Value.String("_rev"), documents[0].String("_rev"));
            
            var getResult = db.Document
                .Get(replaceResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), replaceResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), replaceResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), replaceResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.AreEqual(getResult.Value.Int("baz"), document.Int("baz"));
            
            Assert.IsFalse(getResult.Value.Has("bar"));
        }
        
        [Test()]
        public void Should_replace_document_with_ifMatch_and_return_412()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("baz", 54321);
            
            var replaceResult = db.Document
                .IfMatch("123456789")
                .Replace(documents[0].String("_id"), document);
            
            Assert.AreEqual(412, replaceResult.StatusCode);
            Assert.IsFalse(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(replaceResult.Value.String("_rev"), documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_replace_document_with_ifMatch_and_lastUpdatePolicy()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("baz", 54321);
            
            var replaceResult = db.Document
                .IfMatch("123456789", AUpdatePolicy.Last)
                .Replace(documents[0].String("_id"), document);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreNotEqual(replaceResult.Value.String("_rev"), documents[0].String("_rev"));
            
            var getResult = db.Document
                .Get(replaceResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), replaceResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), replaceResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), replaceResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            
            Assert.AreEqual(getResult.Value.Int("baz"), document.Int("baz"));
            
            Assert.IsFalse(getResult.Value.Has("bar"));
        }
        
        [Test()]
        public void Should_replace_document_with_generic_object()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var dummy = new Dummy();
            dummy.Foo = "some other new string";
            dummy.Baz = 54321;
            
            var replaceResult = db.Document
                .Replace(documents[0].String("_id"), dummy);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreNotEqual(replaceResult.Value.String("_rev"), documents[0].String("_rev"));
            
            var getResult = db.Document
                .Get(replaceResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), replaceResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), replaceResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), replaceResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), dummy.Foo);
            
            Assert.AreEqual(getResult.Value.Int("baz"), dummy.Baz);
            
            Assert.AreEqual(0, dummy.Bar);
        }
        
        #endregion
        
        #region Delete operations
        
        [Test()]
        public void Should_delete_document()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            
            var deleteResult = db.Document
                .Delete(documents[0].String("_id"));
            
            Assert.AreEqual(202, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(deleteResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(deleteResult.Value.String("_rev"), documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_delete_document_with_waitForSync()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            
            var deleteResult = db.Document
                .WaitForSync(true)
                .Delete(documents[0].String("_id"));
            
            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(deleteResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(deleteResult.Value.String("_rev"), documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_delete_document_with_ifMatch()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            
            var deleteResult = db.Document
                .IfMatch(documents[0].String("_rev"))
                .Delete(documents[0].String("_id"));
            
            Assert.AreEqual(202, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(deleteResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(deleteResult.Value.String("_rev"), documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_delete_document_with_ifMatch_and_return_412()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var deleteResult = db.Document
                .IfMatch("123456789")
                .Delete(documents[0].String("_id"));
            
            Assert.AreEqual(412, deleteResult.StatusCode);
            Assert.IsFalse(deleteResult.Success);
            Assert.AreEqual(deleteResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(deleteResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(deleteResult.Value.String("_rev"), documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_delete_document_with_ifMatch_and_lastUpdatePolicy()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            
            var deleteResult = db.Document
                .IfMatch("123456789", AUpdatePolicy.Last)
                .Delete(documents[0].String("_id"));
            
            Assert.AreEqual(202, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.AreEqual(deleteResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(deleteResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(deleteResult.Value.String("_rev"), documents[0].String("_rev"));
        }
        
        #endregion
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
