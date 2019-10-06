using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _300961820_SiChen_Lab2
{
    [DynamoDBTable("Bookshelf")]
    public class User
    {
        [DynamoDBHashKey("UserId")] 
        public int UserId
        {
            get; set;
        }
        [DynamoDBProperty]
        public string UserName
        {
            get; set;
        }
        [DynamoDBProperty]
        public string Email
        {
            get; set;
        }
        [DynamoDBProperty("Books")]
        public List<Book> Books
        {
            get; set;
        }
        public User()
        {
            Books = new List<Book>();
        }
    }
}
