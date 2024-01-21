using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EnitBook.BL.Entities;
using EnitBook.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace EnitBook.web.Controllers
{
    public class PostsController : Controller
    {
        private readonly EnitBookDbContext _context;
        private readonly UserManager<User> _userManager;
        public PostsController(EnitBookDbContext context, UserManager<User> userManager)
        {
            
            _context = context;
            _userManager = userManager;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var enitBookDbContext = _context.Posts.Include(p => p.User);
            return View(await enitBookDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,UserId,Title,Content,PublishedDateTime")] Post post)
        {
            post.PublishedDateTime = DateTime.Now;
            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            post.UserId = currentUserId;
                    var postImage = HttpContext.Request.Form.Files["PostImage"];
                    if (postImage != null && postImage.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await postImage.CopyToAsync(stream);
                            post.PostImage = stream.ToArray();
                        }
                    }

                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Profils");
                

                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
                return View(post);
            
        }


        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
            return View(post);
        }

        // POST: Posts/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,UserId,Title,Content,PublishedDateTime")] Post updatedPost)
        {
            var userId = _userManager.GetUserId(User);
            var existingPost = await _context.Posts.FindAsync(id);
            if (id != updatedPost.PostId)
            {
                return NotFound();
            }

            // Check ownership before proceeding with the edit
            


            if (ModelState.IsValid)
            {
                try
                {
                    // Update other properties of the post
                    existingPost.UserId = updatedPost.UserId;
                    existingPost.Title = updatedPost.Title;
                    existingPost.Content = updatedPost.Content;
                    existingPost.PublishedDateTime = updatedPost.PublishedDateTime;

                    // Check if a new image has been uploaded
                    var newPostImage = HttpContext.Request.Form.Files["PostImage"];
                    if (newPostImage != null && newPostImage.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await newPostImage.CopyToAsync(stream);
                            existingPost.PostImage = stream.ToArray();
                        }
                    }

                    _context.Update(existingPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(updatedPost.PostId))
                    {
                        return NotFound();
                    }
                 
                    else
                    {
                        
                        throw;
                    }
                    if (updatedPost.UserId != userId)
                    {
                        return Forbid();
                    }
                }

                return RedirectToAction("Index", "Profils");
            }

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", updatedPost.UserId);
            return View(updatedPost);
        }


        // GET: Posts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = _userManager.GetUserId(User);
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            if (post.User.Id != userId)
            {
                return Forbid();
            }

            return View(post);
        }
        
    

    // POST: Posts/Delete/5
    [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'EnitBookDbContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Profils");
        }

        private bool PostExists(int id)
        {
          return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }


}
