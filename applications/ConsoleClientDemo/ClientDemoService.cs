using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Identity;
using Volo.Abp.IdentityModel;
using Volo.Abp.TenantManagement;

namespace ConsoleClientDemo
{
    public class ClientDemoService : ITransientDependency
    {
        private readonly IIdentityUserAppService _userAppService;
        private readonly ITenantAppService _tenantAppService;
        private readonly IIdentityModelAuthenticationService _authenticator;
        private readonly AbpRemoteServiceOptions _remoteServiceOptions;

        public ClientDemoService(
            IIdentityUserAppService userAppService,
            IIdentityModelAuthenticationService authenticator,
            IOptions<AbpRemoteServiceOptions> remoteServiceOptions,
            ITenantAppService tenantAppService)
        {
            _userAppService = userAppService;
            _authenticator = authenticator;
            _tenantAppService = tenantAppService;
            _remoteServiceOptions = remoteServiceOptions.Value;
        }

        public async Task RunAsync()
        {
            await TestWithHttpClient();
            await TestIdentityService();
            await TestTenantManagementService();
        }

        /// <summary>
        /// 演示如何手动创建HttpClient并使用IIdentityModelAuthenticationService服务。
        /// </summary>
        private async Task TestWithHttpClient()
        {
            Console.WriteLine();
            Console.WriteLine("*** TestWithHttpClient ************************************");

            try
            {
                using (var client = new HttpClient())
                {
                    await _authenticator.TryAuthenticateAsync(client);

                    var url = GetServerUrl() + "Test/Index";

                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 演示如何使用应用程序服务接口(本例中的IIdentityUserAppService)来调用远程服务，动态http客户端代理系统可以使用远程服务。
        /// <para>不需要使用IIdentityModelAuthenticationService，因为动态http客户端代理系统在内部使用它。您只需注入一个服务(IIdentityUserAppService)并像调用本地方法一样调用一个方法(GetListAsync)。</para>
        /// </summary>
        private async Task TestIdentityService()
        {
            Console.WriteLine();
            Console.WriteLine("*** TestIdentityService ************************************");

            try
            {
                var output = await _userAppService.GetListAsync(new GetIdentityUsersInput());

                Console.WriteLine("Total user count: " + output.TotalCount);

                foreach (var user in output.Items)
                {
                    Console.WriteLine($"- UserName={user.UserName}, Email={user.Email}, Name={user.Name}, Surname={user.Surname}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 演示如何使用应用程序服务接口(ITenantAppService)来调用远程服务，动态http客户端代理系统可以使用远程服务。
        /// <para>不需要使用IIdentityModelAuthenticationService，因为动态http客户端代理系统在内部使用它。您只需注入一个服务(ITenantAppService)并像调用本地方法一样调用一个方法(GetListAsync)。</para>
        /// </summary>
        private async Task TestTenantManagementService()
        {
            Console.WriteLine();
            Console.WriteLine("*** TestTenantManagementService ************************************");

            try
            {
                var output = await _tenantAppService.GetListAsync(new GetTenantsInput());

                Console.WriteLine("Total tenant count: " + output.TotalCount);

                foreach (var tenant in output.Items)
                {
                    Console.WriteLine($"- Id={tenant.Id}, Name={tenant.Name}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private string GetServerUrl()
        {
            return _remoteServiceOptions.RemoteServices.Default.BaseUrl.EnsureEndsWith('/');
        }
    }
}
