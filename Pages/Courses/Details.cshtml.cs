using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CourseAppChallenge.Data;
using CourseAppChallenge.ViewModels;
using CourseAppChallenge.ViewModels.CourseViewModels;

namespace CourseAppChallenge.Pages.Courses;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public GetCourseByIdViewModel GetCourseByIdViewModel { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null) return NotFound(new ErrorResultViewModel("Id can not be null."));
        
        try
        {
            GetCourseByIdViewModel = await _context.Courses.AsNoTracking().
                Include(x => x.CourseItems).
                FirstOrDefaultAsync(y => y.CourseId == id);
            
            return Page();
        }
        catch (DbException ex)
        {
            return string.IsNullOrEmpty(GetCourseByIdViewModel?.CourseTitle) ? 
                NotFound(new ErrorResultViewModel("Can not find this course.")) : 
                StatusCode(500, new ErrorResultViewModel("Internal server error.", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResultViewModel("Something is wrong.", ex.Message));
        }
    }
}
