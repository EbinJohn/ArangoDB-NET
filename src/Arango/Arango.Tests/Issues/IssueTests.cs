﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class IssueTests : IDisposable
    {
        public IssueTests()
		{
			Database.CreateTestDatabase(Database.TestDatabaseGeneral);
		}
        
        [Test()]
        public void Issue_No8_Guid_conversion()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName, ACollectionType.Document);
            var db = new ADatabase(Database.Alias);
            
            var demo = new IssueNo8Entity();
            demo.SomeOtherId = Guid.NewGuid();
            demo.Name = "My name";
            
            var createResult = db.Document.Create<IssueNo8Entity>(Database.TestDocumentCollectionName, demo);
            
            Assert.IsTrue(createResult.Success);
            
            var getresult = db.Document.Get<IssueNo8Entity>(createResult.Value.ID());
            
            Assert.IsTrue(getresult.Success);
            Assert.AreEqual(demo.SomeOtherId.ToString(), getresult.Value.SomeOtherId.ToString());
        }
        
        [Test()]
        public void Issue_No9_Enum_type_handling()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName, ACollectionType.Document);
            var db = new ADatabase(Database.Alias);
            
            var demo = new IssueNo9Entity();
            demo.SomeOtherId = Guid.NewGuid();
            demo.Name = "My name";
            demo.MyFavoriteColor = IssueNo9Entity.Color.Blue;
            
            var createResult = db.Document.Create<IssueNo9Entity>(Database.TestDocumentCollectionName, demo);
        
            Assert.IsTrue(createResult.Success);
            
            var getResult = db.Document.Get<IssueNo9Entity>(createResult.Value.ID());
            
            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(demo.MyFavoriteColor, getResult.Value.MyFavoriteColor);
            
            var getDocResult = db.Document.Get(createResult.Value.ID());
            
            Assert.IsTrue(getDocResult.Success);
            Assert.IsTrue(getDocResult.Value.IsString("MyFavoriteColor"));
            Assert.AreEqual(demo.MyFavoriteColor.ToString(), getDocResult.Value.String("MyFavoriteColor"));
            
            // change JSON serialization options to serialize enum types as values (integers and not strings)
            ASettings.JsonParameters.UseValuesOfEnums = true;
            
            var createResult2 = db.Document.Create<IssueNo9Entity>(Database.TestDocumentCollectionName, demo);
            
            Assert.IsTrue(createResult2.Success);
            
            var getDocResult2 = db.Document.Get(createResult2.Value.ID());
            
            Assert.IsTrue(getDocResult2.Success);
            Assert.IsTrue(getDocResult2.Value.IsLong("MyFavoriteColor"));
            Assert.AreEqual((int)demo.MyFavoriteColor, getDocResult2.Value.Int("MyFavoriteColor"));
        }
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
