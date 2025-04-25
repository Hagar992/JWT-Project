using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace JWT_Project.Models
{
    public class AddRoleModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
    #region MyRegion
    //[Required]: معناها إن القيمة لازم تكون موجودة، ولو مش موجودة هيتم رفض الطلب(BadRequest).

    //هذا الموديل بيُستخدم عادة مع HttpPost عشان يستقبل البيانات من Body في شكل JSON.
    #endregion
}