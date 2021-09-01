using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public long InsertToTable<T>(T table)
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
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // BulkWriteAsync returns a Task<T> so we need to get the value of Result property
            var bulkWriteResult = bulkWriteTask.GetType().GetProperty("Result").GetValue(bulkWriteTask);
            sw.Stop();

            //=======================================

            var bulkWriteResultType = bulkWriteResult.GetType();
            var isAcknowledged = bulkWriteResultType.GetProperty("IsAcknowledged", BindingFlags.Instance | BindingFlags.Public).GetValue(bulkWriteResult);
            var insertedCount = bulkWriteResultType.GetProperty("InsertedCount", BindingFlags.Instance | BindingFlags.Public).GetValue(bulkWriteResult);

            ConsolePrinter.PrintLine($"Mongo DB -> OK?: {isAcknowledged} - Inserted Count: {insertedCount}", ConsoleColor.Green);

            return sw.ElapsedMilliseconds;
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

            public BsonValue id { get; set; }
            public BsonValue tagName { get; set; }
            public BsonValue count { get; set; }

            public Tag(Tags.Row row)
            {
                id = BsonValue.Create(row.Id);
                tagName = BsonValue.Create(row.TagName);
                count = BsonValue.Create(row.Count);
            }
        }

        private class UsersBadge
        {
            [BsonId] public ObjectId _id { get; set; }
            public BsonValue id { get; set; }
            public BsonValue userId { get; set; }
            public BsonValue badgeId { get; set; }
            public BsonValue date { get; set; }

            public UsersBadge(UsersBadges.Row row)
            {
                id = BsonValue.Create(row.Id);
                userId = BsonValue.Create(row.UserId);
                badgeId = BsonValue.Create(row.BadgeId);
                date = BsonValue.Create(row.Date);
            }

        }

        private class Badge
        {
            [BsonId] public ObjectId _id { get; set; }
            public BsonValue id { get; set; }
            public BsonValue name { get; set; }

            public Badge(Badges.Row row)
            {
                id = BsonValue.Create(row.Id);
                name = BsonValue.Create(row.Name);
            }

        }
        private class Vote
        {
            [BsonId] public ObjectId _id { get; set; }

            public BsonValue id { get; set; }
            public BsonValue postId { get; set; }
            public BsonValue voteTypeId { get; set; }
            public BsonValue creationDate { get; set; }

            public Vote(Votes.Row row)
            {
                id = BsonValue.Create(row.Id);
                postId = BsonValue.Create(row.PostId);
                voteTypeId = BsonValue.Create(row.VoteTypeId);
                creationDate = BsonValue.Create(row.CreationDate);
            }
        }

        private class Comment
        {
            [BsonId] public ObjectId _id { get; set; }

            public BsonValue id { get; set; }
            public BsonValue score { get; set; }
            public BsonValue text { get; set; }
            public BsonValue contentLicense { get; set; }
            public BsonValue userId { get; set; }
            public BsonValue creationDate { get; set; }

            public Comment(Comments.Row row)
            {
                id = BsonValue.Create(row.Id);
                score = BsonValue.Create(row.Score);
                text = BsonValue.Create(row.Text);
                contentLicense = BsonValue.Create(row.ContentLicense);
                userId = BsonValue.Create(row.UserId);
                creationDate = BsonValue.Create(row.CreationDate);
            }
        }

        private class User
        {
            [BsonId] public ObjectId _id { get; set; }

            public BsonValue id { get; set; }
            public BsonValue reputation { get; set; }
            public BsonValue creationDate { get; set; }
            public BsonValue displayName { get; set; }
            public BsonValue lastAccessDate { get; set; }
            public BsonValue websiteUrl { get; set; }
            public BsonValue location { get; set; }
            public BsonValue aboutMe { get; set; }
            public BsonValue views { get; set; }
            public BsonValue upVotes { get; set; }
            public BsonValue downVotes { get; set; }
            public BsonValue accountId { get; set; }

            public User(Users.Row row)
            {
                id = row.Id;
                reputation = BsonValue.Create(row.Reputation);
                creationDate = BsonValue.Create(row.CreationDate);
                displayName = BsonValue.Create(row.DisplayName);
                lastAccessDate = BsonValue.Create(row.LastAccessDate);
                websiteUrl = BsonValue.Create(row.WebsiteUrl);
                location = BsonValue.Create(row.Location);
                aboutMe = BsonValue.Create(row.AboutMe);
                views = BsonValue.Create(row.Views);
                upVotes = BsonValue.Create(row.UpVotes);
                downVotes = BsonValue.Create(row.DownVotes);
                accountId = BsonValue.Create(row.AccountId);
            }
        }

        //private class User
        //{
        //    [BsonId] public ObjectId _id { get; set; }

        //    public BsonInt32 id { get; set; }
        //    public BsonInt32 reputation { get; set; }
        //    public BsonDateTime creationDate { get; set; }
        //    public BsonString displayName { get; set; }
        //    public BsonDateTime lastAccessDate { get; set; }
        //    public BsonString websiteUrl { get; set; }
        //    public BsonString location { get; set; }
        //    public BsonString aboutMe { get; set; }
        //    public BsonInt32 views { get; set; }
        //    public BsonInt32 upVotes { get; set; }
        //    public BsonInt32 downVotes { get; set; }
        //    public BsonInt32 accountId { get; set; }

        //    public User(Users.Row row)
        //    {
        //        id = row.Id;
        //        reputation = row.Reputation;
        //        creationDate = row.CreationDate;
        //        displayName = row.DisplayName;
        //        lastAccessDate = row.LastAccessDate;
        //        websiteUrl = row.WebsiteUrl ?? BsonString.Empty;
        //        location = row.Location ?? BsonString.Empty;
        //        aboutMe = row.AboutMe ?? BsonString.Empty;
        //        views = row.Views;
        //        upVotes = row.UpVotes;
        //        downVotes = row.DownVotes;
        //        accountId = row.AccountId;
        //    }
        //}

        private class Post
        {
            [BsonId] public ObjectId _id { get; set; }

            public BsonValue id { get; set; }
            public BsonValue postTypeId { get; set; }
            public BsonValue acceptedAnswerId { get; set; }
            public BsonValue score { get; set; }
            public BsonValue viewCount { get; set; }
            public BsonValue body { get; set; }
            public BsonValue ownerUserId { get; set; }
            public BsonValue lastEditorUserId { get; set; }
            public BsonValue title { get; set; }
            public BsonValue tags { get; set; }
            public BsonValue answerCount { get; set; }
            public BsonValue commentCount { get; set; }
            public BsonValue favoriteCount { get; set; }
            public BsonValue contentLicense { get; set; }
            public BsonValue creationDate { get; set; }
            public BsonValue lastActivityDate { get; set; }
            public BsonValue lastEditDate { get; set; }

            public Post(Posts.Row row)
            {
                id = BsonValue.Create(row.Id);
                postTypeId = BsonValue.Create(row.PostTypeId);
                acceptedAnswerId = BsonValue.Create(row.AcceptedAnswerId);
                score = BsonValue.Create(row.Score);
                viewCount = BsonValue.Create(row.ViewCount);
                body = BsonValue.Create(row.Body);
                ownerUserId = BsonValue.Create(row.OwnerUserId);
                lastEditorUserId = BsonValue.Create(row.LastEditorUserId);
                title = BsonValue.Create(row.Title);
                tags = BsonValue.Create(row.Tags);
                answerCount = BsonValue.Create(row.AnswerCount);
                commentCount = BsonValue.Create(row.CommentCount);
                favoriteCount = BsonValue.Create(row.FavoriteCount);
                contentLicense = BsonValue.Create(row.ContentLicense);
                creationDate = BsonValue.Create(row.CreationDate);
                lastActivityDate = BsonValue.Create(row.LastActivityDate);
                lastEditDate = BsonValue.Create(row.LastEditDate);
            }
        }
    }
}
