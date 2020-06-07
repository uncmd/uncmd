using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Collections.Generic;
using Uncmd.Shared;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;

namespace InternalGateway
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpEntityFrameworkCoreSqliteModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule),
        typeof(AbpTenantManagementHttpApiModule),
        typeof(AbpAspNetCoreMultiTenancyModule)
        )]
    public class InternalGatewayHostModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            context.Services.AddOcelot(context.Services.GetConfiguration());
            context.Services.AddSwaggerForOcelot(configuration);
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            app.UsePathBase("/gateway");

            app.UseStaticFiles();

            app.UseSwaggerForOcelotUI(options =>
            {
                options.DownstreamSwaggerEndPointBasePath = "/gateway/swagger/docs";
                options.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseOcelot()
                .Wait();
        }
    }
}
