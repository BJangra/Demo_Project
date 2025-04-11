using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace FirstProject_ECommerce.Models
{
    public class Order
    {
        public  int  Id { get; set; }
        public  String  UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public  decimal  TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public  List<OrderItem>  OrderItems { get; set; }
    }
  
}
 