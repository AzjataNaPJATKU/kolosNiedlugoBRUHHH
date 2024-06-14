using System.Data.SqlClient;
using POPPOPOP.DTO;

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

    public async Task<bool> DoesOrderExist(int idProduct, int amount)
    {
        var query = "SELECT 1 FROM Order WHERE idProduct = @ID and Amount = @AMOUNT and createdAt < current_date and fufilledAt is null";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", idProduct);
        command.Parameters.AddWithValue("@AMOUNT", amount);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<GetdataDto> GetData(int idProduct, int amount)
    {
        var query = "SELECT 1 FROM Order JOIN Product on Order.idProduct = Product.idProduct WHERE idProduct = @ID and Amount = @AMOUNT";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", idProduct);
        command.Parameters.AddWithValue("@AMOUNT", amount);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        
        await reader.ReadAsync();

        if (!reader.HasRows) throw new Exception();

        var idOrderOrdinal = reader.GetOrdinal("IdOrder");
        var PriceOrdinal = reader.GetOrdinal("Price");

        var newdto = new GetdataDto()
        {
            IdOrder = reader.GetInt32(idOrderOrdinal),
            Price = reader.GetInt32(PriceOrdinal)
        };
        
        return newdto;
    }

    public async Task<bool> DoesOrderExistInProWare(int idOrder)
    {
        var query = "SELECT 1 FROM Product_Warehouse WHERE idorder = @ID";
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", idOrder);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<int> GetNewId()
    {
        var query = "Select Max(IdProductWarehouse) from ProductWarehouse";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return (int)res!;
    }

    public async Task UpdateFufilledAt(int idOrder)
    {
        var query = "Update Order set fufilledAt = current_date where idOrder = @ID";
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", idOrder);

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }
    public async Task<int> InsertIntoProdWare(AddProductToWarehouseDto aptw,GetdataDto gt)
    {
        
        var query = "insert into Product_Warehouse values (@id,@idwarehouse,@idProduct,@idOrder,@amount,@price,current_date);";
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        var newId = await GetNewId();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", newId);
        command.Parameters.AddWithValue("@idwarehouse", aptw.IdWarehouse);
        command.Parameters.AddWithValue("@idProduct", aptw.IdProduct);
        command.Parameters.AddWithValue("@idOrder", gt.IdOrder);
        command.Parameters.AddWithValue("@amount", aptw.Amount * gt.Price);
        command.Parameters.AddWithValue("@price", gt.Price);

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();

        return newId;
    }

    public async Task ex2(int idOrder)
    {
        var query = "CREATE PROCEDURE AddProductToWarehouse @IdProduct INT, @IdWarehouse INT, @Amount INT, @CreatedAt DATETIME AS" +
                    "  BEGIN DECLARE @IdProductFromDb INT, @IdOrder INT, @Price DECIMAL(5,2);" +
                    " SELECT TOP 1 @IdOrder = o.IdOrder  FROM Order o  LEFT JOIN Product_Warehouse pw ON o.IdOrder=pw.IdOrder" +
                    " WHERE o.IdProduct=@IdProduct AND o.Amount=@Amount AND pw.IdProductWarehouse IS NULL AND " +
                    " o.CreatedAt<@CreatedAt; SELECT @IdProductFromDb=Product.IdProduct, @Price=Product.Price FROM Product WHERE IdProduct=@IdProduct" +
                    " IF @IdProductFromDb IS NULL  BEGIN   RAISERROR('Invalid parameter: Provided IdProduct does not exist'," +
                    " 18, 0);  RETURN; END; IF @IdOrder IS NULL BEGIN RAISERROR('Invalid parameter: There is no order to fullfill', 18, 0);" +
                    " RETURN; END; IF NOT EXISTS(SELECT 1 FROM Warehouse WHERE IdWarehouse=@IdWarehouse) BEGIN RAISERROR('Invalid parameter: " +
                    "Provided IdWarehouse does not exist', 18, 0); RETURN; END; SET XACT_ABORT ON; BEGIN TRAN; UPDATE Order SET" +
                    " FulfilledAt=@CreatedAt WHERE IdOrder=@IdOrder; INSERT INTO Product_Warehouse(IdWarehouse, IdProduct," +
                    " IdOrder, Amount, Price, CreatedAt) VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount*@Price, @CreatedAt);" +
                    " SELECT @@IDENTITY AS NewId; COMMIT; END";
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", idOrder);

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }
}