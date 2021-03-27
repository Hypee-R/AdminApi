using System.Text;
using AccessControl.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

using AccessControl.Models;
using CorePMSAdmin.Models;
using ServicePMSAdmin.Services;

namespace PMSAdminApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCrententialsAccess();
            services.AddCors(o => o.AddPolicy("CorsPolicy", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials()));
            services.AddDbContext<BDContext>(o => o.UseSqlServer(_configuration.GetConnectionString("default")));

            services.AddTransient<IPMSAdminService, PMSAdminService>();

            var jwtSection = _configuration.GetSection("jwt");
            var jwtOptions = new JwtOptions();

            jwtSection.Bind(jwtOptions);
            services.AddAuthentication()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = jwtOptions.ValidateAudience,
                        ValidateLifetime = jwtOptions.ValidateLifetime
                    };
                });

            services.Configure<JwtOptions>(jwtSection);
            services.Configure<AccessOptions>(_configuration.GetSection("access"));
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseCredentials();
            app.UseMvc();
        }
    }
}
