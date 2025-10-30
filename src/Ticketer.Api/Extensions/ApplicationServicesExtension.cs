// ===================================THIS FILE WAS AUTO GENERATED===================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Google;
using Ticketer.Api.Accounts;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Ticketer.Api.Entities;
using Ticketer.Api.Models;
using Ticketer.Api.Configurations;
using Ticketer.Api.TokenServices;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Ticketer.Api.DTOs.DtoProfiles;
using Ticketer.Api;
using Ticketer.Api.Repositories.Interfaces;
using Ticketer.Api.Repositories.Implementations;
using Ticketer.Api.Services.Interfaces;
using Ticketer.Api.Services.Implementations;



namespace Ticketer.Api.Extensions
{
    /// <summary>
    /// Extension for adding application services
    /// </summary>
    public static class ApplicationServicesExtension
    {
        /// <summary>
        /// Cache configuration key
        /// </summary>
        private const string cacheConfigurationKey = "Framework:CommonConfig:Cache";
        private const string jwtConfigurationKey = "Authentication:JwtSettings";
        private const string googleConfigurationKey = "Authentication:Google";
        private const string oidcConfigurationKey = "Authentication:OIDC";

        /// <summary>
        /// Add application services
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {


            #region===========================DBContext Registration===========================
             // services.AddDbContextPool<TicketerDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultAppConnection")));
             services.AddDbContext<TicketerDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultAppConnection")));

            #endregion

            #region===========================IOption Configurations===========================
             services.Configure<OIDCConfiguration>(configuration.GetSection(oidcConfigurationKey));
             services.Configure<GoogleConfiguration>(configuration.GetSection(googleConfigurationKey));
             services.Configure<JwtConfiguration>(configuration.GetSection(jwtConfigurationKey));
             services.Configure<CacheConfiguration>(configuration.GetSection(cacheConfigurationKey));

            #endregion

            #region===========================DI Registrations===========================
             services.AddTransient<ITicketService, TicketService>();
             services.AddTransient<ITicketRepository, TicketRepository>();

            #endregion

            #region===========================Auto Mapper Configurations===========================

             services.AddAutoMapper(cfg =>
             {
                 cfg.AddProfile<TicketProfile>();
             });

            #endregion

            #region===========================CORS Registrations===========================
            var corsOrigins = configuration["CorsOrigins"]?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", policyBuilder =>
                {
                    policyBuilder
                        .WithOrigins(corsOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            #endregion

            #region===========================Authentication Registrations===========================

             services.AddTransient<ITokenService, TokenService>();
             services.AddTransient<IAccountService, AccountService>();
             services.AddTransient<TokenManagerMiddleware>();

             services.AddIdentityCore<User>(options =>
                    {
                       options.User.RequireUniqueEmail = true;
                        options.Password.RequiredLength = 8;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                    })
                   .AddRoles<Role>()
                   .AddEntityFrameworkStores<TicketerDbContext>()
                   .AddSignInManager();


            var jwtSettings = new JwtConfiguration();
            configuration.GetSection("Authentication:JwtSettings").Bind(jwtSettings);

            var oidcSettings = new OIDCConfiguration();
            configuration.GetSection("Authentication:OIDC").Bind(oidcSettings);

            var googleSettings = new GoogleConfiguration();
            configuration.GetSection("Authentication:Google").Bind(googleSettings);


            // Policy scheme chooses the right handler per request.
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "MultiAuth";
                    options.DefaultChallengeScheme = "MultiAuth";
                })
                .AddPolicyScheme("MultiAuth", "JWT or OIDC/Google", options =>
                {
                    options.ForwardDefaultSelector = ctx =>
                    {
                        var hasBearer = ctx.Request.Headers["Authorization"].ToString()
                            .StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);

                        // APIs: prefer JWT (no browser redirects)
                        if (ctx.Request.Path.StartsWithSegments("/api") || hasBearer)
                            return JwtBearerDefaults.AuthenticationScheme;

                        // Browser pages: use cookies (challenge goes to OIDC/Google)
                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    };

                    // If a browser hits a protected page without auth, challenge via OIDC by default.
                    options.ForwardChallenge = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
                {
                    o.LoginPath = "/login";
                    o.AccessDeniedPath = "/denied";
                    o.SlidingExpiration = true;
                })

                 // ---- JWT Auth ----
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, b =>
                {
                    b.RequireHttpsMetadata = true;
                    b.SaveToken = true;
                    b.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key!)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                })

                // Your internal OIDC (e.g., Azure AD/Okta/Auth0/your own)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, o =>
                {
                    o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.Authority = oidcSettings.Authority;
                    o.ClientId = oidcSettings.ClientId;
                    o.ClientSecret = oidcSettings.ClientSecret;
                    o.ResponseType = "code";
                    o.SaveTokens = true;
                    o.GetClaimsFromUserInfoEndpoint = true;
                    o.Scope.Add("openid");
                    o.Scope.Add("profile");
                    o.Scope.Add("email");
                })

                // ---- Google (quick) via OAuth handler ----
                .AddGoogle("Google", o =>
                {
                    o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.ClientId = googleSettings.ClientId!;
                    o.ClientSecret = googleSettings.ClientSecret!;
                    // Default CallbackPath is /signin-google; register it in Google Cloud Console
                    o.SaveTokens = true;
                    o.Scope.Add("email");
                    o.Scope.Add("profile");
                    // Optional: map extras
                    // o.ClaimActions.MapJsonKey("urn:google:picture", "picture");
                });

            #endregion

            #region===========================Authorization Registrations===========================
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireUserRole", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("User") ||
                        context.User.IsInRole("Viewer") ||
                        context.User.IsInRole("Admin")));
            });

            #endregion

            #region===========================Other Registrations===========================
             services.AddMemoryCache();

             services.AddHttpContextAccessor();
            #endregion


            return services;
        }
    }
}
