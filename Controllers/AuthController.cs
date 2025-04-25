using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using JWT_Project.Models;
using JWT_Project.Services;

namespace JWT_Project.Controllers
{
    // تعريف الراوت الأساسي للكنترولر: api/Auth (اسم الكنترولر بدون كلمة Controller)
    [Route("api/[controller]")]

    // هذا كنترولر API وليس MVC عادي، يرجع JSON فقط
    [ApiController]
    public class AuthController : ControllerBase
    {
        // تعريف متغير خاص بالخدمة المسؤولة عن المصادقة
        private readonly IAuthService _authService;

        // Constructor للكنترولر يستقبل خدمة المصادقة من الـ Dependency Injection
        public AuthController(IAuthService authService)
        {
            _authService = authService; // تخزين الخدمة في المتغير الخاص بالكنترولر
        }

        // Endpoint للتسجيل - يتم الوصول له عبر POST إلى api/Auth/register
        [HttpPost("register")]
        //public IActionResult Register(RegisterModel model) //ده تزامني، يعني الكود ينفذ خطوة بخطوة.
        //ده غير متزامن (Async)، يعني ممكن تنفذ عمليات طويلة (زي قاعدة بيانات أو API) من غير ما توقف السيرفر وتستنى النتيجة باستخدام await.
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            // التحقق من صحة البيانات المرسلة (Validation)
            //"لو البيانات مش صحيحة (يعني فيها مشكلة)، رجّع BadRequest"
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // إذا فيه خطأ، يرجع BadRequest مع التفاصيل

            // تنفيذ منطق التسجيل باستخدام الخدمة
            var result = await _authService.RegisterAsync(model);

            // إذا لم يتم التوثيق بنجاح، يرجع رسالة الخطأ
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            // إذا تم بنجاح، يرجع البيانات على شكل OK
            return Ok(result);
        }

        // Endpoint للحصول على التوكن - POST إلى api/Auth/token
        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            // التحقق من صحة النموذج
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // محاولة الحصول على التوكن باستخدام البيانات المدخلة
            var result = await _authService.GetTokenAsync(model);

            // فشل في المصادقة، رجع رسالة الخطأ
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            // نجاح في المصادقة، رجع البيانات (تشمل التوكن)
            return Ok(result);
        }

        // Endpoint لإضافة دور (Role) للمستخدم - POST إلى api/Auth/addrole
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            // التأكد من صحة النموذج
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // محاولة إضافة الدور باستخدام الخدمة
            var result = await _authService.AddRoleAsync(model);

            // إذا فيه رسالة خطأ، رجعها
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            // تم الإضافة بنجاح، يرجع نفس النموذج
            return Ok(model);

            #region ✅ ملخص سريع:
            
            //    الميثود وظيفتها
            //    RegisterAsync لتسجيل مستخدم جديد وإرجاع توكن
            //    GetTokenAsync لتسجيل الدخول(بإيميل وباسورد) وإرجاع توكن
            //    AddRoleAsync لإضافة Role(مثل User أو Admin) لمستخدم موجود
            #endregion
        }
    }
}

