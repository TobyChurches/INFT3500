namespace WebStore.Models.ViewModels;

public class CheckoutViewModel
{
    // Customer Information
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public int Postcode { get; set; }
    public string Suburb { get; set; }
    public string State { get; set; }

    // Delivery
    public string DeliveryAddress { get; set; }
    public int DeliveryPostcode { get; set; }
    public string DeliverySuburb { get; set; }
    public string DeliveryState { get; set; }

    // Payment
    public long CardNumber { get; set; }
    public string CardOwner { get; set; }
    public DateTime Expiry { get; set; }
    public int CVV { get; set; }
}
