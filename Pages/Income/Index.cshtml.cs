using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PANExpenseTracker.Data;
using PANExpenseTracker.Models;

namespace PANExpenseTracker.Pages.Income;

public class IndexModel : PageModel
{
    private readonly IIncomeRepository _repo;
    private readonly ICategoryRepository _cat;
    public IndexModel(IIncomeRepository repo, ICategoryRepository cat)
    { _repo = repo; _cat = cat; }

    public IEnumerable<PANExpenseTracker.Models.Income> IncomeList { get; set; } = [];
    public IEnumerable<IncomeCategory> Categories { get; set; } = [];
    public DateFilter Filter { get; set; } = new();
    public decimal TotalAmount { get; set; }

    [TempData] public string? SuccessMessage { get; set; }

    public async Task OnGetAsync(string period = "month", DateTime? from = null, DateTime? to = null)
    {
        ViewData["Title"] = "Income";
        ViewData["ActivePage"] = "Income";
        Filter = new DateFilter { Period = period, From = from, To = to };
        IncomeList = await _repo.GetAllAsync(Filter);
        Categories = await _cat.GetIncomeCategoriesAsync();
        TotalAmount = IncomeList.Sum(i => i.Amount);
    }

    public async Task<IActionResult> OnPostAddAsync(
        DateTime incomeDate, string details, int categoryId, decimal amount, string? notes)
    {
        await _repo.AddAsync(new PANExpenseTracker.Models.Income
        {
            IncomeDate = incomeDate, Details = details,
            CategoryId = categoryId, Amount = amount, Notes = notes
        });
        SuccessMessage = "Income record added.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(
        int incomeId, DateTime incomeDate, string details, int categoryId, decimal amount, string? notes)
    {
        await _repo.UpdateAsync(new PANExpenseTracker.Models.Income
        {
            IncomeId = incomeId, IncomeDate = incomeDate,
            Details = details, CategoryId = categoryId, Amount = amount, Notes = notes
        });
        SuccessMessage = "Income record updated.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _repo.SoftDeleteAsync(id);
        SuccessMessage = "Income record deleted.";
        return RedirectToPage();
    }
}
