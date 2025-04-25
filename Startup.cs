using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using JWT_Project.Helpers;
using JWT_Project.Models;
using JWT_Project.Services;

namespace JWT_Project
{
    public class Startup
    {
        // تعريف الـ constructor الذي يقوم بتمرير إعدادات التطبيق من Configuration.
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // خاصية Configuration التي تحتوي على إعدادات التطبيق.
        public IConfiguration Configuration { get; }

        // هذا الميثود يتم استدعاؤه لتسجيل الخدمات التي يحتاجها التطبيق.
        public void ConfigureServices(IServiceCollection services)
        {
            // هنا نقوم بتكوين إعدادات JWT من ملف appsettings.json.
            services.Configure<JWT>(Configuration.GetSection("JWT"));

            // إضافة خدمات Identity الخاصة بالمستخدمين، بما في ذلك تطبيق Identity لمستخدمي التطبيق.
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // تسجيل خدمة AuthService للاستخدام في الـ Dependency Injection.
            services.AddScoped<IAuthService, AuthService>();

            // إعداد DbContext ليستخدم SQL Server مع سلسلة الاتصال من appsettings.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            // إعداد خدمات الـ Authentication باستخدام JWT Bearer Tokens.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false; // يمكن إرسال التوكن عبر HTTP
                o.SaveToken = false; // لا يتم حفظ التوكن في الجلسة
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // التحقق من صحة المفتاح المستخدم في التوقيع
                    ValidateIssuer = true, // التحقق من المصداقية
                    ValidateAudience = true, // التحقق من الجمهور المستهدف
                    ValidateLifetime = true, // التحقق من صلاحية التوكن
                    ValidIssuer = Configuration["JWT:Issuer"], // إعداد مصدر التوكن من الإعدادات
                    ValidAudience = Configuration["JWT:Audience"], // إعداد الجمهور المستهدف من الإعدادات
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])) // المفتاح السري
                };
            });

            // إضافة خدمات الـ Controllers (Controllers) لتمكين التعامل مع الـ API.
            services.AddControllers();

            // إضافة خدمة Swagger لتوثيق الـ API وتوليد الوثائق التفاعلية.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestApiJWT", Version = "v1" });
            });
        }

        // هذا الميثود يتم استدعاؤه لتكوين الـ Middleware الذي يدير طلبات HTTP.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // إذا كان التطبيق في بيئة Development، نعرض أخطاء مفصلة ونفعّل Swagger UI.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(); // تفعيل Swagger لتوثيق الـ API
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestApiJWT v1"));
            }

            // تحويل كل طلب HTTP إلى HTTPS (حماية الاتصال).
            app.UseHttpsRedirection();

            // تفعيل التوجيه بين الـ Routes.
            app.UseRouting();

            // تفعيل الـ Authentication و الـ Authorization للتحقق من هوية المستخدم وصلاحياته.
            app.UseAuthentication();
            app.UseAuthorization();

            // ربط الـ Controllers مع الـ Endpoints الخاصة بالـ API.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
