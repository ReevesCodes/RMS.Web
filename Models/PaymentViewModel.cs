// RMS.Web/Models/PaymentMethodViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace RMS.Web.Models
{
    public class PaymentMethodViewModel
    {
        [Required]
        [Display(Name = "Card Holder")]
        public string CardHolderName { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

 //        [Required]
        [Display(Name = "Expiration Date")]
       public string ExpirationDate { get; set; }  // Or DateTime if you prefer

        [Required]
        public string CVV { get; set; }
}
    }

