using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Repositories.General_Repositories;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using PharmacyManagementApi.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PharmacyManagementApi
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Log4net
            builder.Services.AddLogging(l => l.AddLog4Net());
            #endregion

            #region JWT-Authorization Swagger set-up
            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            #endregion

            #region JWT-Authentication-Injection
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey:JWT"])),
                    };
                });
            #endregion

            #region Context
            builder.Services.AddDbContext<PharmacyContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"))
            );
            #endregion

            #region GPT-SERVICE

            builder.Services.AddHttpClient<MedicineDescriptorAIService>();
            builder.Services.AddSingleton<MedicineDescriptorAIService>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();

                var configuration = provider.GetRequiredService<IConfiguration>();
                var apiKey = configuration["OpenAI:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("OpenAI API key is missing from configuration.");
                }

                var logger = provider.GetRequiredService<ILogger<MedicineDescriptorAIService>>();

                return new MedicineDescriptorAIService(httpClient, apiKey, logger, configuration);
            });

            #endregion

            #region Repository
            builder.Services.AddScoped<IRepository<int, Customer>, CustomerRepository>();
            builder.Services.AddScoped<IRepository<string, UserCredential>, UserCredentialRepository>();
            builder.Services.AddScoped<IRepository<int, Purchase>, PurchaseRepository>();
            builder.Services.AddScoped<IRepository<int, PurchaseDetail>, PurchaseDetailRepository>();
            builder.Services.AddScoped<IRepository<int, Stock>, StockRepository>();
            builder.Services.AddScoped<IRepository<int, Vendor>, VendorRepository>();
            builder.Services.AddScoped<IRepository<int, Medicine>, MedicineRepository>();
            builder.Services.AddScoped<IRepository<int, Category>, CategoryRepository>();
            builder.Services.AddScoped<IRepository<int, Order>, OrderRepository>();
            builder.Services.AddScoped<IRepository<int, OrderDetail>, OrderDetailRepository>();
            builder.Services.AddScoped<IRepository<int, DeliveryDetail>, DeliveryDetailRepository>();
            builder.Services.AddScoped<IRepository<int, Cart>, CartRepository>();
            builder.Services.AddScoped<IRepository<int, Brand>, BrandRepository>();
            builder.Services.AddScoped<IRepository<int, Feedback>, FeedbackRepository>();
            builder.Services.AddScoped<IRepository<int, Medication>, MedicationRepository>();
            builder.Services.AddScoped<IRepository<int, MedicationItem>, MedicationItemRepository>();
            builder.Services.AddScoped<ITransactionService, TransactionRepository>();
            builder.Services.AddScoped<StockJoinedRepository, StockJoinedRepository>();
            builder.Services.AddScoped<OrderDetailsJoinedRepository, OrderDetailsJoinedRepository>();
            builder.Services.AddScoped<CustomerJoinedRepository, CustomerJoinedRepository>();
            #endregion

            #region BusinessLogic
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IFeedbackService, FeedBackService>();
            builder.Services.AddScoped<IViewService, ViewService>();
            builder.Services.AddScoped<IMedicationService, MedicationService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            #endregion
            #region CORS
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("MyCors", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
            #endregion

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("MyCors");

            app.UseAuthentication();
            app.UseAuthorization();
          

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}
