using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Repositories.General_Repositories;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using PharmacyManagementApi.Services;

namespace PharmacyManagementApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<PharmacyContext>(
            options => options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"))
        );
            #region Repository
            builder.Services.AddScoped<IReposiroty<int, Customer>, CustomerRepository>();
            builder.Services.AddScoped<IReposiroty<string ,UserCredential>, UserCredentialRepository>();
            builder.Services.AddScoped<IReposiroty<int ,Purchase>, PurchaseRepository>();
            builder.Services.AddScoped<IReposiroty<int, PurchaseDetail>, PurchaseDetailRepository>();
            builder.Services.AddScoped<IReposiroty<int, Stock>, StockRepository>();
            builder.Services.AddScoped<IReposiroty<int, Vendor>, VendorRepository>();
            builder.Services.AddScoped<IReposiroty<int, Medicine>, MedicineRepository>();
            builder.Services.AddScoped<IReposiroty<int, Category>, CategoryRepository>();
            builder.Services.AddScoped<IReposiroty<int, Order>, OrderRepository>();
            builder.Services.AddScoped<IReposiroty<int, OrderDetail>, OrderDetailRepository>();
            builder.Services.AddScoped<IReposiroty<int, DeliveryDetail>, DeliveryDetailRepository>();
            builder.Services.AddScoped<IReposiroty<int, Cart>, CartRepository>();
            builder.Services.AddScoped<ITransactionService, TransactionRepository>();
            builder .Services.AddScoped<StockJoinedRepository, StockJoinedRepository>();
            builder.Services.AddScoped<CustomerJoinedRepository, CustomerJoinedRepository>();


            #endregion

            #region PharmacyBL
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IPurchaseService, PurchaseService>();
            builder.Services.AddScoped<IUserService,UserService>();
            




            #endregion


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
