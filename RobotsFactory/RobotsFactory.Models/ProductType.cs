namespace RobotsFactory.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class ProductType
    {
        [Key]
        public int ProductTypeId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}