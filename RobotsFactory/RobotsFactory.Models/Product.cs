namespace RobotsFactory.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Dimensions { get; set; }

        public double Weight { get; set; }

        public string Description { get; set; }

        public int ReleaseYear { get; set; }

        public decimal Price { get; set; }

        [ForeignKey("ProductTypes")]
        public int ProductTypeId { get; set; }

        public virtual ProductType ProductType { get; set; }

        [ForeignKey("Manufacturers")]
        public int ManufacturerId { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }
    }
}