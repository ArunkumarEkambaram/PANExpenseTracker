using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PANExpenseTracker.Data;
using PANExpenseTracker.Models;

namespace PANExpenseTracker.Pages.Loans;

public class IndexModel : PageModel
{
    private readonly ILoanRepository _repo;
    public IndexModel(ILoanRepository repo) => _repo = repo;

    public IEnumerable<Loan> Loans { get; set; } = [];
    public DateFilter Filter { get; set; } = new();

    [TempData] public string? SuccessMessage { get; set; }
    [TempData] public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(string period = "year", DateTime? from = null, DateTime? to = null)
    {
        ViewData["Title"] = "Loans";
        ViewData["ActivePage"] = "Loans";
        Filter = new DateFilter { Period = period, From = from, To = to };
        Loans = await _repo.GetAllAsync(Filter);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _repo.SoftDeleteAsync(id);
        SuccessMessage = "Loan deleted successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSettleAsync(int id)
    {
        await _repo.MarkSettledAsync(id, DateTime.Today);
        SuccessMessage = "Loan marked as settled.";
        return RedirectToPage();
    }
}
