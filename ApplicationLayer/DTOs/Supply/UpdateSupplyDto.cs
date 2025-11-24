using System.ComponentModel.DataAnnotations;

namespace ApplicationLayer.DTOs.Supply;

public class UpdateSupplyDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be non-negative")]
    public int Stock { get; set; }
}

