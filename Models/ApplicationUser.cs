using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project.Models
{
    // تعريف كلاس جديد اسمه ApplicationUser بيرث من IdentityUser علشان نضيف عليه خصائص مخصصة
    public class ApplicationUser : IdentityUser
    {
        // خاصية FirstName تمثل الاسم الأول للمستخدم
        // [Required] معناها إن الخاصية دي لازم تتدخل ومينفعش تبقى null أو فاضية
        // [MaxLength(50)] معناها أقصى عدد أحرف هو 50، لو زاد هيطلع Error في التحقق من البيانات
        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }
    }

}
