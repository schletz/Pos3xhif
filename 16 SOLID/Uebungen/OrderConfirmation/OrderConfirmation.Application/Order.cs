namespace OrderConfirmation.Application;

/// <summary>
/// The data of an order: who ordered, the total amount and whether it is paid.
/// </summary>
public class Order
{
    public Order(string customerEmail, decimal totalAmount)
    {
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
    }

    public string CustomerEmail { get; }
    public decimal TotalAmount { get; }
    public bool IsPaid { get; private set; }

    public void MarkAsPaid() => IsPaid = true;
}
