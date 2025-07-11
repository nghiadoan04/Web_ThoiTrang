
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web_ThoiTrang.Data;
using Web_ThoiTrang.Models; // Đảm bảo đã có Model User
using BCrypt.Net; // Cần cài đặt package này cho việc hash mật khẩu
using Web_ThoiTrang.ViewModels;
using System.Security.Claims; // Cần thiết cho ClaimsPrincipal và Claim
using Microsoft.AspNetCore.Authentication; // Cần thiết cho SignInAsync, SignOutAsync
using Microsoft.AspNetCore.Authentication.Cookies; // Cần thiết nếu sử dụng Cookie Authentication

namespace Web_ThoiTrang.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem Username hoặc Email đã tồn tại chưa
                if (_context.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }

                // Tạo đối tượng User từ ViewModel và gán cứng Role
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Role = "Customer", // Gán cứng Role
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập tên đăng nhập và mật khẩu.");
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View();
            }

            // === BẮT ĐẦU PHẦN THÊM LOGIC XÁC THỰC VÀ CHUYỂN HƯỚNG ===

            // 1. Tạo danh sách Claims (Thông tin về người dùng)
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username), // Tên người dùng
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ID người dùng
        new Claim(ClaimTypes.Role, user.Role) // Vai trò của người dùng (Admin/Customer)
    };

            // 2. Tạo ClaimsIdentity (đại diện cho danh tính người dùng)
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 3. Đăng nhập người dùng (tạo cookie xác thực)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            TempData["SuccessMessage"] = $"Chào mừng, {user.FullName ?? user.Username}!";

            // 4. Chuyển hướng người dùng dựa trên vai trò
            if (user.Role == "Admin")
            {
                // Chuyển hướng đến Admin Dashboard (ví dụ: /Admin/Home/Index)
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else // Role là Customer hoặc các role khác
            {
                // Chuyển hướng đến trang chủ của người dùng (ví dụ: /Home/Index)
                return RedirectToAction("Index", "Home", new { area = "" }); // area = "" để chỉ ra không thuộc Area nào
            }
            // =======================================================
        }

        // GET: /Auth/Logout (hoặc POST tùy ý)
        public async Task<IActionResult> Logout()
        {
            // Đăng xuất người dùng (xóa cookie xác thực)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công.";
            // Chuyển hướng về trang chủ
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}