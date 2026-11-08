using MoviesAdmin;
using MoviesAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class CriticReviewsController : Controller
{
    private readonly MoviesAdminContext _context;

    //the logged in user
    private string? _userId;

    public CriticReviewsController(MoviesAdminContext context)
    {
        _context = context;
    }

    // User object not available when constructor is called.
    // OnActionExecuting() runs after the controller is constructed, after model binding, and right before the action method runs, so everything is populated: HttpContext, RouteData, ModelState, User, bound action arguments, and so on.
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Controller is fully constructed; ControllerContext, HttpContext, etc. are set
        base.OnActionExecuting(context);

        _userId = User.GetUserId();

    }

    // GET: CRITICREVIEWS
    public async Task<IActionResult> Index()
    {
        // Get reviews that belong to the logged in user
        var trails = await _context.CriticReview
            .Where(m => m.CreatedBy == _userId)
            .Include(h => h.Movie)
            .ToListAsync();

        return View(trails);
    }

    // GET: CRITICREVIEWS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var criticReview = await _context.CriticReview
            .Where(m => m.CreatedBy == _userId)
            .Include(h => h.Movie)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (criticReview == null)
        {
            return NotFound();
        }

        return View(criticReview);
    }

    // GET: CRITICREVIEWS/Create
    public async Task<IActionResult> Create(int? movieId)
    {
        // Find IDs of movies that the current user has already reviewed
        var reviewedMovieIds = await _context.CriticReview
            .Where(r => r.CreatedBy == _userId)
            .Select(r => r.MovieId)
            .ToListAsync();

        // Get only movies that are NOT in the reviewed list
        var availableMovies = await _context.Movie
            .Where(m => !reviewedMovieIds.Contains(m.Id))
            .ToListAsync();

        // Pass the filtered list to the SelectList
        ViewData["MovieId"] = new SelectList(availableMovies, "Id", "Title", movieId);

        return View();
    }


    // POST: CRITICREVIEWS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,MovieId,Description,Rating,IsPublished")] CriticReview criticReview)
    {
        // Assign userId to record
        criticReview.CreatedBy = _userId;

        // Check if the critic has already reviewed this movie
        bool existingReview = await _context.CriticReview
            .AnyAsync(r => r.MovieId == criticReview.MovieId && r.CreatedBy == _userId);

        if (existingReview)
        {
            ModelState.AddModelError("MovieId", "You have already written a review for this movie.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(criticReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Title", criticReview.MovieId);

        return View(criticReview);
    }

    // GET: CRITICREVIEWS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var criticReview = await _context.CriticReview
            .Where(m => m.CreatedBy == _userId && m.IsPublished == false) // Only edit when not published yet
            .FirstOrDefaultAsync(m => m.Id == id);

        if (criticReview == null)
        {
            return NotFound();
        }

        // Find IDs of movies reviewed by the current user, EXCLUDING the current movie being edited
        var reviewedMovieIds = await _context.CriticReview
            .Where(r => r.CreatedBy == _userId && r.MovieId != criticReview.MovieId)
            .Select(r => r.MovieId)
            .ToListAsync();

        // Get available movies
        var availableMovies = await _context.Movie
            .Where(m => !reviewedMovieIds.Contains(m.Id))
            .ToListAsync();

        ViewData["MovieId"] = new SelectList(availableMovies, "Id", "Title", criticReview.MovieId);

        return View(criticReview);
    }

    // POST: CRITICREVIEWS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,MovieId,Description,Rating,IsPublished,CreatedDate")] CriticReview criticReview)
    {
        if (id != criticReview.Id)
        {
            return NotFound();
        }

        // Ensure the logged-in user owns this review and assign CreatedBy securely
        criticReview.CreatedBy = _userId;

        // Ensure they aren't changing it to a movie they already reviewed (excluding current review ID)
        bool duplicateReview = await _context.CriticReview
            .AnyAsync(r => r.MovieId == criticReview.MovieId && r.CreatedBy == _userId && r.Id != criticReview.Id);

        if (duplicateReview)
        {
            ModelState.AddModelError("MovieId", "You already have another review registered for this movie.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(criticReview);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CriticReviewExists(criticReview.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                // Catch unique constraint violations gracefully instead of crashing
                ModelState.AddModelError("", "A review for this movie by this user already exists.");
                ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Title", criticReview.MovieId);
                return View(criticReview);
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Title", criticReview.MovieId);
        return View(criticReview);
    }

    // GET: CRITICREVIEWS/Delete/5
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var criticReview = await _context.CriticReview
         .Where(m => m.CreatedBy == _userId)
         .Include(h => h.Movie)
         .FirstOrDefaultAsync(m => m.Id == id);

        //var criticReview = await _context.CriticReview
        //    .Where(m => m.CreatedBy == _userId && m.IsPublished == false) // Only edit when not published yet
        //    .FirstOrDefaultAsync(m => m.Id == id);


        if (criticReview == null)
        {
            return NotFound();
        }

        return View(criticReview);
    }

    // POST: CRITICREVIEWS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var criticreview = await _context.CriticReview.FindAsync(id);

        if (criticreview != null)
        {
            _context.CriticReview.Remove(criticreview);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool CriticReviewExists(int? id)
    {
        return _context.CriticReview.Any(e => e.Id == id);
    }
}
