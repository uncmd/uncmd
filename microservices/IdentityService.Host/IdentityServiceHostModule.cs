using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.Security.Claims;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Uncmd.Shared;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace IdentityService.Host
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpEntityFrameworkCorePostgreSqlModule),
        typeof(AbpAuditLoggingEntityFrameworkCoreModule),
        typeof(AbpPermissionManagementEntityFrameworkCoreModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule),
        typeof(AbpIdentityHttpApiModule),
        typeof(AbpIdentityEntityFrameworkCoreModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpAspNetCoreMultiTenancyModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule)
        )]
    public class IdentityServiceHostModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            Configure<AbpMultiTenancyOptions>(options =>
            {
                options.IsEnabled = UncmdConsts.IsMultiTenancyEnabled;
            });

            context.Services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.ApiName = configuration["AuthServer:ApiName"];
                    options.RequireHttpsMetadata = false;
                });

            context.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Identity Service API", Version = "v1"});
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseNpgsql();
            });

            Configure<AbpAuditingOptions>(options =>
            {
                options.IsEnabledForGetRequests = true;
                options.ApplicationName = "IdentityService";
            });

            context.Services.AddHealthChecks()
                .AddNpgSql("Host=150.158.107.66; Port=5432; User Id=postgres; Password=xxxxxx; Database=auth;");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            app.UseCorrelationId();
            app.UseVirtualFiles();
            app.UseRouting();
            app.UseAuthentication();
            if (UncmdConsts.IsMultiTenancyEnabled)
            {
                app.UseMultiTenancy();
            }
            app.Use(async (ctx, next) =>
            {
                var currentPrincipalAccessor = ctx.RequestServices.GetRequiredService<ICurrentPrincipalAccessor>();
                var map = new Dictionary<string, string>()
                {
                    { "sub", AbpClaimTypes.UserId },
                    { "role", AbpClaimTypes.Role },
                    { "email", AbpClaimTypes.Email },
                    //any other map
                };
                var mapClaims = currentPrincipalAccessor.Principal.Claims.Where(p => map.Keys.Contains(p.Type)).ToList();
                currentPrincipalAccessor.Principal.AddIdentity(new ClaimsIdentity(mapClaims.Select(p => new Claim(map[p.Type], p.Value, p.ValueType, p.Issuer))));
                await next();
            });

            app.UseAbpRequestLocalization(); //TODO: localization?
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API");
            });
            app.UseAuditing();
            app.UseConfiguredEndpoints();

            app.UseHealthChecks("/healthChecks", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = WriteResponse,
                ResultStatusCodes = new Dictionary<HealthStatus, int>
                {
                    { HealthStatus.Healthy, 200 },
                    { HealthStatus.Unhealthy, 420 },
                    { HealthStatus.Degraded, 403 }
                }
            });
        }

        private static Task WriteResponse(HttpContext context, HealthReport healthReport)
        {
            context.Response.ContentType = "application/json";
            var result = JsonConvert.SerializeObject(new
            {
                code = context.Response.StatusCode,
                errors = healthReport.Entries.Select(p => new
                {
                    key = p.Key,
                    value = p.Value.Status.ToString()
                })
            });

            return context.Response.WriteAsync(result);
        }
    }
}
