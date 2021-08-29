using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Linq;

namespace ideal_giggle
{
    public class MongoAdapter
    {
        private string ConnectionString { get; }
        public string DataBase { get; }

        public MongoAdapter()
        {
            ConnectionString = "mongodb://127.0.0.1:27017";
            DataBase = "mylib";
        }

        public void FillVotesTable(Votes votesTable)
        {
            string collectionName = nameof(Votes).ToLower();

            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DataBase);

            CreateCollectionOptions options = new CreateCollectionOptions();
            options.Capped = false;

            db.CreateCollection(collectionName, options);

            var postsCollection = db.GetCollection<Vote>(collectionName);

            var listWrites = votesTable.Rows
                                  .Select(p => new InsertOneModel<Vote>(new Vote(p)))
                                  .ToList();

            var resultWrites = postsCollection.BulkWriteAsync(listWrites).Result;

            ConsolePrinter.PrintLine($"OK?: {resultWrites.IsAcknowledged} - Inserted Count: {resultWrites.InsertedCount}", ConsoleColor.Green);
        }

        public void FillCommentsTable(Comments commentsTable)
        {
            string collectionName = nameof(Comments).ToLower();

            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DataBase);

            CreateCollectionOptions options = new CreateCollectionOptions();
            options.Capped = false;

            db.CreateCollection(collectionName, options);

            var commentsCollection = db.GetCollection<Comment>(collectionName);

            var listWrites = commentsTable.Rows
                                  .Select(p => new InsertOneModel<Comment>(new Comment(p)))
                                  .ToList();

            var resultWrites = commentsCollection.BulkWriteAsync(listWrites).Result;

            ConsolePrinter.PrintLine($"OK?: {resultWrites.IsAcknowledged} - Inserted Count: {resultWrites.InsertedCount}", ConsoleColor.Green);
        }

        public void FillUsersTable(Users usersTable)
        {
            string collectionName = nameof(Users).ToLower();

            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DataBase);

            CreateCollectionOptions options = new CreateCollectionOptions();
            options.Capped = false;

            db.CreateCollection(collectionName, options);

            var commentsCollection = db.GetCollection<User>(collectionName);

            var listWrites = usersTable.Rows
                                  .Select(p =>
                                            new InsertOneModel<User>(new User(p)))
                                  .ToList();

            var resultWrites = commentsCollection.BulkWriteAsync(listWrites).Result;

            ConsolePrinter.PrintLine($"OK?: {resultWrites.IsAcknowledged} - Inserted Count: {resultWrites.InsertedCount}", ConsoleColor.Green);
        }

        public void FillPostsTable(Posts postsTable)
        {
            string collectionName = nameof(Posts).ToLower();

            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DataBase);

            CreateCollectionOptions options = new CreateCollectionOptions();
            options.Capped = false;

            db.CreateCollection(collectionName, options);

            var postsCollection = db.GetCollection<Post>(collectionName);

            var listWrites = postsTable.Rows
                                  .Select(p => new InsertOneModel<Post>(new Post(p)))
                                  .ToList();

            var resultWrites = postsCollection.BulkWriteAsync(listWrites).Result;

            ConsolePrinter.PrintLine($"OK?: {resultWrites.IsAcknowledged} - Inserted Count: {resultWrites.InsertedCount}", ConsoleColor.Green);
        }

        public void FillBadgesTable(Badges badgesTable)
        {
            string collectionName = nameof(Badges).ToLower();

            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DataBase);

            CreateCollectionOptions options = new CreateCollectionOptions();
            options.Capped = false;

            db.CreateCollection(collectionName, options);

            var badgesCollection = db.GetCollection<Badge>(collectionName);

            var listWrites = badgesTable.Rows
                                  .Select(p => new InsertOneModel<Badge>(new Badge(p)))
                                  .ToList();

            var resultWrites = badgesCollection.BulkWriteAsync(listWrites).Result;

            ConsolePrinter.PrintLine($"OK?: {resultWrites.IsAcknowledged} - Inserted Count: {resultWrites.InsertedCount}", ConsoleColor.Green);
        }

        public void FillUsersBadgesTable(UsersBadges usersBadgesTable)
        {
            string collectionName = nameof(UsersBadges).ToLower();

            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DataBase);

            CreateCollectionOptions options = new CreateCollectionOptions();
            options.Capped = false;

            db.CreateCollection(collectionName, options);

            var usersBadgesCollection = db.GetCollection<UserBadge>(collectionName);

            var listWrites = usersBadgesTable.Rows
                                  .Select(p => new InsertOneModel<UserBadge>(new UserBadge(p)))
                                  .ToList();

            var resultWrites = usersBadgesCollection.BulkWriteAsync(listWrites).Result;

            ConsolePrinter.PrintLine($"OK?: {resultWrites.IsAcknowledged} - Inserted Count: {resultWrites.InsertedCount}", ConsoleColor.Green);
        }

        public void FillTagsTable(Tags tagsTable)
        {
            string collectionName = nameof(Tags).ToLower();

            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DataBase);

            CreateCollectionOptions options = new CreateCollectionOptions();
            options.Capped = false;

            db.CreateCollection(collectionName, options);

            var tagsCollection = db.GetCollection<Tag>(collectionName);

            var listWrites = tagsTable.Rows
                                  .Select(p => new InsertOneModel<Tag>(new Tag(p)))
                                  .ToList();

            var resultWrites = tagsCollection.BulkWriteAsync(listWrites).Result;

            ConsolePrinter.PrintLine($"OK?: {resultWrites.IsAcknowledged} - Inserted Count: {resultWrites.InsertedCount}", ConsoleColor.Green);
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
                websiteUrl = row.WebsiteUrl;
                location = row.Location;
                aboutMe = row.AboutMe;
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
