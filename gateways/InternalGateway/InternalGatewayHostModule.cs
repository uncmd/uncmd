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

            context.Services.AddMvc();
            context.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Internal Gateway API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });

            context.Services.AddOcelot(context.Services.GetConfiguration());
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            app.UseCorrelationId();
            app.UseVirtualFiles();
            app.UseRouting();
            app.UseAuthentication();

            // get from service discovery later
            var apiList = new List<string>()
            {
                "identity",
                "multi-tenancy",
                "blogging"
            };
            //app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                apiList.ForEach(apiItem =>
                {
                    options.SwaggerEndpoint($"/{apiItem}/swagger.json", $"{apiItem} API");
                });
            });

            app.UseConfiguredEndpoints();

            app.UseOcelot()
                .Wait();
        }
    }
}
