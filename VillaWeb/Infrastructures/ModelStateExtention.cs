using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VillaWeb.Infrastructures;
public static class ModelStateExtention
{
    public static void AddModelErrors(this ModelStateDictionary modelState, IEnumerable<string> errorMessages)
    {
        foreach (var errorMessage in errorMessages)
        {
            modelState.AddModelError(string.Empty, errorMessage);
        }
    }
}
