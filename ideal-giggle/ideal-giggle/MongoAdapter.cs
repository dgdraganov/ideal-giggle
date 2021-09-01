using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ideal_giggle
{
    public class MongoAdapter : IDbAdapter
    {
        private string ConnectionString { get; }
        public string DataBase { get; }

        public string Name{ get; }

        public MongoAdapter()
        {
            ConnectionString = "mongodb://127.0.0.1:27017";
            DataBase = "MongoDB";
            Name = "Mongo Adapter";
        }

        public void InsertToTable<T>(T table)
        {
            var tableType = table.GetType();

            string collectionName = tableType.Name.ToLower();
            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DataBase);

            if (!CollectionExists(db, collectionName))
            {
                CreateCollectionOptions options = new CreateCollectionOptions();
                options.Capped = false;
                db.CreateCollection(collectionName, options);
            }

            //=======================================
            // Get Vote dynamically based on Votes (actial generic T) 
            // var coll = db.GetCollection<Vote>(collectionName);
            var correspondingTypeName = tableType.Name.TrimEnd('s'); // Vote type name (corresponds to Votes parameter)
            var getCollectionMethod = db.GetType().GetMethod("GetCollection", BindingFlags.Public | BindingFlags.Instance); 
            var correspondingType = typeof(MongoAdapter).GetNestedType(correspondingTypeName, BindingFlags.NonPublic); // Vote type 
            var getCollectionInfo = getCollectionMethod.MakeGenericMethod(correspondingType); // make GetCollection generic
            var collectionResult = getCollectionInfo.Invoke(db, new[] { collectionName, null }); // call db.GetCollection<Vote>
            //=======================================


            //=======================================
            // Getting all the table rows to be inserted
            // and parsing them to object[]
            var rowsPropValue = table.GetType().GetProperty("Rows", BindingFlags.Instance | BindingFlags.Public).GetValue(table);
            var rows = (rowsPropValue as IEnumerable<object>).ToArray();
            //=======================================


            //=======================================
            // Prepare InsertOneModel type for usage 
            var insertOnModelType = Assembly
                                        .GetAssembly(typeof(MongoClient))
                                        .GetType("MongoDB.Driver.InsertOneModel`1", true)
                                        .MakeGenericType(correspondingType); // InsertOneModel<Vote>

            // Get InsertOneModel<Vote> constructor
            var insertOnModelTypeConstructor = insertOnModelType.GetConstructor(new[] { correspondingType });
            //=======================================

            // Craete empty Array to be filled out with InsertOneModel instances
            var listToInsert = Array.CreateInstance(insertOnModelType, rows.Length);

            var rowType = typeof(T).GetNestedType("Row");   // class Votes.Row
            var correspondingTypeConstructor = correspondingType.GetConstructor(new[] { rowType }); // Vote(Votes.Row) constructor
            for (int i = 0; i < rows.Length; i++)
            {
                // new Vote(p)
                var argForInsertOnModel = correspondingTypeConstructor
                                                                .Invoke(new[] { rows[i] });

                // new InsertOneModel<Vote>( ... )
                var insertOnModelTypeInstance = insertOnModelTypeConstructor
                                                                .Invoke(new[] { argForInsertOnModel });
                // Add to list
                listToInsert.SetValue(insertOnModelTypeInstance, i);
            }

            //=======================================
            // Calling BulkWriteAsync and getting the result
            // get BulkWriteAsync method
            var bulkWriteAsyncMethod = collectionResult.GetType().GetMethod("BulkWriteAsync",
                                                                            new[] { listToInsert.GetType(),
                                                                                typeof(BulkWriteOptions),
                                                                                typeof(CancellationToken) });
            // invoke BulkWriteAsync method
            var bulkWriteTask = bulkWriteAsyncMethod.Invoke(collectionResult,
                                                                new object[] {
                                                                        listToInsert,
                                                                        null,
                                                                        default(CancellationToken) });

            // BulkWriteAsync returns a Task<T> so we need to get the value of Result property
            var bulkWriteResult = bulkWriteTask.GetType().GetProperty("Result").GetValue(bulkWriteTask);
            //=======================================

            var bulkWriteResultType = bulkWriteResult.GetType();
            var isAcknowledged = bulkWriteResultType.GetProperty("IsAcknowledged", BindingFlags.Instance | BindingFlags.Public).GetValue(bulkWriteResult);
            var insertedCount = bulkWriteResultType.GetProperty("InsertedCount", BindingFlags.Instance | BindingFlags.Public).GetValue(bulkWriteResult);

            ConsolePrinter.PrintLine($"OK?: {isAcknowledged} - Inserted Count: {insertedCount}", ConsoleColor.Green);
        }

        private bool CollectionExists(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var options = new ListCollectionNamesOptions { Filter = filter };

            return database.ListCollectionNames(options).Any();
        }

        private class Tag
        {
            [BsonId] public ObjectId _id { get; set; }

            public BsonInt32 id { get; set; }
            public BsonString tagName { get; set; }
            public BsonInt32 count { get; set; }

            public Tag(Tags.Row row)
            {
                id = row.Id;
                tagName = row.TagName;
                count = row.Count;
            }
        }

        private class UserBadge
        {
            [BsonId] public ObjectId _id { get; set; }
            public BsonInt32 id { get; set; }
            public BsonInt32 userId { get; set; }
            public BsonInt32 badgeId { get; set; }
            public BsonDateTime date { get; set; }

            public UserBadge(UsersBadges.Row row)
            {
                id = row.Id;
                userId = row.UserId;
                badgeId = row.BadgeId;
                date = row.Date;
            }

        }

        private class Badge
        {
            [BsonId] public ObjectId _id { get; set; }
            public BsonInt32 id { get; set; }
            public BsonString name { get; set; }

            public Badge(Badges.Row row)
            {
                id = row.Id;
                name = row.Name;
            }

        }
        private class Vote
        {
            [BsonId] public ObjectId _id { get; set; }

            public BsonInt32 id { get; set; }
            public BsonInt32 postId { get; set; }
            public BsonInt32 voteTypeId { get; set; }
            public BsonDateTime creationDate { get; set; }

            public Vote(Votes.Row row)
            {
                id = row.Id;
                postId = row.PostId;
                voteTypeId = row.VoteTypeId;
                creationDate = row.CreationDate;
            }
        }

        private class Comment
        {
            [BsonId] public ObjectId _id { get; set; }

            public BsonInt32 id { get; set; }
            public BsonInt32 score { get; set; }
            public BsonString text { get; set; }
            public BsonString contentLicense { get; set; }
            public BsonInt32 userId { get; set; }
            public BsonDateTime creationDate { get; set; }

            public Comment(Comments.Row row)
            {
                id = row.Id;
                score = row.Score;
                text = row.Text;
                contentLicense = row.ContentLicense;
                userId = row.UserId;
                creationDate = row.CreationDate;
            }
        }

        private class User
        {
            [BsonId] public ObjectId _id { get; set; }

            public BsonInt32 id { get; set; }
            public BsonInt32 reputation { get; set; }
            public BsonDateTime creationDate { get; set; }
            public BsonString displayName { get; set; }
            public BsonDateTime lastAccessDate { get; set; }
            public BsonString websiteUrl { get; set; }
            public BsonString location { get; set; }
            public BsonString aboutMe { get; set; }
            public BsonInt32 views { get; set; }
            public BsonInt32 upVotes { get; set; }
            public BsonInt32 downVotes { get; set; }
            public BsonInt32 accountId { get; set; }

            public User(Users.Row row)
            {
                id = row.Id;
                reputation = row.Reputation;
                creationDate = row.CreationDate;
                displayName = row.DisplayName;
                lastAccessDate = row.LastAccessDate;
                websiteUrl = row.WebsiteUrl ?? BsonString.Empty;
                location = row.Location ?? BsonString.Empty;
                aboutMe = row.AboutMe ?? BsonString.Empty;
                views = row.Views;
                upVotes = row.UpVotes;
                downVotes = row.DownVotes;
                accountId = row.AccountId;
            }
        }

        private class Post
        {
            [BsonId] public ObjectId _id { get; set; }

            public BsonInt32 id { get; set; }
            public BsonInt32 postTypeId { get; set; }
            public BsonInt32 acceptedAnswerId { get; set; }
            public BsonInt32 score { get; set; }
            public BsonInt32 viewCount { get; set; }
            public BsonString body { get; set; }
            public BsonInt32 ownerUserId { get; set; }
            public BsonInt32 lastEditorUserId { get; set; }
            public BsonString title { get; set; }
            public BsonString tags { get; set; }
            public BsonInt32 answerCount { get; set; }
            public BsonInt32 commentCount { get; set; }
            public BsonInt32 favoriteCount { get; set; }
            public BsonString contentLicense { get; set; }
            public BsonDateTime creationDate { get; set; }
            public BsonDateTime lastActivityDate { get; set; }
            public BsonDateTime lastEditDate { get; set; }

            public Post(Posts.Row row)
            {
                id = row.Id;
                postTypeId = row.PostTypeId;
                acceptedAnswerId = row.AcceptedAnswerId;
                score = row.Score;
                viewCount = row.ViewCount;
                body = row.Body;
                ownerUserId = row.OwnerUserId;
                lastEditorUserId = row.LastEditorUserId;
                title = row.Title;
                tags = row.Tags;
                answerCount = row.AnswerCount;
                commentCount = row.CommentCount;
                favoriteCount = row.FavoriteCount;
                contentLicense = row.ContentLicense;
                creationDate = row.CreationDate;
                lastActivityDate = row.LastActivityDate;
                lastEditDate = row.LastEditDate;
            }
        }
    }
}
