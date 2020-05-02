using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using vroom.Extensions;

namespace vroom.Models
{
    public class Bike
    {
        public int Id { get; set; }

        public Make Make { get; set; }
        [RegularExpression("^[1-9]*$" , ErrorMessage ="you must select make")]
        public int MakeID { get; set; }


        public Model Model { get; set; }
        [RegularExpression("^[1-9]*$", ErrorMessage = "you must select model")]
        public int ModelID { get; set; }

        [YaerRangeTillDate(1900 , ErrorMessage ="Invaild Year")]
        public int Year { get; set; }


        [Required(ErrorMessage ="must enter mailage")]
        [Range(1, int.MaxValue,ErrorMessage ="must mailage more than 0")]
        public int Mileage { get; set; }

        public string Features { get; set; }


        [Required(ErrorMessage = "must enter seller name")]
        public string SellerName { get; set; }

        [EmailAddress(ErrorMessage ="invaild email")]
        public string SellerEmail { get; set; }


        [Required(ErrorMessage = "must enter seller phone")]
        public string SellerPhone { get; set; }
      

        [Required(ErrorMessage = "must enter price")]
        public int Price { get; set; }


        [Required]
        [RegularExpression("^[A-Za-z]*$", ErrorMessage = "you must select currency")]
        public string Currency { get; set; }


        [DisplayName("Image")]
        public string ImagePath { get; set; }

    }
}
