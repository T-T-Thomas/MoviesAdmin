using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;
using MoviesAdmin.Services;
using static System.Net.WebRequestMethods;

public class MoviesController : Controller
{
    private readonly MoviesAdminContext _context;
    private readonly AzureStorageService _azureStorage;


    public MoviesController(MoviesAdminContext context, AzureStorageService azureStorage)
    {
        _context = context;
        _azureStorage = azureStorage;
    }

    // GET: MOVIES
    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movie
            .OrderByDescending(m => m.ReleaseDate)
            .ToListAsync();

        return View(movies);
    }

    // GET: MOVIES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // GET: MOVIES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MOVIES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Summary,RunTime,Genre,AgeRating,ReleaseDate")] Movie movie, IFormFile ImageUpload)
    {
        if (ModelState.IsValid) // validate input
        {
            //
            // Upload file to Azure Stroage (after input validation occurs)
            //

            // Make a unique filename (GUID.ext)
            string extension = Path.GetExtension(ImageUpload.FileName); // includes the leading dot, e.g. ".jpg"
            string newFileName = $"{Guid.NewGuid()}{extension}";

            // Upload file to Azure Storage
            string url = await _azureStorage.UploadFileAsync(ImageUpload, newFileName);

            // Set the image URL in trail record            
            movie.ImageUrl = url;

            //
            // Save trail in database
            //

            _context.Add(movie); // add new trail to context (db)
            await _context.SaveChangesAsync(); // save context changes in db

            return RedirectToAction(nameof(Index)); // re-direct to Trails/Index page
        }

        return View(movie);
    }


    // GET: MOVIES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    // POST: MOVIES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Summary,RunTime,Genre,AgeRating,ReleaseDate")] Movie movie)
    {
        if (id != movie.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(movie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(movie.Id))
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
        return View(movie);
    }

    // GET: MOVIES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // POST: MOVIES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var movie = await _context.Movie.FindAsync(id);
        if (movie != null)
        {
            _context.Movie.Remove(movie);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MovieExists(int? id)
    {
        return _context.Movie.Any(e => e.Id == id);
    }
}