﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using StudyResource.Data;
using StudyResource.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace StudyResource.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public FavoriteController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Add document to favorite
        [HttpPost]
        public async Task<IActionResult> AddToFavorite(int documentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var isAlreadyFavorite = await _context.Favorites
                .AnyAsync(f => f.UserId == user.Id && f.DocumentId == documentId); if (isAlreadyFavorite)
            {
                TempData["Message"] = "Đã lưu tài liệu";
                return RedirectToAction("Detail", "Document", new { id = documentId });
            }

                var favorite = new Favorite
            {
                UserId = user.Id,
                DocumentId = documentId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", "Document", new { id = documentId });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int documentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id && f.DocumentId == documentId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Đã xóa tài liệu khỏi danh sách yêu thích"; 
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Index()
         {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var favoriteDocuments = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .Include(f => f.Document)
                .ToListAsync();

            return View(favoriteDocuments);
         }
    
    }
}
