using Volo.Abp.Modularity;

namespace Uncmd.Blogging
{
    [DependsOn(
        typeof(BloggingApplicationModule),
        typeof(BloggingDomainTestModule)
        )]
    public class BloggingApplicationTestModule : AbpModule
    {

    }
}
