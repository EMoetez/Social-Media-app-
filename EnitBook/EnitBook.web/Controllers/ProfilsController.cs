using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnitBook.BL.Entities;
using EnitBook.DAL;
using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;


namespace EnitBook.web.Controllers
{
    public class ProfilsController : Controller
    {
        private readonly EnitBookDbContext _context;
        private readonly UserManager<User> _userManager;
        


        public ProfilsController(EnitBookDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyFriends()
        {
            string userId = _userManager.GetUserId(User);

            var friends = await _context.Friends
                .Where(f => f.HostId == userId)
                .ToListAsync();

            return View("MyFriends", friends);
        }
    
    // GET: Profils
    public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var profiles = await _context.Profils
                    .Where(p => p.UserId == user.Id)
                    .ToListAsync();
                var friends = await _context.Friends
                    .Where(f => f.HostId == user.Id)
                    .ToListAsync();
                var friendUserIds = friends.Select(f => f.UserId).ToList();
                //To complete
                // Retrieve users associated with friends
                var userPosts = await _context.Posts
                    .Where(p => p.UserId == user.Id)
                    .ToListAsync();

                var friendPosts = await _context.Posts
                    .Where(p => friendUserIds.Contains(p.UserId))
                    .Include(p => p.User)
                    .ToListAsync();
                var userComments = await _context.Comments
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();
                var friendComments = await _context.Comments
                    .Where(c => friendUserIds.Contains(c.UserId))
                    .ToListAsync();

                var allComments = userComments.Concat(friendComments).ToList();

                var allPosts = userPosts.Concat(friendPosts).ToList();
                var viewModel = new ProfilPostsViewModel
                {
                    Profiles = profiles,
                    Posts = allPosts,
                    Comments = allComments
                };

                return View(viewModel);
            }

            return Problem("User not found.");
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(string commentContent, int postId)
        {
            try
            {
                // Get the current user ID
                var userId = _userManager.GetUserId(User);

                // Create a new comment
                var comment = new Comment
                {
                    UserId = userId,
                    PostId = postId,
                    Content = commentContent,
                    commentDateTime = DateTime.Now
                };
                
               
                // Add the comment to the database
         
               await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log or output the exception for debugging
                Console.WriteLine($"Exception in AddComment: {ex}");
                return RedirectToAction(nameof(Index)); // Redirect to a meaningful page
            }
        }

        [HttpPost]
        public async Task<ActionResult> SearchAsync()
        {
            string input = Request.Form["inputValue"];
            var matchingUsers = _context.Users.Where(u => u.FirstName == input).ToList();
            var user = await _userManager.GetUserAsync(User);

            if (input == user.FirstName)
            {
                // Set fname to null if the searched user is the current user
                ViewBag.fname = null;
            }
            else if (matchingUsers.Any())
            {
                var match = matchingUsers.First();
                ViewBag.fname = match.FirstName;
                ViewBag.lname = match.LastName;
                ViewBag.mail = match.Email;
                ViewBag.img = match.ProfilePicture;
            }
            else
            {
                ViewBag.NoMatch = true;
            }

            return View("test");
        }
        [HttpPost]
        public async Task<ActionResult> AddFriendAsync(string email, string firstname, string lastname)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var friendUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.FirstName == firstname && u.LastName == lastname);
                var friend = new Friend
                {
                    name = firstname,
                    lstname = lastname,
                    HostId = user.Id,
                    UserId = friendUser.Id
                 };

                 _context.Friends.Add(friend);   
                 await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }




        // GET: Profils/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Profils == null)
            {
                return NotFound();
            }

            var profil = await _context.Profils
                .FirstOrDefaultAsync(m => m.IdProfil == id);
            if (profil == null)
            {
                return NotFound();
            }

            return View(profil);
        }

        // GET: Profils/Create
        public async Task<IActionResult> CreateAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var profil = await _context.Profils
                    .Where(p => p.UserId == user.Id)
                    .ToListAsync();

                return View(profil);
            }
            return _context.Profils != null ?
                            View(await _context.Profils.ToListAsync()) :
                            Problem("Entity set 'EnitBookDbContext.Profils'  is null.");
        }

        // POST: Profils/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProfil")] Profil profil)
        {
            if (ModelState.IsValid)
            {
                _context.Add(profil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profil);


        }

        // GET: Profils/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Profils == null)
            {
                return NotFound();
            }

            var profil = await _context.Profils.FindAsync(id);
            if (profil == null)
            {
                return NotFound();
            }
            return View(profil);
        }

        // POST: Profils/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProfil")] Profil profil)
        {
            if (id != profil.IdProfil)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfilExists(profil.IdProfil))
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
            return View(profil);
        }


        // GET: Profils/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Profils == null)
            {
                return NotFound();
            }

            var profil = await _context.Profils
                .FirstOrDefaultAsync(m => m.IdProfil == id);
            if (profil == null)
            {
                return NotFound();
            }

            return View(profil);
        }
  
    

    // POST: Profils/Delete/5
    [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Profils == null)
            {
                return Problem("Entity set 'EnitBookDbContext.Profils'  is null.");
            }
            var profil = await _context.Profils.FindAsync(id);
            if (profil != null)
            {
                _context.Profils.Remove(profil);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfilExists(int id)
        {
          return (_context.Profils?.Any(e => e.IdProfil == id)).GetValueOrDefault();
        }
    }
}
