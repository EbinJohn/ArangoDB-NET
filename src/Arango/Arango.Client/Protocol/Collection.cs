﻿using System.Dynamic;
using System.Net;

namespace Arango.Client.Protocol
{
    internal class Collection
    {
        private string _apiUri { get { return "_api/collection/"; } }
        private JsonParser _parser = new JsonParser();
        private ArangoNode _node;

        internal Collection(ArangoNode node)
        {
            _node = node;
        }

        internal ArangoCollection Post(string name, ArangoCollectionType type, bool waitForSync, long journalSize)
        {
            dynamic bodyObject = new ExpandoObject();
            bodyObject.name = name;
            bodyObject.type = (int)type;
            bodyObject.waitForSync = waitForSync;
            bodyObject.journalSize = journalSize;
            bodyObject.isSystem = false;

            var request = new Request();
            request.RelativeUri = _apiUri;
            request.Method = RequestMethod.POST.ToString();
            request.Body = _parser.Serialize(bodyObject);

            var response = _node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = (long)response.JsonObject.id;
                    collection.Name = response.JsonObject.name;
                    collection.WaitForSync = response.JsonObject.waitForSync;
                    collection.JournalSize = journalSize;
                    collection.Status = (ArangoCollectionStatus)response.JsonObject.status;
                    collection.Type = (ArangoCollectionType)response.JsonObject.type;
                    break;
                default:
                    break;
            }

            return collection;
        }

        internal ArangoCollection Get(int id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + id + "/properties";
            request.Method = RequestMethod.GET.ToString();

            var response = _node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = (long)response.JsonObject.id;
                    collection.Name = response.JsonObject.name;
                    collection.WaitForSync = response.JsonObject.waitForSync;
                    collection.JournalSize = (long)response.JsonObject.journalSize;
                    collection.Status = (ArangoCollectionStatus)response.JsonObject.status;
                    collection.Type = (ArangoCollectionType)response.JsonObject.type;
                    break;
                default:
                    break;
            }

            return collection;
        }

        // returns only ID, Name, Status and Type
        internal ArangoCollection Get(string collectionName)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + collectionName + "/properties";
            request.Method = RequestMethod.GET.ToString();

            var response = _node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = (long)response.JsonObject.id;
                    collection.Name = response.JsonObject.name;
                    collection.WaitForSync = response.JsonObject.waitForSync;
                    collection.JournalSize = (long)response.JsonObject.journalSize;
                    collection.Status = (ArangoCollectionStatus)response.JsonObject.status;
                    collection.Type = (ArangoCollectionType)response.JsonObject.type;
                    break;
                default:
                    break;
            }

            return collection;
        }
    }
}
