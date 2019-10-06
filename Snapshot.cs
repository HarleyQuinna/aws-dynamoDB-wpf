using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _300961820_SiChen_Lab2
{
    public class Snapshot
    {
        public Snapshot()
        {
            LastTimeOpen = new DateTime();
            LastPageCount = 1;
        }

        public Snapshot(DateTime lastOpen, int lastPage)
        {
            LastPageCount = lastPage;
            LastTimeOpen = lastOpen;
        }
        public DateTime LastTimeOpen
        {
            get; set;
        }

        public int LastPageCount
        {
            get; set;
        }
    }
}
