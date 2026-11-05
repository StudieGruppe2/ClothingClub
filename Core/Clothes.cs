namespace Core;

public class Clothes
{
    public int ItemID { get; set; }
   
    public string Type { get; set; } = "";
    
    public string Description { get; set; } = "";
   
    public string Size { get; set; } = "";
   
    public string Color { get; set; } = "";
    
    public int? BorrowedByMemberId { get; set; } // Id'et på hvem der har lånt tøjet
    
    public DateTime? BorrowedOn { get; set; } // så vi holder øje med hvornår det er lånt ud - spørgsmålstegn betyder at værdien kan være null
    
    public DateTime? DueDate { get; set; } // den dato tøjet skal afleveres tilbage fra lån 
    
    
    /*public bool IsBorrowed // Bruges til at vise til andre om tøjet er udlånt
    {
        get { return BorrowedByMemberId != null; }
    }
  */  
}