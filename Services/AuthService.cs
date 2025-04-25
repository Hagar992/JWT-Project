using JWT_Project.Helpers;
using JWT_Project.Models;
using JWT_Project.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWT_Project.Services
{
    // تعريف كلاس AuthService الذي يطبق الواجهة IAuthService
    public class AuthService : IAuthService
    {
        // تعريف المتغيرات الخاصة بالـ UserManager و RoleManager و JWT
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        // دالة البناء (Constructor) لضبط القيم باستخدام حقن التبعية (Dependency Injection)
        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value; // استخراج القيم من إعدادات الـ JWT من appsettings.json
        }

        // دالة لتسجيل مستخدم جديد
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            // التحقق إذا كان البريد الإلكتروني مسجل من قبل
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            // التحقق إذا كان اسم المستخدم مسجل من قبل
            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            // إنشاء مستخدم جديد بناءً على البيانات المدخلة
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            // إنشاء المستخدم مع كلمة المرور
            var result = await _userManager.CreateAsync(user, model.Password);

            // إذا فشل إنشاء المستخدم، نجمع رسائل الخطأ
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                    errors += $"{error.Description},";
                return new AuthModel { Message = errors };
            }

            // إضافة المستخدم إلى دور "User"
            await _userManager.AddToRoleAsync(user, "User");

            // إنشاء توكن JWT للمستخدم
            var jwtSecurityToken = await CreateJwtToken(user);

            // إرجاع النموذج الذي يحتوي على معلومات المصادقة
            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
        }

        // دالة لتسجيل الدخول باستخدام البريد الإلكتروني وكلمة المرور
        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            // البحث عن المستخدم بالبريد الإلكتروني
            var user = await _userManager.FindByEmailAsync(model.Email);

            // التحقق من وجود المستخدم وكلمة المرور
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            // إنشاء التوكن
            var jwtSecurityToken = await CreateJwtToken(user);
            // الحصول على الأدوار الخاصة بالمستخدم
            var rolesList = await _userManager.GetRolesAsync(user);

            // تعبئة البيانات في نموذج المصادقة
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            return authModel;
        }

        // دالة لإضافة دور لمستخدم
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            // البحث عن المستخدم باستخدام الـ ID
            var user = await _userManager.FindByIdAsync(model.UserId);

            // التحقق من وجود المستخدم والدور
            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            // التحقق إذا كان المستخدم بالفعل في هذا الدور
            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            // إضافة الدور للمستخدم
            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }

        // دالة خاصة لإنشاء التوكن JWT
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            // الحصول على الكليمات الخاصة بالمستخدم
            var userClaims = await _userManager.GetClaimsAsync(user);
            // الحصول على الأدوار
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            // إضافة كل دور إلى قائمة الكليمات
            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            // إنشاء الكليمات الأساسية
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims) // دمج كليمات المستخدم
            .Union(roleClaims); // دمج كليمات الأدوار

            // إنشاء مفتاح التوقيع
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            // إنشاء التوقيع باستخدام خوارزمية HmacSha256
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            // إنشاء التوكن JWT
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
