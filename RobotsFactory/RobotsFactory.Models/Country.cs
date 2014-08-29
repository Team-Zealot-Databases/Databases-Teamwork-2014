namespace RobotsFactory.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        [StringLength(20)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
    }
}