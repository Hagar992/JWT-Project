using System.Threading.Tasks;
using JWT_Project.Models;
namespace JWT_Project.Services 
{
    // تعريف واجهة اسمها IAuthService تحتوي على توقيعات الدوال اللي لازم تُطبَّق في أي كلاس يطبق هذه الواجهة
    public interface IAuthService
    {
        // دالة لتسجيل مستخدم جديد بشكل غير متزامن، وتستقبل بيانات التسجيل من RegisterModel وتُرجع AuthModel
        Task<AuthModel> RegisterAsync(RegisterModel model);

        // دالة لتسجيل الدخول (توليد توكن) بشكل غير متزامن، تستقبل بيانات تسجيل الدخول وتُرجع AuthModel
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);

        // دالة لإضافة صلاحية (Role) لمستخدم بشكل غير متزامن، وتُرجع رسالة نصية بالنتيجة
        Task<string> AddRoleAsync(AddRoleModel model);
    }
}
