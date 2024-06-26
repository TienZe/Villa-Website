using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VillaWeb.Infrastructures;
public static class ModelStateExtention
{
    public static void AddModelErrors(this ModelStateDictionary modelState, IEnumerable<string> errorMessages)
    {
        foreach (var errorMessage in errorMessages) {
            modelState.AddModelError(string.Empty, errorMessage);
        }
    }

    public static void AddErrors(this ModelStateDictionary modelState, Dictionary<string, List<string>> validationErrors)
    {
        foreach (var entry in validationErrors) {
            foreach (var error in entry.Value) {
                modelState.AddModelError(entry.Key, error);
            }
        }
    }
}
