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
using CourseAppChallenge.ViewModels.CourseItemViewModels;

namespace CourseAppChallenge.Pages.CourseItems;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public EditCourseItemViewModel EditCourseItemViewModel { get; set; } = default!;

    public CourseItem CourseItem { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null) return NotFound(new ErrorResultViewModel("Id can not be null."));

        CourseItem = await _context.CourseItems.FirstOrDefaultAsync(m => m.CourseItemId == id);
        if (CourseItem == null) return NotFound(new ErrorResultViewModel("Can not find this module."));

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            var entry = _context.Update(CourseItem);
            entry.CurrentValues.SetValues(EditCourseItemViewModel);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return !CourseItemExists(CourseItem.CourseItemId) ? NotFound(new ErrorResultViewModel("Can not find this module", ex.Message)) : StatusCode(500, new ErrorResultViewModel("Something is wrong.", ex.Message));
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

    private bool CourseItemExists(Guid id)
    {
        return (_context.CourseItems?.Any(e => e.CourseItemId == id)).GetValueOrDefault();
    }
}