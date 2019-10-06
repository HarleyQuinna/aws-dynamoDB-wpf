using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _300961820_SiChen_Lab2
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<User> users;
        private User user;
        private Book book;
        private DDBOperations dBOperations;
        public MainWindow()
        {
            InitializeComponent();
            this.dBOperations = new DDBOperations();
            //dBOperations.CreateTable();
            dBOperations.InsertItem();
            //dBOperations.DeleteItem();

            this.users = new ObservableCollection<User>();
            this.user = new User();
            dgUser.ItemsSource = null;
            dgUser.ItemsSource = users;

        }

        public void LoadTable(Object obj)
        {
            var table = Amazon.DynamoDBv2.DocumentModel.Table.LoadTable(dBOperations.client, "Bookshelf");
            var search = table.Scan(new Amazon.DynamoDBv2.DocumentModel.Expression());

            var documentList = new List<Document>();
            do
            {
                documentList.AddRange(search.GetNextSet());
            } while (!search.IsDone);

            var users = new ObservableCollection<User>();
            foreach (var doc in documentList)
            {
                var user = new User();
                user.Books = new List<Book>();
                foreach (var attribute in doc.GetAttributeNames())
                {
                    var value = doc[attribute];
                    if (attribute == "UserId")
                    {
                        user.UserId = Convert.ToInt32(value.AsPrimitive().Value);
                    }
                    else if (attribute == "UserName")
                    {
                        user.UserName = value.AsPrimitive().Value.ToString();
                    }
                    else if (attribute == "Email")
                    {
                        user.Email = value.AsPrimitive().Value.ToString();
                    }
                    else if (attribute == "Books")
                    {
                        var bookList = new List<Document>();
                        bookList = value.AsListOfDocument();
                        foreach (var b in bookList)
                        {
                            var book = new Book();
                            foreach(var attrBook in b.GetAttributeNames())
                            {
                                var valueBook = b[attrBook];
                                if(attrBook == "Title")
                                {
                                    book.Title = valueBook.AsPrimitive().Value.ToString();
                                }
                                else if (attrBook == "ISBN")
                                {
                                    book.ISBN = valueBook.AsPrimitive().Value.ToString();
                                }
                                else if (attrBook == "Pages")
                                {
                                    book.Pages = Convert.ToInt32(valueBook.AsPrimitive().Value);
                                }
                                else if (attrBook == "Author")
                                {
                                    book.Author = valueBook.AsPrimitive().Value.ToString();
                                }
                                else if (attrBook == "Snapshots")
                                {
                                    var s = new Document();
                                    s = valueBook.AsDocument();
                                    foreach (var attrSnap in s.GetAttributeNames())
                                    {
                                        var valueSnap = s[attrSnap];
                                        if (attrSnap == "LastTimeOpen")
                                        {
                                            book.Snapshots.LastTimeOpen = Convert.ToDateTime(valueSnap.AsPrimitive().Value);
                                        }
                                        else if (attrSnap == "LastPageCount")
                                        {
                                            book.Snapshots.LastPageCount = Convert.ToInt32(valueSnap.AsPrimitive().Value);
                                        }
                                    }
                                }
                            }
                            user.Books.Add(book);
                        }

                    }
                }
                users.Add(user);
            }
            this.users = users;
            dgUser.ItemsSource = null;
            dgUser.ItemsSource = users;
        }

        private void TxtLastYear_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtLastOpenYear.Text == "YYYY")
            {
                txtLastOpenYear.Text = "";
            }
        }

        private void TxtLastYear_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLastOpenYear.Text == "")
            {
                txtLastOpenYear.Text = "YYYY";
            }
        }

        private void TxtLastMonth_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtLastOpenMonth.Text == "MM")
            {
                txtLastOpenMonth.Text = "";
            }
        }

        private void TxtLastMonth_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLastOpenMonth.Text == "")
            {
                txtLastOpenMonth.Text = "MM";
            }
        }

        private void TxtLastDay_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtLastOpenDay.Text == "DD")
            {
                txtLastOpenDay.Text = "";
            }
        }

        private void TxtLastDay_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLastOpenDay.Text == "")
            {
                txtLastOpenDay.Text = "DD";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.LoadTable(null);
        }

        private void DgUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User selectedUser = dgUser.SelectedItem as User;
            ltbBooks.Items.Clear();
            txtbSnapshot.Text = "";
            txtbBook.Text = "";
            if (selectedUser != null)
            {
                lblUser.Content = $"{selectedUser.UserName}'s Bookshelf";
                this.user = selectedUser;
                DateTime temp = new DateTime(1,1,1);
                foreach (Book book in selectedUser.Books)
                { 
                    ListBoxItem item = new ListBoxItem();
                    item.Content = book.Title;
                    ltbBooks.Items.Add(item);
                    int value = DateTime.Compare(temp, book.Snapshots.LastTimeOpen);
                    if (value < 0)
                    {
                        temp = book.Snapshots.LastTimeOpen;
                    }
                }
                foreach (Book book in selectedUser.Books)
                {
                    if(book.Snapshots.LastTimeOpen == temp)
                    {
                        txtbSnapshot.Text = book.Title + ":\nLast Time Open: " + book.Snapshots.LastTimeOpen.ToString() + "\nLast Time Page Count: " + book.Snapshots.LastPageCount;
                        break;
                    }
                }
            }

                }

        private void LtbBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selection = ltbBooks.SelectedItem as ListBoxItem;
            if (selection != null)
            {
                foreach (Book book in user.Books)
                {
                    if (book.Title.Equals(selection.Content))
                    {
                        txtbBook.Text = book.ToString();
                        this.book = book;
                        break;
                    }
                }
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            dBOperations.AddUser(Convert.ToInt32(txtUserId.Text), txtUsernaem.Text, txtEmail.Text);
            this.LoadTable(null);
            txtUserId.Text = "";
            txtUsernaem.Text = "";
            txtEmail.Text = "";
        }

        private void BtnEditBook_Click(object sender, RoutedEventArgs e)
        {
            dBOperations.AddBook(user,
                txtISBN.Text, txtBookTitle.Text, txtAuthor.Text, Convert.ToInt32(txtPages.Text),
                new DateTime(Convert.ToInt32(txtLastOpenYear.Text), Convert.ToInt32(txtLastOpenMonth.Text),
                Convert.ToInt32(txtLastOpenDay.Text)), Convert.ToInt32(txtLastPage.Text));
            ltbBooks.Items.Clear();
            DateTime temp = new DateTime(1, 1, 1);
            foreach (Book book in user.Books)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = book.Title;
                ltbBooks.Items.Add(item);
                int value = DateTime.Compare(temp, book.Snapshots.LastTimeOpen);
                if (value < 0)
                {
                    temp = book.Snapshots.LastTimeOpen;
                }
            }
            foreach (Book book in user.Books)
            {
                if (book.Snapshots.LastTimeOpen == temp)
                {
                    txtbSnapshot.Text = book.Title + ":\nLast Time Open: " + book.Snapshots.LastTimeOpen.ToString() + "\nLast Time Page Count: " + book.Snapshots.LastPageCount;
                    break;
                }
            }
            txtAuthor.Text = "";
            txtBookTitle.Text = "";
            txtISBN.Text = "";
            txtLastOpenDay.Text = "";
            txtLastOpenMonth.Text = "";
            txtLastOpenYear.Text = "";
            txtPages.Text = "";
            txtLastPage.Text = "";

        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            dBOperations.DeleteUser(user.UserId, user.UserName);
            this.LoadTable(null);
        }

        private void BtnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            dBOperations.DeleteBook(user, this.book);
            ltbBooks.Items.Clear();
            DateTime temp = new DateTime(1, 1, 1);
            foreach (Book book in user.Books)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = book.Title;
                ltbBooks.Items.Add(item);
                int value = DateTime.Compare(temp, book.Snapshots.LastTimeOpen);
                if (value < 0)
                {
                    temp = book.Snapshots.LastTimeOpen;
                }
            }
            foreach (Book book in user.Books)
            {
                if (book.Snapshots.LastTimeOpen == temp)
                {
                    txtbSnapshot.Text = book.Title + ":\nLast Time Open: " + book.Snapshots.LastTimeOpen.ToString() + "\nLast Time Page Count: " + book.Snapshots.LastPageCount;
                    break;
                }
            }
        }
    }
}
