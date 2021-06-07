using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ideal_giggle
{
    public class MongoAdapter
    {
        public string ConnectionString { get; set; }

        public MongoAdapter(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void AddPosts(Votes votes)
        {
            var client = new MongoClient("mongodb+srv://<username>:<password>@<cluster-address>/test?w=majority");
            var db = client.GetDatabase("...databaseName...");
            var postsCollection = db.GetCollection<Vote>("posts");

            var listWrites = votes.Rows
                                    .Select(p => new InsertOneModel<Vote>(new Vote(){
                                                                                    id = p.Id,
                                                                                    postId = p.PostId,
                                                                                    voteTypeId = p.VoteTypeId,
                                                                                    creationDate = p.CreationDate,
                                                                                }))
                                    .ToList();


            var resultWrites = postsCollection.BulkWriteAsync(listWrites).Result;

            Console.WriteLine($"OK?: {resultWrites.IsAcknowledged} - Inserted Count: {resultWrites.InsertedCount}");
        }
        
        private class Vote
        {
            [BsonId] public ObjectId _id { get; set; }

            public BsonInt32 id { get; set; }
            public BsonInt32 postId { get; set; }
            public BsonInt32 voteTypeId { get; set; }
            public BsonDateTime creationDate { get; set; }
            
        }
    }
}
