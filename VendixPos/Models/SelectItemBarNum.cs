namespace VendixPos.Models
{
    public class SelectItemBarNum
    {

      
            public int ItemID { get; set; }
            public string ItemName { get; set; }
            public int ItemQuantity { get; set; }
            public float UnitPrice { get; set; }
            public int UnitQuantity { get; set; }
            public string SecondUnit { get; set; }
            public float LowPrice { get; set; }
            public bool ItemNoQuan { get; set; }
            public bool Checked { get; set; }
            public double avgitem { get; set; }
            public float InvoiceItemPrice { get; set; }
     
    }
}
