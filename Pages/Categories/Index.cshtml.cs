using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PANExpenseTracker.Data;
using PANExpenseTracker.Models;

namespace PANExpenseTracker.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly ICategoryRepository _repo;
    public IndexModel(ICategoryRepository repo) => _repo = repo;

    public IEnumerable<ExpenseCategory> ExpenseCategories { get; set; } = [];
    public IEnumerable<IncomeCategory> IncomeCategories { get; set; } = [];

    [TempData] public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        ViewData["Title"] = "Categories";
        ViewData["ActivePage"] = "Categories";
        ExpenseCategories = await _repo.GetExpenseCategoriesAsync();
        IncomeCategories  = await _repo.GetIncomeCategoriesAsync();
    }

    // ── Expense Category handlers
    public async Task<IActionResult> OnPostAddExpenseCategoryAsync(string name, string? icon)
    {
        await _repo.AddExpenseCategoryAsync(new ExpenseCategory { Name = name, Icon = icon });
        SuccessMessage = "Expense category added.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditExpenseCategoryAsync(int categoryId, string name, string? icon)
    {
        await _repo.UpdateExpenseCategoryAsync(new ExpenseCategory { CategoryId = categoryId, Name = name, Icon = icon });
        SuccessMessage = "Expense category updated.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteExpenseCategoryAsync(int id)
    {
        await _repo.SoftDeleteExpenseCategoryAsync(id);
        SuccessMessage = "Expense category removed.";
        return RedirectToPage();
    }

    // ── Income Category handlers
    public async Task<IActionResult> OnPostAddIncomeCategoryAsync(string name, string? icon)
    {
        await _repo.AddIncomeCategoryAsync(new IncomeCategory { Name = name, Icon = icon });
        SuccessMessage = "Income category added.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditIncomeCategoryAsync(int categoryId, string name, string? icon)
    {
        await _repo.UpdateIncomeCategoryAsync(new IncomeCategory { CategoryId = categoryId, Name = name, Icon = icon });
        SuccessMessage = "Income category updated.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteIncomeCategoryAsync(int id)
    {
        await _repo.SoftDeleteIncomeCategoryAsync(id);
        SuccessMessage = "Income category removed.";
        return RedirectToPage();
    }
}
