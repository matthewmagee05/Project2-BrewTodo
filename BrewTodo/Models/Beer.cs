using System.ComponentModel.DataAnnotations;

namespace brewtodo.Models
{
    public class Beer
    {
        public int BeerID { get; set; }
        [Required]
        [MaxLength(20)]
        public string BeerName { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public int BeerTypeID { get; set; } 
        public int BreweryID { get; set; }

        public virtual BeerType BeerType { get; set; }
        public virtual Brewery Brewery { get; set; }
    }
}