using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Core.Application.Pipelines.Security
{
    public class DecryptedJsonModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (typeof(IDecryptedModel).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new DecryptedJsonModelBinder();
            }

            return null!;
        }
    }
}
