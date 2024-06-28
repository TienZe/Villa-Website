using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VillaAPI.Infrastructure;

public static class ModelStateExtensions
{
    public static Dictionary<string, List<string>> ToErrorDictionary(this ModelStateDictionary modelState)
    {
        return modelState
            .Where(ms => ms.Value.Errors.Any())
            .ToDictionary(
                ms => ms.Key,
                ms => ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );
    }
}
