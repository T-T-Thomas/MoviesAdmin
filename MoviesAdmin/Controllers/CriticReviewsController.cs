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
    public IActionResult Create(int? movieId)
    {
        ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Title", movieId);

        return View();
    }
    

    // POST: CRITICREVIEWS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,MovieId,Description,Rating,IsPublished")] CriticReview criticReview)
    {
        //Assign userId to record
        criticReview.CreatedBy = _userId;

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

        ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Title", criticReview.MovieId);

        return View(criticReview);
    }

    // POST: CRITICREVIEWS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,MovieId,Description,Rating,IsPublished,CreatedBy,CreatedDate")] CriticReview criticReview)
    {
        if (id != criticReview.Id)
        {
            return NotFound();
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
