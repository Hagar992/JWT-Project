using JWT_Project; 
using Microsoft.AspNetCore.Hosting; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.Hosting; 
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace JWT_Project
{
   
    public class Program
    {
        // الطريقة الرئيسية التي يتم استدعاؤها عند تشغيل التطبيق.
        public static void Main(string[] args)
        {
            // إنشاء المضيف (Host) وتشغيله باستخدام إعدادات الاستضافة المحددة في Startup.
            CreateHostBuilder(args).Build().Run();
        }

        // إنشاء مضيف ويب (Web Host) باستخدام الإعدادات الافتراضية، مع تحديد الـ Startup class.
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args) // استخدام الإعدادات الافتراضية.
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // تحديد فئة Startup التي تحتوي على إعدادات التطبيق.
                    webBuilder.UseStartup<Startup>();
                });
    }
}
