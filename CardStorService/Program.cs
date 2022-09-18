using CardStorageService.Data;
using CardStorService.Models;
using CardStorService.Services;
using CardStorService.Services.impl;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

namespace CardStorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container


            #region Configure Options Services

            builder.Services.Configure<DatabaseOptions>(options =>
            {
                builder.Configuration.GetSection("Settings:DatabaseOptions").Bind(options);
            });

            #endregion

            #region Logging Service

            //Настройка логгера - ограничения на вывод сообщения и ограничения размера файла
            // и обработка всех http запросов
            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.RequestHeaders.Add("Authorization");
                logging.RequestHeaders.Add("X-Real-IP");
                logging.RequestHeaders.Add("X-Forwarded-For");
            });

            // В этом блоке стандартная настройка логгера
            builder.Host.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();

            }).UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = true });


            #endregion

            #region Configur EF DBContext Service (CardStorageService Database)

            builder.Services.AddDbContext<CardStorageServiceDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);
            });
            #endregion

            #region Configur Repository Services

            builder.Services.AddScoped<IClientRepositoryService, ClientRepository>();
            builder.Services.AddScoped<ICardRepositoryService, CardRepository>();

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
            app.UseHttpLogging(); //17.09.2022

            app.MapControllers();

            app.Run();
        }
    }
}