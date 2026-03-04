using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PANExpenseTracker.Data;
using PANExpenseTracker.Models;

namespace PANExpenseTracker.Pages.Expenses;

public class IndexModel : PageModel
{
    private readonly IExpenseRepository _repo;
    private readonly ICategoryRepository _cat;
    public IndexModel(IExpenseRepository repo, ICategoryRepository cat)
    { _repo = repo; _cat = cat; }

    public IEnumerable<Expense> Expenses { get; set; } = [];
    public IEnumerable<ExpenseCategory> Categories { get; set; } = [];
    public DateFilter Filter { get; set; } = new();
    public decimal TotalAmount { get; set; }

    [TempData] public string? SuccessMessage { get; set; }

    public async Task OnGetAsync(string period = "month", DateTime? from = null, DateTime? to = null)
    {
        ViewData["Title"] = "Expenses";
        ViewData["ActivePage"] = "Expenses";
        Filter = new DateFilter { Period = period, From = from, To = to };
        Expenses = await _repo.GetAllAsync(Filter);
        Categories = await _cat.GetExpenseCategoriesAsync();
        TotalAmount = Expenses.Sum(e => e.Amount);
    }

    public async Task<IActionResult> OnPostAddAsync(
        DateTime expenseDate, string details, int categoryId, decimal amount, string? notes)
    {
        await _repo.AddAsync(new Expense
        {
            ExpenseDate = expenseDate, Details = details,
            CategoryId = categoryId, Amount = amount, Notes = notes
        });
        SuccessMessage = "Expense added.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(
        int expenseId, DateTime expenseDate, string details, int categoryId, decimal amount, string? notes)
    {
        await _repo.UpdateAsync(new Expense
        {
            ExpenseId = expenseId, ExpenseDate = expenseDate,
            Details = details, CategoryId = categoryId, Amount = amount, Notes = notes
        });
        SuccessMessage = "Expense updated.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _repo.SoftDeleteAsync(id);
        SuccessMessage = "Expense deleted.";
        return RedirectToPage();
    }
}
