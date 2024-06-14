namespace POPPOPOP.Repositories;

public interface IDbrepository
{
    Task<bool> DoesProductExist(int idProduct);
    Task<bool> DoesWarehouseExist(int idWarehouse);
    
}