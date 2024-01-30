using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Core.Application.Pipelines.Security
{
    public class DecryptedJsonModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var httpContext = bindingContext.HttpContext;
            var decryptedJson = httpContext.Items["DecryptedRequestBody"] as string;

            if (!string.IsNullOrEmpty(decryptedJson))
            {
                try
                {
                    var model = JsonConvert.DeserializeObject(decryptedJson, bindingContext.ModelType);
                    bindingContext.Result = ModelBindingResult.Success(model);
                }
                catch (JsonException)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }

            return Task.CompletedTask;
        }
    }


}
