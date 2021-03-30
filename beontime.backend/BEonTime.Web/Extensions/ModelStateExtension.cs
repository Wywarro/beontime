using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BEonTime.Web.Extensions
{
    public static class ModelStateExtension
    {
        public static ModelStateDictionary AddErrorsToModelState(this ModelStateDictionary modelState, IdentityResult identityResults)
        {
            foreach (var e in identityResults.Errors)
            {
                modelState.TryAddModelError(e.Code, e.Description);
            }
            return modelState;
        }

        public static ModelStateDictionary AddErrorToModelState(this ModelStateDictionary modelState, string code, string description)
        {
            modelState.TryAddModelError(code, description);
            return modelState;
        }
    }
}
