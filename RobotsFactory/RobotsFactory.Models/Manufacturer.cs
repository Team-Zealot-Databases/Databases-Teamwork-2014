namespace RobotsFactory.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Manufacturer
    {
        [Key]
        public int ManufacturerId { get; set; }

        [Required]
        public string Name { get; set; }

        [ForeignKey("Address")]
        public int AddressId { get; set; }

        public virtual Address Address { get; set; }
    }
}