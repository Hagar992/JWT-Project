// استيراد مكتبة ASP.NET Core الخاصة بالهوية (Identity) اللي فيها الكلاسات المتعلقة بالمستخدمين والأدوار
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

// استيراد مكتبة Entity Framework Core المسؤولة عن التعامل مع قاعدة البيانات
using Microsoft.EntityFrameworkCore;

// تعريف مساحة الأسماء الخاصة بموديلات المشروع
namespace JWT_Project.Models
{
    // تعريف كلاس ApplicationDbContext اللي بيكون مسؤول عن الاتصال بقاعدة البيانات
    // وبيورّث من IdentityDbContext اللي بيحتوي على جداول جاهزة زي Users و Roles
    // وتم تحديد نوع المستخدم بأنه ApplicationUser (كلاس مخصص بتكتبيه لاحقًا لتمديد خصائص المستخدم)
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // الكونستركتور (constructor) بياخد options لتكوين الاتصال بقاعدة البيانات، وبيبعتهم للـ base class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
