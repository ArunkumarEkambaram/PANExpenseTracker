using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PANExpenseTracker.Data;
using PANExpenseTracker.Models;

namespace PANExpenseTracker.Pages.LoanPayments;

public class IndexModel : PageModel
{
    private readonly ILoanInterestPaymentRepository _repo;
    private readonly ILoanRepository _loans;
    public IndexModel(ILoanInterestPaymentRepository repo, ILoanRepository loans)
    { _repo = repo; _loans = loans; }

    public IEnumerable<LoanInterestPayment> Payments { get; set; } = [];
    public IEnumerable<Loan> ActiveLoans { get; set; } = [];
    public DateFilter Filter { get; set; } = new();
    public decimal TotalCollected { get; set; }
    public int? SelectedLoanId { get; set; }

    [TempData] public string? SuccessMessage { get; set; }

    public async Task OnGetAsync(string period = "month", DateTime? from = null,
        DateTime? to = null, int? loanId = null)
    {
        ViewData["Title"] = "Loan Payments";
        ViewData["ActivePage"] = "LoanPayments";
        Filter = new DateFilter { Period = period, From = from, To = to };
        SelectedLoanId = loanId;
        Payments = await _repo.GetAllAsync(Filter, loanId);
        ActiveLoans = await _loans.GetAllAsync(new DateFilter { Period = "all" });
        TotalCollected = Payments.Sum(p => p.AmountPaid);
    }

    public async Task<IActionResult> OnPostAddAsync(int loanId, DateTime paymentDate,
        decimal amountPaid, string paymentMode, string? notes)
    {
        await _repo.AddAsync(new LoanInterestPayment
        {
            LoanId = loanId,
            PaymentDate = paymentDate,
            AmountPaid = amountPaid,
            PaymentMode = paymentMode,
            Notes = notes
        });
        SuccessMessage = "Payment recorded successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(int paymentId, int loanId,
        DateTime paymentDate, decimal amountPaid, string paymentMode, string? notes)
    {
        await _repo.UpdateAsync(new LoanInterestPayment
        {
            PaymentId = paymentId,
            LoanId = loanId,
            PaymentDate = paymentDate,
            AmountPaid = amountPaid,
            PaymentMode = paymentMode,
            Notes = notes
        });
        SuccessMessage = "Payment updated.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _repo.SoftDeleteAsync(id);
        SuccessMessage = "Payment deleted.";
        return RedirectToPage();
    }
}