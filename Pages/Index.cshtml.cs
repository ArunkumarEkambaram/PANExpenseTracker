using Microsoft.AspNetCore.Mvc.RazorPages;
using PANExpenseTracker.Data;
using PANExpenseTracker.Models;

namespace PANExpenseTracker.Pages;

public class IndexModel : PageModel
{
    private readonly IDashboardRepository _dash;
    public IndexModel(IDashboardRepository dash) => _dash = dash;

    public DashboardSummary Summary { get; set; } = new();
    public DateFilter Filter { get; set; } = new();

    public async Task OnGetAsync(string period = "month", DateTime? from = null, DateTime? to = null)
    {
        ViewData["Title"] = "Dashboard";
        ViewData["ActivePage"] = "Dashboard";

        Filter = new DateFilter { Period = period, From = from, To = to };
        Summary = await _dash.GetSummaryAsync(Filter);
    }
}
