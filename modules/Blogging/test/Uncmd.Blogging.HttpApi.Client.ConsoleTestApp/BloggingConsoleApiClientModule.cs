using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Uncmd.Blogging
{
    [DependsOn(
        typeof(BloggingHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class BloggingConsoleApiClientModule : AbpModule
    {
        
    }
}
