using Core;

namespace Backend.Repositories;

public interface IClubMemberRepository
{
    List<ClubMember> GetAll();
   
    ClubMember? GetById(int memberID);
   
    void Add(ClubMember member);
   
    void Remove(int memberID);               // sletter også medlemmets tøj

    void AddClothes(int memberID, Clothes item);
   
    void RemoveClothes(int memberID, int itemID);

    List<Clothes> GetAllClothes();           // alt tøj på tværs af medlemmer
    
    Clothes? GetClothesById(int itemID);     // ét stykke tøj ud fra id
    
    bool BorrowClothes(int itemID, int borrowedByMemberId, DateTime borrowedOn, DateTime dueDate);

    bool ReturnClothes(int itemID, int borrowedByMemberId);
}