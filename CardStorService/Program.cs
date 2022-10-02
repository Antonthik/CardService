using AutoMapper;
using CardStorageService.Data;
using CardStorService.Mappings;
using CardStorService.Models;
using CardStorService.Models.Requests;
using CardStorService.Models.Validators;
using CardStorService.Services;
using CardStorService.Services.impl;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.Text;

namespace CardStorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container

            #region Configure FluentValidator

            builder.Services.AddScoped<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();//��������� ���������
            builder.Services.AddScoped<IValidator<CreateCardRequest>, CreateCardRequestValidator>();//��������� ���������
            builder.Services.AddScoped<IValidator<CreateClientRequest>, CreateClientValidator>();//��������� ���������

            #endregion

            #region Configure Mapper
            //�������� �������
            var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MappingsProfile()));//������� ������������ �������
            var mapper = mapperConfiguration.CreateMapper();//������� ������
            builder.Services.AddSingleton(mapper);//����������� ������

            #endregion

            #region Configure Options Services

            builder.Services.Configure<DatabaseOptions>(options =>
            {
                builder.Configuration.GetSection("Settings:DatabaseOptions").Bind(options);
            });

            #endregion

            #region Logging Service

            //��������� ������� - ����������� �� ����� ��������� � ����������� ������� �����
            // � ��������� ���� http ��������
            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.RequestHeaders.Add("Authorization");
                logging.RequestHeaders.Add("X-Real-IP");
                logging.RequestHeaders.Add("X-Forwarded-For");
            });

            // � ���� ����� ����������� ��������� �������
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


            #region Configure JWT Tokens

            //�������� ����� ��� ������ � ������� - �������� �� ����� �� ������������
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
            })
             //���������� ��������� -   
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new
                TokenValidationParameters //������ ��������� ������
                {
                    ValidateIssuerSigningKey = true,//�������� ����
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticateService.SecretKey)),//�������� ��������� ����
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            #endregion



            #region Configure Services

            builder.Services.AddSingleton<IAuthenticateService, AuthenticateService>();
            //builder.Services.AddSingleton<AuthenticateService>();
            #endregion



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();//24.09.2022 �������������� ������� ��� ���������� ����������������

            //24.09.2022 ������ ������� - ���������

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "��� ������", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()//��������� � ������ ������ ��� �������� ������
                {
                    Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()//��������� � ������ - ����� ������� ������ ������� ������������ ������
                                                                         //� ����� �� ������ AddSecurityDefinition
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();//24.09.2022 �������� � �������� #region Configure JWT Tokens 

            app.UseAuthentication();//24.09.2022 �������� � �������� #region Configure JWT Tokens 
            app.UseAuthorization();
            app.UseHttpLogging(); //17.09.2022

            app.MapControllers();

            app.Run();
        }
    }
}