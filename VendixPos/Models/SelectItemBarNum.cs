namespace VendixPos.Models
{
    public class SelectItemBarNum
    {

      
            public int ItemID { get; set; }
            public string ItemName { get; set; }
            public int ItemQuantity { get; set; }
            public double UnitPrice { get; set; }
            public int UnitQuantity { get; set; }
            public string SecondUnit { get; set; }
            public double LowPrice { get; set; }
            public bool ItemNoQuan { get; set; }
            public bool Checked { get; set; }
            public double avgitem { get; set; }
            public float InvoiceItemPrice { get; set; }
            public int lowestItemQun { get; set; }



    }
}
