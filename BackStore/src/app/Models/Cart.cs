namespace MyApi.Models 
{
    public class Cart
    {
   public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
    }
}