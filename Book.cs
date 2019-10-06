using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _300961820_SiChen_Lab2
{
    public class Book
    {
        public string Title
        {
            get; set;
        }
        public string ISBN
        {
            get; set;
        }
        public int Pages
        {
            get; set;
        }
        public string Author
        {
            get; set;
        }
        public Snapshot Snapshots
        {
            get; set;
        }

        public Book()
        {
            Snapshots = new Snapshot();
        }


        public override string ToString()
        {
            return "Title: " + Title
                + "\n\nISBN: " + ISBN
                + "\n\nAuthor: " + Author
                + "\n\nPages: " + Pages
                + "\n\nSnapshot: \n\tLast Time Open: " + Snapshots.LastTimeOpen
                +"\n\tLast Page Count: " + Snapshots.LastPageCount
                ;
        }
    }
}
