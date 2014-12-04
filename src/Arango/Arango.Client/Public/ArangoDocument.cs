﻿﻿using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ArangoDocument
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        
        internal ArangoDocument(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        public ArangoDocument CreateCollection(bool value)
        {
            // string because value will be stored in query string
            _parameters.String(ParameterName.CreateCollection, value.ToString().ToLower());
        	
        	return this;
        }
        
        public ArangoDocument IfMatch(string value)
        {
            // string because value will be stored in query string
            _parameters.String(ParameterName.IfMatch, value);
        	
        	return this;
        }
        
        public ArangoDocument IfNoneMatch(string value)
        {
            // string because value will be stored in query string
            _parameters.String(ParameterName.IfNoneMatch, value);
        	
        	return this;
        }
        
        public ArangoDocument WaitForSync(bool value)
        {
            // string because value will be stored in query string
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
        	return this;
        }
        
        #endregion
        
        #region Get (GET)
        
        public ArangoResult<Dictionary<string, object>> Get(string handle)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Document, "/" + handle);
            
            // optional: 
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: 
            request.TrySetHeaderParameter(ParameterName.IfNoneMatch, _parameters);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>);
                    }
                    break;
                case 412:
                    if (response.DataType == DataType.Document)
                    {
                        result.Value = (response.Data as Dictionary<string, object>);
                    }
                    break;
                case 304:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Create (POST)
        
        public ArangoResult<Dictionary<string, object>> Create(string collection, Dictionary<string, object> document)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Document, "");
            
            // required: target collection name
            request.QueryString.Add(ParameterName.Collection, collection);
            // required: document to be created
            request.Body = JSON.ToJSON(document);
            // optional: determines if target collection should be created if it doesn't exist
            request.TrySetQueryStringParameter(ParameterName.CreateCollection, _parameters);
            // optional: wait until data are synchronised to disk
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>);
                    }
                    break;
                case 400:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
    }
}
