using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy_DB_C_
{
    public class Task 
    {
        public int TaskID { get; set; }
        public int EmployeeID { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; } 
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now; 

    }
}
