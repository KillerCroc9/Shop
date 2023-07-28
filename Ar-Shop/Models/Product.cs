using System.ComponentModel.DataAnnotations.Schema;

namespace Ar_Shop.Models
{
    public class Product
    {
        // Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; } // New property for quantity

        // Navigation properties for one-to-many relationship
        public List<Picture> Pictures { get; set; }
        public List<Review> Reviews { get; set; }
        [NotMapped]
        public List<IFormFile>? PictureFiles { get; set; }


        // Constructor
        public Product()
        {
            Pictures = new List<Picture>();
            Reviews = new List<Review>();
        }
    }

    public class Picture
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool NeedToDelete { get; internal set; }
        // Add any additional properties related to pictures
    }

    public class Review
    {
        public int Id { get; set; }
        public string Content { get; set; }
        // Add any additional properties related to reviews
    }
}
