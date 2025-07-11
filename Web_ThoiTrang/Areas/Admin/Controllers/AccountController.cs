// Đường dẫn: Web_ThoiTrang/Areas/Admin/Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Web_ThoiTrang.Data;
using Web_ThoiTrang.Models;

namespace Web_ThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")] // Đánh dấu đây là một Controller thuộc Area "Admin"
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Account
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả tài khoản từ cơ sở dữ liệu
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // GET: Admin/Account/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/Account/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống giả mạo yêu cầu
        public async Task<IActionResult> Create([Bind("Username,PasswordHash,Email,FullName,PhoneNumber,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                // TODO: Trước khi lưu, bạn cần hash mật khẩu (PasswordHash)
                // Ví dụ: user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                // Hoặc sử dụng Identity system
                user.CreatedAt = DateTime.Now; // Đặt ngày tạo
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Account/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,FullName,PhoneNumber,Role,IsActive")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy user cũ từ DB để giữ lại PasswordHash nếu không thay đổi
                    var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                    if (existingUser != null)
                    {
                        user.PasswordHash = existingUser.PasswordHash; // Giữ lại mật khẩu cũ nếu không có trường thay đổi mật khẩu
                        user.CreatedAt = existingUser.CreatedAt; // Giữ lại ngày tạo
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Account/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}