namespace Core;

public class ClubMember
{
    public int Id { get; set; } = Random.Shared.Next();
   
    public string Name { get; set; } = "";
   
    public string Phonenumber { get; set; } = "";
    
    public string EmailAddress { get; set; } = "";
   
    // En liste, der kan indeholde flere Clothes-objekter,
    // fordi ét klubmedlem kan eje mange stykker tøj
    // = new() opretter en tom liste fra starten,
    // så vi kan tilføje tøj uden først at skulle oprette listen
    public List<Clothes> Clothes { get; set; } = new();
}