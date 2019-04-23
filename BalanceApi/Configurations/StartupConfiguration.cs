using System;
using System.Collections.Generic;
using Balance.DAL;
using Balance.DAL.Repositories;
using Balance.Models.Options;
using Balance.Services.Abstractions;
using Balance.Services.Implementations;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace Balance.Api.Configurations
{
    public static class StartupConfiguration
    {
        public static void ConfigureCustomServices(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureApiVersioning(services);
            ConfigureSwagger(services);
            ConfigureDb(services, configuration);
            ConfigureServices(services, configuration);
            ConfigureIdentityServer(services, configuration);
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddCors();
            services.AddOptions();
            services.Configure<TransactionOptions>(configuration.GetSection("TransactionOptions"));
        }

        private static void ConfigureApiVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(0, 0);
            });
        }

        private static void ConfigureDb(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BalanceDbContext>(options =>
                    options.UseNpgsql(
                        configuration.GetConnectionString("Default"),
                        b => b.MigrationsAssembly(typeof(BalanceDbContext).Assembly.FullName)),
                ServiceLifetime.Transient, ServiceLifetime.Transient);
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using Bearer scheme. Example \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey",

                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", Array.Empty<string>() }
                });

                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "User balance API."
                });
            });
        }

        private static void ConfigureIdentityServer(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            services.AddTransient<IProfileService, IdentityClaimsProfileService>();

            var identityServer = services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(IdentityConfiguration.GetIdentityResources())
                .AddInMemoryApiResources(IdentityConfiguration.GetApiResources())
                .AddInMemoryClients(IdentityConfiguration.GetClients())
                .AddProfileService<IdentityClaimsProfileService>();

            identityServer.Services.AddTransient<ICorsPolicyService>(p =>
            {
                var corsService =
                    new DefaultCorsPolicyService(p.GetRequiredService<ILogger<DefaultCorsPolicyService>>())
                    {
                        AllowAll = true
                    };
                return corsService;
            });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = configuration["IS4:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = configuration["IS4:ApiName"];
                    options.ApiSecret = configuration["IS4:ApiSecret"].Sha256();
                });
        }
    }
}