using System.Configuration;
using System;

namespace JWT_Project.Helpers
{
    public class JWT
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInDays { get; set; }
    }
    #region فايدة الكلاس ده في المشروع
    //الكلاس ده غالبًا بيتم ربطه بملف الإعدادات appsettings.json
    //وبيتم ربطه في Startup.cs أو Program.cs باستخدام:

        //services.Configure<JWT>(Configuration.GetSection("JWT"));
        //وبكده تقدر تستخدم الإعدادات دي في أي مكان في المشروع لما تعمل Inject للـ Options.
    #endregion
}
