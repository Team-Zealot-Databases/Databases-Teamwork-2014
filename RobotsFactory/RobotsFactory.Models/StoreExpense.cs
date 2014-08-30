namespace RobotsFactory.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class StoreExpense
    {
        [Key]
        public int ExpenseId { get; set; }

        public DateTime ReportDate { get; set; }

        public decimal Expense { get; set; }

        [ForeignKey("Store")]
        public int StoreId { get; set; }

        public virtual Store Store { get; set; }
    }
}