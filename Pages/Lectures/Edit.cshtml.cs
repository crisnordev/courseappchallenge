using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseAppChallenge.Data;
using CourseAppChallenge.Models;
using CourseAppChallenge.ViewModels;
using CourseAppChallenge.ViewModels.LectureViewModels;

namespace CourseAppChallenge.Pages.Lectures;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public EditLectureViewModel EditLectureViewModel { get; set; }

    public Lecture Lecture { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null) return NotFound(new ErrorResultViewModel("Id can not be null."));

        Lecture = await _context.Lectures.FirstOrDefaultAsync(m => m.LectureId == id);
        if (Lecture == null) return NotFound(new ErrorResultViewModel("Can not find this lecture."));

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            var entry = _context.Update(Lecture);
            entry.CurrentValues.SetValues(EditLectureViewModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return !LectureExists(Lecture.LectureId)
                ? NotFound(new ErrorResultViewModel("Can not find this lecture", ex.Message))
                : StatusCode(500, new ErrorResultViewModel("Something is wrong.", ex.Message));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new ErrorResultViewModel("Internal server error.", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResultViewModel("Something is wrong.", ex.Message));
        }
    }

    private bool LectureExists(Guid id)
    {
        return (_context.Lectures?.Any(e => e.LectureId == id)).GetValueOrDefault();
    }
}