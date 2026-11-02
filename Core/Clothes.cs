namespace Core;

public class Clothes
{
    public int ItemID { get; set; }
   
    public string Type { get; set; } = "";
    
    public string Description { get; set; } = "";
   
    public string Size { get; set; } = "";
   
    public string Color { get; set; } = "";
    
    // Samler tøjets data til én tekst, fx "kjole, str. M, rød"
    public string GetDescription()
    {
        return $"{Type}, str. {Size}, {Color}";
    }
    
}