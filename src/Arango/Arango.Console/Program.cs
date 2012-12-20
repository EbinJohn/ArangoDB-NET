﻿using System;
using System.IO;
using Arango.Client;
using System.Dynamic;

namespace Arango.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string alias = "test";
            string[] connectionString = File.ReadAllText(@"..\..\..\..\..\ConnectionString.txt").Split(';');
            
            ArangoNode node = new ArangoNode(
                connectionString[0],
                int.Parse(connectionString[1]),
                connectionString[2],
                connectionString[3], 
                alias
            );
            ArangoClient.Nodes.Add(node);

            ArangoDatabase database = new ArangoDatabase(alias);
            ArangoDocument document = database.GetDocument("10843274/12481674", "x12481674");

            System.Console.WriteLine("ID: {0}, Rev: {1}, Data: {2}", document.ID, document.Revision, document.Data);

            ArangoDocument doc = new ArangoDocument();
            doc.Json.foo = "abc";
            doc.Json.bar = new ExpandoObject();
            doc.Json.bar.baz = 123;

            System.Console.WriteLine("foo: {0} {1}", doc.Has("foo"), doc.Json.foo);
            System.Console.WriteLine("bar: {0} {1}", doc.Has("bar"), doc.Json.bar);
            System.Console.WriteLine("bar.baz: {0} {1}", doc.Has("bar.baz"), doc.Json.bar.baz);
            System.Console.WriteLine("non: {0}", doc.Has("non"));
            System.Console.WriteLine("non.exist: {0}", doc.Has("non.exist"));

            System.Console.ReadLine();
        }
    }
}
