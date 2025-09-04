// using RMS.Data.Entities;
// using System.ComponentModel.DataAnnotations;

// namespace RMS.Web.Models // or RMS.Web.Models — use the correct folder
// {
//     public class CheckoutViewModel
//     {
//         public DeliveryAddress DeliveryAddress { get; set; } = new DeliveryAddress();
//         public PaymentMethod PaymentMethod { get; set; } = new PaymentMethod();
//     }
// }

using System.ComponentModel.DataAnnotations;

namespace RMS.Web.Models
{
    public class CheckoutViewModel
    {
        public DeliveryAddressViewModel DeliveryAddress { get; set; } = new DeliveryAddressViewModel();
        public PaymentMethodViewModel PaymentMethod { get; set; } = new PaymentMethodViewModel();

        public decimal CartTotalAmount { get; set; } 

         public string PayPalOrderId { get; set; }
    }

    
}

