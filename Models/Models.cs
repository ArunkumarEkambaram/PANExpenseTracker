namespace PANExpenseTracker.Models;

// ─── LOAN ───────────────────────────────────────────────────
public class Loan
{
    public int LoanId { get; set; }
    public string FullName { get; set; } = "";
    public string? Address { get; set; }
    public string? MobileNo { get; set; }
    public decimal Amount { get; set; }
    public DateTime GivenDate { get; set; }
    public string InterestType { get; set; } = "Monthly"; // Monthly | Yearly
    public decimal InterestAmount { get; set; }
    public decimal InterestPercentage { get; set; }
    public DateTime? InterestPaidDate { get; set; }
    public string? Notes { get; set; }
    public bool IsSettled { get; set; }
    public DateTime? SettledDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Computed
    public decimal TotalInterestDue
    {
        get
        {
            if (IsSettled) return 0;
            var months = (int)((DateTime.Today - GivenDate).TotalDays / 30.44);
            if (months <= 0) return 0;
            return InterestType == "Monthly"
                ? InterestAmount * months
                : InterestAmount * Math.Ceiling(months / 12.0m);
        }
    }
    public decimal TotalRepayable => Amount + TotalInterestDue;
}

public class LoanInterestPayment
{
    public int PaymentId { get; set; }
    public int LoanId { get; set; }
    public string BorrowerName { get; set; } = "";
    public DateTime PaymentDate { get; set; }
    public decimal AmountPaid { get; set; }
    public string PaymentMode { get; set; } = "Cash";
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    // For display
    public decimal LoanInterestAmount { get; set; }
    public string LoanInterestType { get; set; } = "";
}

// ─── EXPENSE CATEGORY ────────────────────────────────────────
public class ExpenseCategory
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = "";
    public string? Icon { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ─── INCOME CATEGORY ─────────────────────────────────────────
public class IncomeCategory
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = "";
    public string? Icon { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ─── EXPENSE ─────────────────────────────────────────────────
public class Expense
{
    public int ExpenseId { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string Details { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public string? CategoryIcon { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// ─── INCOME ──────────────────────────────────────────────────
public class Income
{
    public int IncomeId { get; set; }
    public DateTime IncomeDate { get; set; }
    public string Details { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public string? CategoryIcon { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// ─── DASHBOARD ───────────────────────────────────────────────
public class DashboardSummary
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalLoansOutstanding { get; set; }
    public decimal AllTimeIncome { get; set; }
    public decimal AllTimeExpenses { get; set; }
    public decimal AmountInHand => AllTimeIncome - AllTimeExpenses - TotalLoansOutstanding;
    public decimal MonthlyBurnRate { get; set; }
    public int ActiveLoansCount { get; set; }
    public List<CategoryBreakdown> TopExpenseCategories { get; set; } = new();
    public List<CategoryBreakdown> TopIncomeCategories { get; set; } = new();
    public List<MonthlyTrend> MonthlyTrends { get; set; } = new();
}

public class CategoryBreakdown
{
    public string Category { get; set; } = "";
    public string? Icon { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

public class MonthlyTrend
{
    public string Month { get; set; } = "";
    public decimal Income { get; set; }
    public decimal Expenses { get; set; }
}

// ─── FILTER ──────────────────────────────────────────────────
public class DateFilter
{
    public string Period { get; set; } = "month"; // day | week | month | year | all | custom
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public (DateTime from, DateTime to) GetRange()
    {
        var today = DateTime.Today;
        return Period switch
        {
            "day"   => (today, today),
            "week"  => (today.AddDays(-(int)today.DayOfWeek), today.AddDays(6 - (int)today.DayOfWeek)),
            "month" => (new DateTime(today.Year, today.Month, 1), new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month))),
            "year" => today.Month >= 4
              ? (new DateTime(today.Year, 4, 1), new DateTime(today.Year + 1, 3, 31))
              : (new DateTime(today.Year - 1, 4, 1), new DateTime(today.Year, 3, 31)),
            "all"   => (new DateTime(2000, 1, 1), new DateTime(2099, 12, 31)),
            "custom"=> (From ?? today.AddMonths(-1), To ?? today),
            _       => (new DateTime(today.Year, today.Month, 1), today)
        };
    }
}

