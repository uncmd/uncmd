using Uncmd.Blogging.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Uncmd.Blogging
{
    public abstract class BloggingController : AbpController
    {
        protected BloggingController()
        {
            LocalizationResource = typeof(BloggingResource);
        }
    }
}
