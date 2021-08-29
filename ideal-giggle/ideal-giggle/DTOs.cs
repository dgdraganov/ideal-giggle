using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ideal_giggle
{
    [XmlRoot("badges")]
    public class Badges
    {
        [XmlElement("row")]
        public List<Row> Rows { get; set; }

        public class Row
        {
            [XmlAttribute("Id")]
            public int Id { get; set; }

            [XmlAttribute("Name")]
            public string Name { get; set; }
        }
    }

    [XmlRoot("badges")]
    public class UsersBadges
    {
        [XmlElement("row")]
        public List<Row> Rows { get; set; }

        public class Row
        {
            [XmlAttribute("Id")]
            public int Id { get; set; }

            [XmlAttribute("UserId")]
            public int UserId { get; set; }

            [XmlAttribute("BadgeId")]
            public int BadgeId { get; set; }

            [XmlAttribute("Date")]
            public DateTime Date { get; set; }
        }
    }


    [XmlRoot("posts")]
    public class Posts
    {
        [XmlElement("row")]
        public List<Row> Rows { get; set; }

        public class Row
        {
            [XmlAttribute("Id")]
            public int Id { get; set; }

            [XmlAttribute("PostTypeId")]
            public int PostTypeId { get; set; }

            [XmlAttribute("AcceptedAnswerId")]
            public int AcceptedAnswerId { get; set; }

            [XmlAttribute("Score")]
            public int Score { get; set; }

            [XmlAttribute("ViewCount")]
            public int ViewCount { get; set; }

            [XmlAttribute("Body")]
            public string Body { get; set; }

            [XmlAttribute("OwnerUserId")]
            public int OwnerUserId { get; set; }

            [XmlAttribute("LastEditorUserId")]
            public int LastEditorUserId { get; set; }

            [XmlAttribute("Title")]
            public string Title { get; set; }

            [XmlAttribute("Tags")]
            public string Tags { get; set; }

            [XmlAttribute("AnswerCount")]
            public int AnswerCount { get; set; }

            [XmlAttribute("CommentCount")]
            public int CommentCount { get; set; }

            [XmlAttribute("FavoriteCount")]
            public int FavoriteCount { get; set; }

            [XmlAttribute("ContentLicense")]
            public string ContentLicense { get; set; }

            [XmlAttribute("CreationDate")]
            public DateTime CreationDate { get; set; }

            [XmlAttribute("LastActivityDate")]
            public DateTime LastActivityDate { get; set; }

            [XmlAttribute("LastEditDate")]
            public DateTime LastEditDate { get; set; }
        }
    }

    [XmlRoot("users")]
    public class Users
    {

        [XmlElement("row")]
        public List<Row> Rows { get; set; }

        public class Row
        {
            [XmlAttribute("Id")]
            public int Id { get; set; }

            [XmlAttribute("Reputation")]
            public int Reputation { get; set; }

            [XmlAttribute("CreationDate")]
            public DateTime CreationDate { get; set; }

            [XmlAttribute("DisplayName")]
            public string DisplayName { get; set; }

            [XmlAttribute("LastAccessDate")]
            public DateTime LastAccessDate { get; set; }

            [XmlAttribute("WebsiteUrl")]
            public string WebsiteUrl { get; set; }

            [XmlAttribute("Location")]
            public string Location { get; set; }

            [XmlAttribute("AboutMe")]
            public string AboutMe { get; set; }

            [XmlAttribute("Views")]
            public int Views { get; set; }

            [XmlAttribute("UpVotes")]
            public int UpVotes { get; set; }

            [XmlAttribute("DownVotes")]
            public int DownVotes { get; set; }

            [XmlAttribute("AccountId")]
            public int AccountId { get; set; }

        }
    }


    [XmlRoot("votes")]
    public class Votes
    {
        [XmlElement("row")]
        public List<Row> Rows { get; set; }

        public class Row
        {
            [XmlAttribute("Id")]
            public int Id { get; set; }

            [XmlAttribute("PostId")]
            public int PostId { get; set; }

            [XmlAttribute("VoteTypeId")]
            public int VoteTypeId { get; set; }

            [XmlAttribute("CreationDate")]
            public DateTime CreationDate { get; set; }
        }

    }

    [XmlRoot("comments")]
    public class Comments
    {
        [XmlElement("row")]
        public List<Row> Rows { get; set; }
        
        public class Row
        {
            [XmlAttribute("Id")]
            public int Id { get; set; }

            [XmlAttribute("PostId")]
            public int PostId { get; set; }

            [XmlAttribute("Score")]
            public int Score { get; set; }

            [XmlAttribute("Text")]
            public string Text { get; set; }

            [XmlAttribute("CreationDate")]
            public DateTime CreationDate { get; set; }

            [XmlAttribute("UserId")]
            public int UserId { get; set; }

            [XmlAttribute("ContentLicense")]
            public string ContentLicense { get; set; }
        }


    }
}
