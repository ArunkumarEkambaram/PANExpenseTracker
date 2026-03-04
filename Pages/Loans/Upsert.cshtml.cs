using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PANExpenseTracker.Data;
using PANExpenseTracker.Models;

namespace PANExpenseTracker.Pages.Loans;

public class UpsertModel : PageModel
{
    private readonly ILoanRepository _repo;
    public UpsertModel(ILoanRepository repo) => _repo = repo;

    public async Task<IActionResult> OnPostAsync(
        int loanId, string fullName, string? address, string? mobileNo,
        decimal amount, DateTime givenDate, string interestType,
        decimal interestAmount, DateTime? interestPaidDate, string? notes)
    {
        var loan = new Loan
        {
            LoanId           = loanId,
            FullName         = fullName,
            Address          = address,
            MobileNo         = mobileNo,
            Amount           = amount,
            GivenDate        = givenDate,
            InterestType     = interestType,
            InterestAmount   = interestAmount,
            InterestPaidDate = interestPaidDate,
            Notes            = notes
        };

        if (loanId == 0)
            await _repo.AddAsync(loan);
        else
            await _repo.UpdateAsync(loan);

        TempData["SuccessMessage"] = loanId == 0 ? "Loan added successfully." : "Loan updated.";
        return RedirectToPage("/Loans/Index");
    }
}
