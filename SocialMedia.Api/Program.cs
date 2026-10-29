
using Microsoft.EntityFrameworkCore;
using SocialMedia.Api.Filters;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Data;
using SocialMedia.Infrastructure.Mappings;
using SocialMedia.Infrastructure.Repositories;
using SocialMedia.Services.Interfaces;
using SocialMedia.Services.Services;
using SocialMedia.Services.Validators;

namespace SocialMedia.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configurar la BD MySql
            var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
            builder.Services.AddDbContext<SocialMediaContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            #endregion

            // Add services to the container.
            //builder.Services.AddTransient<IPostRepository, PostRepository>();
            //builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IPostService, PostService>();
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            builder.Services.AddAutoMapper(typeof(PostProfile).Assembly);

            //Registrar Validadores de FluentValidation
            builder.Services.AddScoped<PostDtoValidator>();
            builder.Services.AddScoped<CrearPostDtoValidator>();
            builder.Services.AddScoped<ActualizarPostDtoValidator>();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
