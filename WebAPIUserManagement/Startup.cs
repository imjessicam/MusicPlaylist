using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using MusicPlaylistModel.Data;
using MusicPlaylistModel.Repositories;

namespace WebAPIUserManagement
{
    public class Startup
    {
        // Konfiguracja serwisow w aplikacji
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpClient();
            services.AddCors();


            services.AddDbContextFactory<DatabaseContext>(options => options.UseSqlServer("server=DESKTOP-9FN9TUU\\SQLEXPRESS; database=MusicPlaylist; Trusted_Connection=True; Connection Timeout=30"));

            // Services. AddTransient - dodaje usluge przejsciowa typu okreslonego w <> parametrze IServiceCollection
            //services.AddTransient<CustomerRepository>();
            services.AddTransient<UserRepository>();
            services.AddTransient<PlaylistRepository>();
            services.AddTransient<SongRepository>();
            services.AddTransient<ArtistRepository>();
            services.AddTransient<AlbumRepository>();
            services.AddTransient<CategoryRepository>();


        }

        // ??
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting(); // wskazywanie odpowiednich adresow

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });


            //app.Run();
        }
    }
}
