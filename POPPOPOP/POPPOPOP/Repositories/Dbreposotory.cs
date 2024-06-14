using System.Data.SqlClient;

namespace POPPOPOP.Repositories;

public class Dbreposotory : IDbrepository
{
    private readonly IConfiguration _configuration;
    public Dbreposotory(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesProductExist(int idProduct)
    {
        var query = "SELECT 1 FROM Product WHERE idProduct = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", idProduct);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;

    }

    public async Task<bool> DoesWarehouseExist(int idWarehouse)
    {
        var query = "SELECT 1 FROM Warehouse WHERE idWarehouse = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", idWarehouse);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
}