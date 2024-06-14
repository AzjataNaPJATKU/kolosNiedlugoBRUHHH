using POPPOPOP.DTO;

namespace POPPOPOP.Repositories;

public interface IDbrepository
{
    Task<bool> DoesProductExist(int idProduct);
    Task<bool> DoesWarehouseExist(int idWarehouse);
    Task<bool> DoesOrderExist(int idProduct, int amount);
    Task<GetdataDto> GetData(int idProduct, int amount);
    Task<bool> DoesOrderExistInProWare(int idOrder);
    Task<int> GetNewId();
    Task UpdateFufilledAt(int idOrder);
    Task<int> InsertIntoProdWare(AddProductToWarehouseDto aptw,GetdataDto gt);
    Task ex2(int id);
}