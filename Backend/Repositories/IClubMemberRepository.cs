using Core;
namespace Backend.Repositories;


public interface IClubMemberRepository
{
    List<ClubMember> GetAll();
    ClubMember? GetById(int memberID);
    void Add(ClubMember member);
    void Remove(int memberID);               // sletter også medlemmets tøj

    void AddClothes(int memberID, Clothes item);       // NYT
    void RemoveClothes(int memberID, int itemID);      // NYT
}