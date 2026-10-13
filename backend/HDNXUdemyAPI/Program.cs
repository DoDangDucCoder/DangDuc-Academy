using Asp.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Asp.Versioning.ApiExplorer;
using HDNXUdemyAPI.Middlewares;
using HDNXUdemyAPI.ModelHelp;
using HDNXUdemyAPI.ProjectExtensisons;
using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Base;
using HDNXUdemyServices.CommonFunction;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stripe;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO.Compression;
using System.Reflection;
using System.Threading.RateLimiting;
using static HDNXUdemyAPI.ProjectExtensisons.ApplicationSwaggerExtension;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HDNXUdemyAPI
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            builder.Host.UseSerilog((ctx, lc) => lc
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File("logs/hdnx-api-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
                .WriteTo.File(new Serilog.Formatting.Json.JsonFormatter(), "logs/hdnx-api-structured-.json", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14));
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys")));

            // Add services to the container.

            // Add services to the container.
            HelperConstantsModel.LoadConfig(configuration);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
            
            var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(ProjectConfig.ConnectionString ?? string.Empty);
            dataSourceBuilder.UseNodaTime();
            var dataSource = dataSourceBuilder.Build();
            builder.Services.AddDbContext<ProjectContext>(options => options.UseNpgsql(dataSource, x => x.UseNodaTime()));

            builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(xmlFilePath);
            });

            builder.Services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(config =>
            {
                config.GroupNameFormat = "'v'VVV";
                config.SubstituteApiVersionInUrl = true;
            });
            builder.Services.AddSwashbuckleSwagger();
            builder.Services.AddApplicationServicesExtension(configuration);
            builder.Services.CustomerApplicationJWTExtension();
            builder.Services.AddAuthorization();
            builder.Services.AddCors(x =>
            {
                x.AddDefaultPolicy(polocy =>
                {
                    polocy.WithOrigins(
                        "http://localhost",
                        "http://localhost:80",
                        "http://localhost:81",
                        "http://localhost:4200",
                        "http://127.0.0.1:4200",
                        "http://localhost:4201",
                        "http://127.0.0.1:4201",
                        "https://web-hdnx.devproinsights.com",
                        "http://web-hdnx.devproinsights.com",
                        "http://hdnx-admin.devproinsights.com",
                        "https://hdnx-admin.devproinsights.com")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
            builder.Services.AddSignalR();
            builder.Services.AddMemoryCache();
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json", "text/plain" });
            });
            builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.SmallestSize);
            builder.Services.AddHealthChecks()
                .AddNpgSql(ProjectConfig.ConnectionString ?? string.Empty, name: "postgresql", tags: new[] { "db", "ready" })
                .AddCheck("self", () => HealthCheckResult.Healthy("API is running"), tags: new[] { "live" });
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 120,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));
            });
            StripeConfiguration.ApiKey = ProjectConfig.StripeSecretKey;

            var app = builder.Build();

            var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            // Configure the HTTP request pipeline.
            var env = app.Environment;
            app.Use(async (context, next) =>
            {
                context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
                context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
                context.Response.Headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
                var csp = context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase)
                    ? "default-src 'self'; img-src 'self' data:; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'; frame-ancestors 'none'; object-src 'none'"
                    : "default-src 'self'; frame-ancestors 'none'; object-src 'none'";
                context.Response.Headers.TryAdd("Content-Security-Policy", csp);
                await next();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwashbuckleSwagger(apiVersionProvider);
            }
            else
            {
                app.UseHsts();
            }

            app.UseResponseCompression();
            app.UseExceptionMiddleware();
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath)),
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=600");
                },
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings = { [".m3u8"] = "application/x-mpegURL", [".ts"] = "video/mp2t" }
                }
            });
            app.UseRouting();
            app.UseRateLimiter();
            app.UseCors();
            app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<HubConfigProject>("/hub-notification");

            app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live")
            });
            app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });

            app.Run();
        }
    }
}
