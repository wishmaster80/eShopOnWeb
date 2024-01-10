using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp;
public  class OrderInfo
{
    public int Id { get; set; }
    public string ShippingAddress { get; set; }
    public string ListOfItems { get; set; }
    public decimal Total { get; set; }
}
