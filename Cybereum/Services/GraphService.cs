using Gremlin.Net;
using Gremlin.Net.Driver;
//using Gremlin.Net.Structure.IO.GraphSON;
//using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cybereum.Services
{
    public class GraphService
    {        
        private readonly string hostname;
        private readonly int port;
        private readonly string authKey;
        private readonly string database;
        private readonly string collection;

        public GraphService(string hostname, int port, string authKey, string database, string collection)
        {
            this.hostname = hostname;
            this.port = port;
            this.authKey = authKey;
            this.database = database;
            this.collection = collection;
        }

        //[JSInvokable("GetGraphData")]
        public async Task<GraphData> GetGraphData()
        {
            var gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: $"/dbs/{database}/colls/{collection}", password: authKey);

            using (var gremlinClient = new GremlinClient(gremlinServer))
            {
                var gremlinScript = "g.V().hasLabel('activity').id()";
                var results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript);

                var nodes = results.Select(result => new Node
                {
                    Name = result.ContainsKey("name") ? Convert.ToString(result["name"]) : null,
                }).ToList();

                gremlinScript = "g.E().hasLabel('precedes').project('id', 'source', 'target', 'duration').by(id()).by(outV().id()).by(inV().id()).by(values('duration'))";
                results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript);

                var links = results.Select(result => new Link
                {
                    Source = (string)result["source"],
                    Target = (string)result["target"],
                    Value = (string)result["duration"]
                }).ToList();

                return new GraphData
                {
                    Nodes = nodes,
                    Links = links
                };
            }
        }

    }

    public class GraphData
    {
        public List<Node> Nodes { get; set; }
        public List<Link> Links { get; set; }
    }

    public class Node
    {
        public string Name { get; set; }
    }

    public class Link
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public string Value { get; set; }
    }
}

