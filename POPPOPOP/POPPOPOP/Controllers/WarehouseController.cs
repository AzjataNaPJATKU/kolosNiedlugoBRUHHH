using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using POPPOPOP.DTO;
using POPPOPOP.Repositories;

namespace POPPOPOP.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IDbrepository _warehouseRepository;
    public WarehouseController(IDbrepository animalsRepository)
    {
        _warehouseRepository = animalsRepository;
    }

    [HttpPost("/post1")]
    public async Task<IActionResult> Post(AddProductToWarehouseDto addProductToWarehouseDto)
    {
        if (!await _warehouseRepository.DoesProductExist(addProductToWarehouseDto.IdProduct)
            && !await _warehouseRepository.DoesWarehouseExist(addProductToWarehouseDto.IdWarehouse))
        {
            return NotFound();
        }
        
        if (!await _warehouseRepository.DoesOrderExist(addProductToWarehouseDto.IdProduct,addProductToWarehouseDto.Amount))
        {
            return NotFound();
        }
        
        var newdto =
                    await _warehouseRepository.GetData(addProductToWarehouseDto.IdProduct, addProductToWarehouseDto.Amount);

        if (!await _warehouseRepository.DoesOrderExistInProWare(newdto.IdOrder))
        {
            return NotFound();
        }
        try
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _warehouseRepository.UpdateFufilledAt(newdto.IdOrder);
                var res = await _warehouseRepository.InsertIntoProdWare(addProductToWarehouseDto, newdto);
                scope.Complete();

                return Ok(res);
            }
        }
        catch (TransactionAbortedException ex)
        {
            Console.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            throw;
        }
    }
    [HttpPost("/post2")]
    public async Task<IActionResult> postEx2(int id)
    {
        await _warehouseRepository.ex2(id);
        return Ok();
    }
}