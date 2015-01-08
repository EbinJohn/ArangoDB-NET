﻿using System;
using System.Collections.Generic;

namespace Arango.Client
{
    // contains ArangoDB specific extension methods, Dictator implementation code can be found in external libraries
    public static partial class DictionaryExtensions
    {
        #region _id
        
        /// <summary>
        /// Checks if `_id` field is present and has valid format.
        /// </summary>
        public static bool HasID(this Dictionary<string, object> dictionary)
        {
            return !string.IsNullOrEmpty(ID(dictionary));
        }
        
        /// <summary>
        /// Retrieves value of `_id` field. If the field is missing or has invalid format null value is returned.
        /// </summary>
        public static string ID(this Dictionary<string, object> dictionary)
        {
            string id;
            
            try
            {
                id = String(dictionary, "_id");
                
                if (!ArangoDocument.IsID(id))
                {
                    id = null;
                }
            }
            catch (Exception)
            {
                id = null;
            }
            
            return id;
        }
        
        /// <summary>
        /// Stores `_id` field value.
        /// </summary>
        /// <exception cref="InvalidFieldTypeException">Specified value has invalid _id field format.</exception>
        public static Dictionary<string, object> ID(this Dictionary<string, object> dictionary, string id)
        {
            if (!ArangoDocument.IsID(id))
            {
                throw new InvalidFieldTypeException("Specified value (" + id + ") has invalid _id field format.");
            }
            
            SetFieldValue(dictionary, "_id", id);
            
            return dictionary;
        }
        
        #endregion
        
        #region _key
        
        /// <summary>
        /// Checks if `_key` field is present and has valid format.
        /// </summary>
        public static bool HasKey(this Dictionary<string, object> dictionary)
        {
            return !string.IsNullOrEmpty(Key(dictionary));
        }
        
        /// <summary>
        /// Retrieves value of `_key` field. If the field is missing or has invalid format null value is returned.
        /// </summary>
        public static string Key(this Dictionary<string, object> dictionary)
        {
            string key;
            
            try
            {
                key = String(dictionary, "_key");
                
                if (!ArangoDocument.IsKey(key))
                {
                    key = null;
                }
            }
            catch (Exception)
            {
                key = null;
            }
            
            return key;
        }
        
        /// <summary>
        /// Stores `_key` field value.
        /// </summary>
        /// <exception cref="InvalidFieldTypeException">Specified value has invalid _key field format.</exception>
        public static Dictionary<string, object> Key(this Dictionary<string, object> dictionary, string key)
        {
            if (!ArangoDocument.IsKey(key))
            {
                throw new InvalidFieldTypeException("Specified value (" + key + ") has invalid _key field format.");
            }
            
            SetFieldValue(dictionary, "_key", key);
            
            return dictionary;
        }
        
        #endregion
        
        #region _rev
        
        /// <summary>
        /// Checks if `_rev` field is present and has valid format.
        /// </summary>
        public static bool HasRev(this Dictionary<string, object> dictionary)
        {
            return !string.IsNullOrEmpty(Rev(dictionary));
        }
        
        /// <summary>
        /// Retrieves value of `_rev` field. If the field is missing or has invalid format null value is returned.
        /// </summary>
        public static string Rev(this Dictionary<string, object> dictionary)
        {
            string rev;
            
            try
            {
                rev = String(dictionary, "_rev");
                
                if (!ArangoDocument.IsRev(rev))
                {
                    rev = null;
                }
            }
            catch (Exception)
            {
                rev = null;
            }
            
            return rev;
        }
        
        /// <summary>
        /// Stores `_rev` field value.
        /// </summary>
        /// <exception cref="InvalidFieldTypeException">Specified value has invalid _rev field format.</exception>
        public static Dictionary<string, object> Rev(this Dictionary<string, object> dictionary, string rev)
        {
            if (!ArangoDocument.IsRev(rev))
            {
                throw new InvalidFieldTypeException("Specified value (" + rev + ") has invalid _rev field format.");
            }   
            
            SetFieldValue(dictionary, "_rev", rev);
            
            return dictionary;
        }
        
        #endregion
    }
}
