using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _300961820_SiChen_Lab2
{
    public class DDBOperations
    {
        public AmazonDynamoDBClient client;
        BasicAWSCredentials credentials;
        string tableName = "Bookshelf";
        DynamoDBContext context;

        public DDBOperations()
        {
            credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["accessId"], ConfigurationManager.AppSettings["secretKey"]);
            client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);
            context = new DynamoDBContext(client);
        }

        public void CreateTable()
        {
            CreateTableRequest request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName="UserId",
                        AttributeType="N"
                    },
                     new AttributeDefinition
                    {
                        AttributeName="UserName",
                        AttributeType="S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName="UserId",
                        KeyType="HASH"
                    },
                    new KeySchemaElement
                    {
                        AttributeName="UserName",
                        KeyType="RANGE"
                    }
                },
                BillingMode = BillingMode.PROVISIONED,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 1
                }

            };
            var response = client.CreateTable(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Table created successfully");
            }
        }

        public void InsertItem()
        {

            Book book1 = new Book
            {
                Title = "Battle Royale",
                ISBN = "928-92-9505-02-5",
                Author = "Xuehd Shane",
                Pages = 612
            };
            Snapshot snapshot1 = new Snapshot
            {
                LastTimeOpen = new DateTime(2019, 10, 2),
                LastPageCount = 456
            };
            book1.Snapshots = snapshot1;

            Book book2 = new Book
            {
                Title = "One Piece",
                ISBN = "978-0-7528-6574-4",
                Author = "Chane Ohine",
                Pages = 2934
            };
            Snapshot snapshot2 = new Snapshot
            {
                LastTimeOpen = new DateTime(2019, 10, 1),
                LastPageCount = 577
            };
            book2.Snapshots = snapshot2;

            List<Book> books = new List<Book>();
            books.Add(book1);
            books.Add(book2);

            User user = new User
            {
                UserId = 563243125,
                UserName = "Tiaris Xu",
                Email = "tiarialove@gmail.com",
                Books = books
            };
            context.Save(user);
        }

        public void AddUser(int id, string username, string email)
        {
            User user = new User
            {
                UserId = id,
                UserName = username,
                Email = email,
            };
            context.Save(user);
        }

        public void AddBook(User user, string isbn, string title, 
            string author, int page, DateTime lastOpen, int lastPage)
        {
            Book book = new Book
            {
                Title = title,
                ISBN = isbn,
                Author = author,
                Pages = page
            };
            Snapshot snapshot = new Snapshot
            {
                LastTimeOpen = lastOpen,
                LastPageCount = lastPage
            };
            book.Snapshots = snapshot;
            user.Books.Add(book);
            context.Save(user);
        }

        public void DeleteUser(int userId, string username)
        {
            DeleteItemRequest request = new DeleteItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "UserId", new AttributeValue{N=userId.ToString()} },
                    { "UserName", new AttributeValue{S=username} }
                }
            };

            var response = client.DeleteItem(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Item deleted successfully");
            }
        }

        public void DeleteBook(User user, Book book)
        {
            user.Books.Remove(book);
        }

        public void DeleteTable()
        {
            var response = client.DeleteTable(tableName);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Table has been deleted!");
                Console.WriteLine($"Table status: {response.TableDescription.TableStatus.Value}");
            }

        }

        public void BackupTable()
        {
            CreateBackupRequest request = new CreateBackupRequest
            {
                BackupName = "BK001",
                TableName = tableName
            };

            var response = client.CreateBackup(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Backup created successfully");
                Console.WriteLine($"Backup BackupArn: {response.BackupDetails.BackupArn}");
                Console.WriteLine($"Backup BackupCreationDateTime: {response.BackupDetails.BackupCreationDateTime}");
                Console.WriteLine($"Backup BackupStatus: {response.BackupDetails.BackupStatus}");
                Console.WriteLine($"Backup BackupSizeBytes: {response.BackupDetails.BackupSizeBytes}");

            }
        }
    }
}
