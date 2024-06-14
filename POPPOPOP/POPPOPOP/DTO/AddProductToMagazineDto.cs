using System.ComponentModel.DataAnnotations;

namespace POPPOPOP.DTO;

public class AddProductToMagazineDto
{
    [Required]
    public int IdProduct { set; get; }
    [Required]
    public int IdWarehouse { set; get; }
    [Required]
    public int Amount { set; get; }
    [Required]
    public string CreatedAt { set; get; }
}