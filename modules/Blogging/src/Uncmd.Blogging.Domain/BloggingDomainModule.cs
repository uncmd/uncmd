using Volo.Abp.Modularity;

namespace Uncmd.Blogging
{
    [DependsOn(
        typeof(BloggingDomainSharedModule)
        )]
    public class BloggingDomainModule : AbpModule
    {

    }
}
