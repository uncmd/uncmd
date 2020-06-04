using Uncmd.Blogging.Localization;
using Volo.Abp.Application.Services;

namespace Uncmd.Blogging
{
    public abstract class BloggingAppService : ApplicationService
    {
        protected BloggingAppService()
        {
            LocalizationResource = typeof(BloggingResource);
            ObjectMapperContext = typeof(BloggingApplicationModule);
        }
    }
}
