using RMS.Data.Entities;
namespace RMS.Web.Models;

public class ConfirmViewModel
{
    public DeliveryAddress DeliveryAddress { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
