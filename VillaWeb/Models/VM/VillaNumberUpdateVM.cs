using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using VillaWeb.Models.Dto;

namespace VillaWeb.Models.VM;
public class VillaNumberUpdateVM
{
    public VillaNumberUpdateDTO VillaNumber { get; set; } = new();

    [ValidateNever]
    public IEnumerable<SelectListItem> SelectListOfVillas { get; set; } = new List<SelectListItem>();
}
