using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.Persistent.Base;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.Circuits;
using DevExpress.ExpressApp.Xpo;
using FarmEase_230508.Blazor.Server.Services;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.Identity.Web;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OData;
using DevExpress.ExpressApp.WebApi.Services;
using FarmEase_230508.WebApi.JWT;
using DevExpress.ExpressApp.Security.Authentication;
using DevExpress.ExpressApp.Security.Authentication.ClientServer;
using DevExpress.ExpressApp.Core;
using FarmEase_230508.Module.BusinessObjects;

namespace FarmEase_230508.Blazor.Server;

public class Startup {
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services) {
        services.AddSingleton(typeof(Microsoft.AspNetCore.SignalR.HubConnectionHandler<>), typeof(ProxyHubConnectionHandler<>));

        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthenticationTokenProvider, JwtTokenProviderService>();
        services.AddScoped<CircuitHandler, CircuitHandlerProxy>();
        services.AddXaf(Configuration, builder => {
            builder.UseApplication<FarmEase_230508BlazorApplication>();
            builder.Modules
                .AddAuditTrailXpo()
                .AddCloningXpo()
                .AddConditionalAppearance()
                .AddDashboards(options => {
                    options.DashboardDataType = typeof(DevExpress.Persistent.BaseImpl.DashboardData);
                })
                .AddFileAttachments()
                .AddOffice()
                .AddReports(options => {
                    options.EnableInplaceReports = true;
                    options.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportDataV2);
                    options.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML;
                })
                .AddStateMachine(options => {
                    options.StateMachineStorageType = typeof(DevExpress.ExpressApp.StateMachine.Xpo.XpoStateMachine);
                })
                .AddValidation(options => {
                    options.AllowValidationDetailsAccess = false;
                })
                .AddViewVariants()
                .Add<FarmEase_230508.Module.FarmEase_230508Module>()
            	.Add<FarmEase_230508BlazorModule>();
            builder.ObjectSpaceProviders
                .AddSecuredXpo((serviceProvider, options) => {
                    string connectionString = null;
                    if(Configuration.GetConnectionString("ConnectionString") != null) {
                        connectionString = Configuration.GetConnectionString("ConnectionString");
                    }
#if EASYTEST
                    if(Configuration.GetConnectionString("EasyTestConnectionString") != null) {
                        connectionString = Configuration.GetConnectionString("EasyTestConnectionString");
                    }
#endif
                    ArgumentNullException.ThrowIfNull(connectionString);
                    options.ConnectionString = connectionString;
                    options.ThreadSafe = true;
                    options.UseSharedDataStoreProvider = true;
                })
                .AddNonPersistent();
            builder.Security
                .UseIntegratedMode(options => {
                    options.RoleType = typeof(PermissionPolicyRole);
                    // ApplicationUser descends from PermissionPolicyUser and supports the OAuth authentication. For more information, refer to the following topic: https://docs.devexpress.com/eXpressAppFramework/402197
                    // If your application uses PermissionPolicyUser or a custom user type, set the UserType property as follows:
                    options.UserType = typeof(FarmEase_230508.Module.BusinessObjects.ApplicationUser);
                    // ApplicationUserLoginInfo is only necessary for applications that use the ApplicationUser user type.
                    // If you use PermissionPolicyUser or a custom user type, comment out the following line:
                    options.UserLoginInfoType = typeof(FarmEase_230508.Module.BusinessObjects.ApplicationUserLoginInfo);
                    options.UseXpoPermissionsCaching();
                })
                .AddPasswordAuthentication(options => {
                    options.IsSupportChangePassword = true;
                })
                .AddAuthenticationProvider<CustomAuthenticationProvider>();
        });
        const string customBearerSchemeName = "CustomBearer";
        var authentication = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
        authentication
            .AddCookie(options => {
                options.LoginPath = "/LoginPage";
            })
            .AddJwtBearer(customBearerSchemeName, options => {
                options.TokenValidationParameters = new TokenValidationParameters() {
                    ValidateIssuerSigningKey = true,
                    //ValidIssuer = Configuration["Authentication:Jwt:Issuer"],
                    //ValidAudience = Configuration["Authentication:Jwt:Audience"],
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:Jwt:IssuerSigningKey"]))
                };
            });
        //Configure OAuth2 Identity Providers based on your requirements. For more information, see
        //https://docs.devexpress.com/eXpressAppFramework/402197/task-based-help/security/how-to-use-active-directory-and-oauth2-authentication-providers-in-blazor-applications
        //https://developers.google.com/identity/protocols/oauth2
        //https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow
        //https://developers.facebook.com/docs/facebook-login/manually-build-a-login-flow
        authentication.AddMicrosoftIdentityWebApp(Configuration, configSectionName: "Authentication:AzureAd", cookieScheme: null);
        authentication.AddMicrosoftIdentityWebApi(Configuration, configSectionName: "Authentication:AzureAd");

        services.AddAuthorization(options => {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(
                JwtBearerDefaults.AuthenticationScheme,
                customBearerSchemeName)
                    .RequireAuthenticatedUser()
                    .RequireXafAuthentication()
                    .Build();
        });

        services
            .AddXafWebApi(Configuration, options => {
                // Use options.BusinessObject<YourBusinessObject>() to make the Business Object available in the Web API and generate the GET, POST, PUT, and DELETE HTTP methods for it.
                options.BusinessObject<ApplicationUser>();
                options.BusinessObject<CropTask>();
                options.BusinessObject<Crop>();
            })
            .AddXpoServices();
        services
            .AddControllers()
            .AddOData((options, serviceProvider) => {
                options
                    .AddRouteComponents("api/odata", new EdmModelBuilder(serviceProvider).GetEdmModel())
                    .EnableQueryFeatures(100);
            });

        services.AddSwaggerGen(c => {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo {
                Title = "FarmEase_230508 API",
                Version = "v1",
                Description = @"Use AddXafWebApi(options) in the FarmEase_230508.Blazor.Server\Startup.cs file to make Business Objects available in the Web API."
            });
            c.AddSecurityDefinition("JWT", new OpenApiSecurityScheme() {
                Type = SecuritySchemeType.Http,
                Name = "Bearer",
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme() {
                            Reference = new OpenApiReference() {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "JWT"
                            }
                        },
                        new string[0]
                    },
            });
            var azureAdAuthorityUrl = $"{Configuration["Authentication:AzureAd:Instance"]}{Configuration["Authentication:AzureAd:TenantId"]}";
            c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows() {
                    AuthorizationCode = new OpenApiOAuthFlow() {
                        AuthorizationUrl = new Uri($"{azureAdAuthorityUrl}/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri($"{azureAdAuthorityUrl}/oauth2/v2.0/token"),
                        Scopes = new Dictionary<string, string> {
                            // Configure scopes corresponding to https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-expose-web-apis
                            { @"[Enter the scope name in the FarmEase_230508.Blazor.Server\Startup.cs file]", @"[Enter the scope description in the FarmEase_230508.Blazor.Server\Startup.cs file]" }
                        }
                    }
                }
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                {
                    new OpenApiSecurityScheme {
                        Name = "OAuth2",
                        Scheme = "OAuth2",
                        Reference = new OpenApiReference {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "OAuth2"
                        },
                        In = ParameterLocation.Header
                    },
                    new string[0]
                }
            });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        if(env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FarmEase_230508 WebApi v1");
                c.OAuthClientId(Configuration["Authentication:AzureAd:ClientId"]);
                c.OAuthUsePkce();
            });
        }
        else {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. To change this for production scenarios, see: https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseXaf();
        app.UseEndpoints(endpoints => {
            endpoints.MapXafEndpoints();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
            endpoints.MapControllers();
        });
    }
}
