using System;
using System.Collections.Generic;

namespace ShaTask.Models
{
    public partial class Cashier
    {
        public Cashier()
        {
            InvoiceHeaders = new HashSet<InvoiceHeader>();
        }

        public int Id { get; set; }
        public string CashierName { get; set; } = null!;
        public int BranchId { get; set; }

        public virtual Branch? Branch { get; set; } = null!;
        public virtual ICollection<InvoiceHeader> InvoiceHeaders { get; set; }
    }
}
