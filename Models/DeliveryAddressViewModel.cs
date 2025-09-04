// // RMS.Web/Models/DeliveryAddressViewModel.cs
// using System.ComponentModel.DataAnnotations;

// namespace RMS.Web.Models
// {
//     public class DeliveryAddressViewModel
//     {
//         [Required]
//         [Display(Name = "Full Name")]
//         public string FullName { get; set; }

//         [Required]
//         [Display(Name = "Address Line 1")]
//         public string AddressLine1 { get; set; }

//         [Display(Name = "Address Line 2")]
//         public string AddressLine2 { get; set; }

//         [Required]
//         public string City { get; set; }

//         [Required]
//         [Display(Name = "Postal Code")]
//         public string PostalCode { get; set; }

//         [Required]
//         public string Country { get; set; }

//         [Required]
//         [Display(Name = "Phone Number")]
//         public string PhoneNumber { get; set; }

//          public string Street { get; set; }
//     }
// }


using System.ComponentModel.DataAnnotations;

namespace RMS.Web.Models
{
    public class DeliveryAddressViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Street")]
        public string Street { get; set; }
    }
}

