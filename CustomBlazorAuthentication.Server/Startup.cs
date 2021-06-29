using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Text;

namespace CustomBlazorAuthentication.Server
{
    using Microsoft.OpenApi.Models;
    using Shared;
    using Swashbuckle.AspNetCore.Filters;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class Startup
    {
        #region Properties
        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// Provider name
        /// </summary>
        public string ProviderName
        {
            get
            {
                return "System.Data.SqlClient";
            }
        }
        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return Configuration["ConnectionStrings:DefaultConnection"];
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //ASP.NET Core Identity Authentication
            var identityBuilder = services.AddIdentity<Model.User, Model.Role>(options => {
                //Password validation criteria
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });
            identityBuilder.AddDefaultTokenProviders();

            //UserStore management
            services.AddTransient<IPasswordHasher<Model.User>, CustomIdentity.PasswordHasher>();
            services.AddTransient<IUserStore<Model.User>, CustomIdentity.UserStore>(obj => new CustomIdentity.UserStore(ProviderName, ConnectionString));
            services.AddTransient<IRoleStore<Model.Role>, CustomIdentity.RoleStore>(obj => new CustomIdentity.RoleStore(ProviderName, ConnectionString));

            //JWT Bearer token authentication
            services.AddAuthentication(options => {
                //Set default JwtBearer authentication
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["JwtIssuer"],
                            ValidAudience = Configuration["JwtAudience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityKey"]))
                        };
                    });

            //Set Default JwtBearer authorization
            services.AddAuthorization(options => {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            //Configure json serialization
            services.AddControllersWithViews()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                });

            services.AddRazorPages();

            //Register the Swagger generator, defining 1 or more Swagger documents
            var commonDescription = "**Type here your swagger login page title**";
            var swaggerAuthenticatedDescription = commonDescription;
            swaggerAuthenticatedDescription += "\r\n";
            swaggerAuthenticatedDescription += "\r\nType here your swagger page description";

            var swaggerDescription = commonDescription;
            swaggerDescription += "\r\n";
            swaggerDescription += "\r\nTo access integration api login with user: demo password: demo";

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CustomBlazorAuthentication API",
                    Contact = new OpenApiContact
                    {
                        Name = "CodeDesignTips.com",
                        Email = "info@codedesigntips.com",
                        Url = new Uri("https://www.codedesigntips.com")
                    },
                    Description = swaggerDescription,
                    Version = "v1"
                });

                //Resolve apiDescriptions conflict
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                
                //Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlFilePath))
                    c.IncludeXmlComments(xmlFilePath);

                //Settings for token authentication
                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                //Add note on roles for methods requiring authentication
                c.OperationFilter<SwaggerAppendAuthorizeToSummaryOperationFilter>();
                //Filter user visible methods
                c.DocumentFilter<SwaggerAuthorizeRoleFilter>();
                //Ignore properties with [SwaggerIgnore] attribute
                c.SchemaFilter<SwaggerIgnoreFilter>();
                //Filter to manager response header documentation
                c.OperationFilter<ResponseHeaderFilter>();

                c.DocumentFilter<SwaggerAuthenticatedDescriptionFilter>(swaggerDescription, swaggerAuthenticatedDescription);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomBlazorAuthentication API v1");
                c.InjectStylesheet("/swagger/swagger.css");
                c.InjectJavascript("/swagger/swagger.js");
                c.DefaultModelsExpandDepth(-1);
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
