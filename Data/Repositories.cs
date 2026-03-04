using Dapper;
using Microsoft.Data.SqlClient;
using PANExpenseTracker.Models;

namespace PANExpenseTracker.Data;

// ══════════════════════════════════════════════════════════════
//  LOAN REPOSITORY
// ══════════════════════════════════════════════════════════════
public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync(DateFilter filter);
    Task<Loan?> GetByIdAsync(int id);
    Task<int> AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
    Task SoftDeleteAsync(int id);
    Task MarkSettledAsync(int id, DateTime settledDate);
}

public class LoanRepository : ILoanRepository
{
    private readonly string _conn;
    public LoanRepository(string conn) => _conn = conn;

    public async Task<IEnumerable<Loan>> GetAllAsync(DateFilter filter)
    {
        var (from, to) = filter.GetRange();
        using var db = new SqlConnection(_conn);
        return await db.QueryAsync<Loan>(
            @"SELECT * FROM Loans
              WHERE IsDeleted=0
                AND CAST(GivenDate AS DATE) BETWEEN @From AND @To
              ORDER BY GivenDate DESC",
            new { From = from, To = to });
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryFirstOrDefaultAsync<Loan>(
            "SELECT * FROM Loans WHERE LoanId=@id AND IsDeleted=0", new { id });
    }

    public async Task<int> AddAsync(Loan loan)
    {
        using var db = new SqlConnection(_conn);
        return await db.ExecuteScalarAsync<int>(
            @"INSERT INTO Loans(FullName,Address,MobileNo,Amount,GivenDate,InterestType,
                    InterestAmount,InterestPercentage,InterestPaidDate,Notes)
                    VALUES(@FullName,@Address,@MobileNo,@Amount,@GivenDate,@InterestType,
                    @InterestAmount,@InterestPercentage,@InterestPaidDate,@Notes);
                    SELECT SCOPE_IDENTITY();", loan);
    }

    public async Task UpdateAsync(Loan loan)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            @"UPDATE Loans SET
                FullName=@FullName, Address=@Address, MobileNo=@MobileNo,
                Amount=@Amount, GivenDate=@GivenDate, InterestType=@InterestType,
                InterestAmount=@InterestAmount, InterestPaidDate=@InterestPaidDate,
                InterestPercentage=@InterestPercentage, Notes=@Notes, UpdatedAt=GETDATE()
              WHERE LoanId=@LoanId", loan);
    }

    public async Task SoftDeleteAsync(int id)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            "UPDATE Loans SET IsDeleted=1, UpdatedAt=GETDATE() WHERE LoanId=@id", new { id });
    }

    public async Task MarkSettledAsync(int id, DateTime settledDate)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            "UPDATE Loans SET IsSettled=1, SettledDate=@settledDate, UpdatedAt=GETDATE() WHERE LoanId=@id",
            new { id, settledDate });
    }
}

// ══════════════════════════════════════════════════════════════
//  EXPENSE REPOSITORY
// ══════════════════════════════════════════════════════════════
public interface IExpenseRepository
{
    Task<IEnumerable<Expense>> GetAllAsync(DateFilter filter);
    Task<Expense?> GetByIdAsync(int id);
    Task<int> AddAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task SoftDeleteAsync(int id);
}

public class ExpenseRepository : IExpenseRepository
{
    private readonly string _conn;
    public ExpenseRepository(string conn) => _conn = conn;

    public async Task<IEnumerable<Expense>> GetAllAsync(DateFilter filter)
    {
        var (from, to) = filter.GetRange();
        using var db = new SqlConnection(_conn);
        return await db.QueryAsync<Expense>(
            @"SELECT e.*, c.Name AS CategoryName, c.Icon AS CategoryIcon
              FROM Expenses e
              INNER JOIN ExpenseCategories c ON c.CategoryId=e.CategoryId
              WHERE e.IsDeleted=0
                AND CAST(e.ExpenseDate AS DATE) BETWEEN @From AND @To
              ORDER BY e.ExpenseDate DESC",
            new { From = from, To = to });
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryFirstOrDefaultAsync<Expense>(
            @"SELECT e.*, c.Name AS CategoryName, c.Icon AS CategoryIcon
              FROM Expenses e
              INNER JOIN ExpenseCategories c ON c.CategoryId=e.CategoryId
              WHERE e.ExpenseId=@id AND e.IsDeleted=0", new { id });
    }

    public async Task<int> AddAsync(Expense expense)
    {
        using var db = new SqlConnection(_conn);
        return await db.ExecuteScalarAsync<int>(
            @"INSERT INTO Expenses(ExpenseDate,Details,CategoryId,Amount,Notes)
              VALUES(@ExpenseDate,@Details,@CategoryId,@Amount,@Notes);
              SELECT SCOPE_IDENTITY();", expense);
    }

    public async Task UpdateAsync(Expense expense)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            @"UPDATE Expenses SET
                ExpenseDate=@ExpenseDate, Details=@Details,
                CategoryId=@CategoryId, Amount=@Amount,
                Notes=@Notes, UpdatedAt=GETDATE()
              WHERE ExpenseId=@ExpenseId", expense);
    }

    public async Task SoftDeleteAsync(int id)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            "UPDATE Expenses SET IsDeleted=1, UpdatedAt=GETDATE() WHERE ExpenseId=@id", new { id });
    }
}

// ══════════════════════════════════════════════════════════════
//  INCOME REPOSITORY
// ══════════════════════════════════════════════════════════════
public interface IIncomeRepository
{
    Task<IEnumerable<Income>> GetAllAsync(DateFilter filter);
    Task<Income?> GetByIdAsync(int id);
    Task<int> AddAsync(Income income);
    Task UpdateAsync(Income income);
    Task SoftDeleteAsync(int id);
}

public class IncomeRepository : IIncomeRepository
{
    private readonly string _conn;
    public IncomeRepository(string conn) => _conn = conn;

    public async Task<IEnumerable<Income>> GetAllAsync(DateFilter filter)
    {
        var (from, to) = filter.GetRange();
        using var db = new SqlConnection(_conn);
        return await db.QueryAsync<Income>(
            @"SELECT i.*, c.Name AS CategoryName, c.Icon AS CategoryIcon
              FROM Income i
              INNER JOIN IncomeCategories c ON c.CategoryId=i.CategoryId
              WHERE i.IsDeleted=0
                AND CAST(i.IncomeDate AS DATE) BETWEEN @From AND @To
              ORDER BY i.IncomeDate DESC",
            new { From = from, To = to });
    }

    public async Task<Income?> GetByIdAsync(int id)
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryFirstOrDefaultAsync<Income>(
            @"SELECT i.*, c.Name AS CategoryName, c.Icon AS CategoryIcon
              FROM Income i
              INNER JOIN IncomeCategories c ON c.CategoryId=i.CategoryId
              WHERE i.IncomeId=@id AND i.IsDeleted=0", new { id });
    }

    public async Task<int> AddAsync(Income income)
    {
        using var db = new SqlConnection(_conn);
        return await db.ExecuteScalarAsync<int>(
            @"INSERT INTO Income(IncomeDate,Details,CategoryId,Amount,Notes)
              VALUES(@IncomeDate,@Details,@CategoryId,@Amount,@Notes);
              SELECT SCOPE_IDENTITY();", income);
    }

    public async Task UpdateAsync(Income income)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            @"UPDATE Income SET
                IncomeDate=@IncomeDate, Details=@Details,
                CategoryId=@CategoryId, Amount=@Amount,
                Notes=@Notes, UpdatedAt=GETDATE()
              WHERE IncomeId=@IncomeId", income);
    }

    public async Task SoftDeleteAsync(int id)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            "UPDATE Income SET IsDeleted=1, UpdatedAt=GETDATE() WHERE IncomeId=@id", new { id });
    }
}

// ══════════════════════════════════════════════════════════════
//  CATEGORY REPOSITORY
// ══════════════════════════════════════════════════════════════
public interface ICategoryRepository
{
    Task<IEnumerable<ExpenseCategory>> GetExpenseCategoriesAsync(bool includeDeleted = false);
    Task<IEnumerable<IncomeCategory>> GetIncomeCategoriesAsync(bool includeDeleted = false);
    Task<int> AddExpenseCategoryAsync(ExpenseCategory cat);
    Task<int> AddIncomeCategoryAsync(IncomeCategory cat);
    Task UpdateExpenseCategoryAsync(ExpenseCategory cat);
    Task UpdateIncomeCategoryAsync(IncomeCategory cat);
    Task SoftDeleteExpenseCategoryAsync(int id);
    Task SoftDeleteIncomeCategoryAsync(int id);
}

public class CategoryRepository : ICategoryRepository
{
    private readonly string _conn;
    public CategoryRepository(string conn) => _conn = conn;

    public async Task<IEnumerable<ExpenseCategory>> GetExpenseCategoriesAsync(bool includeDeleted = false)
    {
        using var db = new SqlConnection(_conn);
        var sql = includeDeleted
            ? "SELECT * FROM ExpenseCategories ORDER BY Name"
            : "SELECT * FROM ExpenseCategories WHERE IsDeleted=0 ORDER BY Name";
        return await db.QueryAsync<ExpenseCategory>(sql);
    }

    public async Task<IEnumerable<IncomeCategory>> GetIncomeCategoriesAsync(bool includeDeleted = false)
    {
        using var db = new SqlConnection(_conn);
        var sql = includeDeleted
            ? "SELECT * FROM IncomeCategories ORDER BY Name"
            : "SELECT * FROM IncomeCategories WHERE IsDeleted=0 ORDER BY Name";
        return await db.QueryAsync<IncomeCategory>(sql);
    }

    public async Task<int> AddExpenseCategoryAsync(ExpenseCategory cat)
    {
        using var db = new SqlConnection(_conn);
        return await db.ExecuteScalarAsync<int>(
            "INSERT INTO ExpenseCategories(Name,Icon) VALUES(@Name,@Icon); SELECT SCOPE_IDENTITY();", cat);
    }

    public async Task<int> AddIncomeCategoryAsync(IncomeCategory cat)
    {
        using var db = new SqlConnection(_conn);
        return await db.ExecuteScalarAsync<int>(
            "INSERT INTO IncomeCategories(Name,Icon) VALUES(@Name,@Icon); SELECT SCOPE_IDENTITY();", cat);
    }

    public async Task UpdateExpenseCategoryAsync(ExpenseCategory cat)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            "UPDATE ExpenseCategories SET Name=@Name, Icon=@Icon WHERE CategoryId=@CategoryId", cat);
    }

    public async Task UpdateIncomeCategoryAsync(IncomeCategory cat)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            "UPDATE IncomeCategories SET Name=@Name, Icon=@Icon WHERE CategoryId=@CategoryId", cat);
    }

    public async Task SoftDeleteExpenseCategoryAsync(int id)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync("UPDATE ExpenseCategories SET IsDeleted=1 WHERE CategoryId=@id", new { id });
    }

    public async Task SoftDeleteIncomeCategoryAsync(int id)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync("UPDATE IncomeCategories SET IsDeleted=1 WHERE CategoryId=@id", new { id });
    }
}

// ══════════════════════════════════════════════════════════════
//  DASHBOARD REPOSITORY
// ══════════════════════════════════════════════════════════════
public interface IDashboardRepository
{
    Task<DashboardSummary> GetSummaryAsync(DateFilter filter);
}

public class DashboardRepository : IDashboardRepository
{
    private readonly string _conn;
    public DashboardRepository(string conn) => _conn = conn;

    public async Task<DashboardSummary> GetSummaryAsync(DateFilter filter)
    {
        var (from, to) = filter.GetRange();
        using var db = new SqlConnection(_conn);

        var totals = await db.QueryFirstAsync(
            @"SELECT
                (SELECT ISNULL(SUM(Amount),0) FROM Income WHERE IsDeleted=0 AND CAST(IncomeDate AS DATE) BETWEEN @From AND @To) + 
                (SELECT ISNULL(SUM(AmountPaid),0) FROM LoanInterestPayments WHERE IsDeleted=0 AND CAST(PaymentDate AS DATE) BETWEEN @From AND @To) AS TotalIncome,
                (SELECT ISNULL(SUM(Amount),0) FROM Expenses WHERE IsDeleted=0 AND CAST(ExpenseDate AS DATE) BETWEEN @From AND @To) AS TotalExpenses,
                (SELECT ISNULL(SUM(Amount),0) FROM Loans    WHERE IsDeleted=0 AND IsSettled=0)                                     AS TotalLoansOutstanding,
                (SELECT COUNT(*)              FROM Loans    WHERE IsDeleted=0 AND IsSettled=0)                                     AS ActiveLoansCount",
            new { From = from, To = to });

        var allTimeAmounts = await db.QueryFirstAsync(
             @"SELECT
                (SELECT ISNULL(SUM(Amount),0) FROM Income WHERE IsDeleted=0)
                + (SELECT ISNULL(SUM(AmountPaid),0) FROM LoanInterestPayments WHERE IsDeleted=0)
                AS AllTimeIncome,
                (SELECT ISNULL(SUM(Amount),0) FROM Expenses WHERE IsDeleted=0) AS AllTimeExpenses",
             new { });

        var burnRate = await db.ExecuteScalarAsync<decimal>(
            @"SELECT ISNULL(AVG(MonthlyTotal),0) FROM (
                SELECT SUM(Amount) AS MonthlyTotal
                FROM Expenses WHERE IsDeleted=0
                  AND ExpenseDate >= DATEADD(MONTH,-3,GETDATE())
                GROUP BY YEAR(ExpenseDate), MONTH(ExpenseDate)
              ) t");

        var topExpense = await db.QueryAsync<CategoryBreakdown>(
            @"SELECT TOP 5 c.Name AS Category, c.Icon, SUM(e.Amount) AS Amount
              FROM Expenses e
              INNER JOIN ExpenseCategories c ON c.CategoryId=e.CategoryId
              WHERE e.IsDeleted=0 AND CAST(e.ExpenseDate AS DATE) BETWEEN @From AND @To
              GROUP BY c.Name, c.Icon ORDER BY Amount DESC",
            new { From = from, To = to });

        var topIncome = await db.QueryAsync<CategoryBreakdown>(
            @"SELECT TOP 5 c.Name AS Category, c.Icon, SUM(i.Amount) AS Amount
              FROM Income i
              INNER JOIN IncomeCategories c ON c.CategoryId=i.CategoryId
              WHERE i.IsDeleted=0 AND CAST(i.IncomeDate AS DATE) BETWEEN @From AND @To
              GROUP BY c.Name, c.Icon ORDER BY Amount DESC",
            new { From = from, To = to });

        var trends = await db.QueryAsync<MonthlyTrend>(
            @"SELECT FORMAT(d.MonthDate,'MMM yyyy') AS Month,
                ISNULL(SUM(e.Amount),0) AS Expenses,
                ISNULL(SUM(i.Amount),0) AS Income
              FROM (
                SELECT DATEADD(MONTH,-n,DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1)) AS MonthDate
                FROM (VALUES(0),(1),(2),(3),(4),(5)) v(n)
              ) d
              LEFT JOIN Expenses e ON YEAR(e.ExpenseDate)=YEAR(d.MonthDate)
                AND MONTH(e.ExpenseDate)=MONTH(d.MonthDate) AND e.IsDeleted=0
              LEFT JOIN Income i ON YEAR(i.IncomeDate)=YEAR(d.MonthDate)
                AND MONTH(i.IncomeDate)=MONTH(d.MonthDate) AND i.IsDeleted=0
              GROUP BY d.MonthDate, FORMAT(d.MonthDate,'MMM yyyy')
              ORDER BY d.MonthDate");

        decimal totalExpAmt = (decimal)totals.TotalExpenses;
        var expList = topExpense.ToList();
        foreach (var c in expList)
            c.Percentage = totalExpAmt > 0 ? Math.Round(c.Amount / totalExpAmt * 100, 1) : 0;

        decimal totalIncAmt = (decimal)totals.TotalIncome;
        var incList = topIncome.ToList();
        foreach (var c in incList)
            c.Percentage = totalIncAmt > 0 ? Math.Round(c.Amount / totalIncAmt * 100, 1) : 0;

        return new DashboardSummary
        {
            TotalIncome = (decimal)totals.TotalIncome,
            TotalExpenses = (decimal)totals.TotalExpenses,
            TotalLoansOutstanding = (decimal)totals.TotalLoansOutstanding,
            AllTimeIncome = (decimal)allTimeAmounts.AllTimeIncome,
            AllTimeExpenses = (decimal)allTimeAmounts.AllTimeExpenses,
            ActiveLoansCount = (int)totals.ActiveLoansCount,
            MonthlyBurnRate = burnRate,
            TopExpenseCategories = expList,
            TopIncomeCategories = incList,
            MonthlyTrends = trends.ToList()
        };
    }
}

// ══════════════════════════════════════════════════════════════
//  LOAN INTEREST PAYMENT REPOSITORY
// ══════════════════════════════════════════════════════════════
public interface ILoanInterestPaymentRepository
{
    Task<IEnumerable<LoanInterestPayment>> GetAllAsync(DateFilter filter, int? loanId = null);
    Task<LoanInterestPayment?> GetByIdAsync(int id);
    Task<int> AddAsync(LoanInterestPayment payment);
    Task UpdateAsync(LoanInterestPayment payment);
    Task SoftDeleteAsync(int id);
}

public class LoanInterestPaymentRepository : ILoanInterestPaymentRepository
{
    private readonly string _conn;
    public LoanInterestPaymentRepository(string conn) => _conn = conn;

    public async Task<IEnumerable<LoanInterestPayment>> GetAllAsync(DateFilter filter, int? loanId = null)
    {
        var (from, to) = filter.GetRange();
        using var db = new SqlConnection(_conn);
        return await db.QueryAsync<LoanInterestPayment>(
            @"SELECT p.*, l.FullName AS BorrowerName, l.InterestAmount AS LoanInterestAmount, l.InterestType AS LoanInterestType
              FROM LoanInterestPayments p
              INNER JOIN Loans l ON l.LoanId = p.LoanId
              WHERE p.IsDeleted = 0
                AND CAST(p.PaymentDate AS DATE) BETWEEN @From AND @To
                AND (@LoanId IS NULL OR p.LoanId = @LoanId)
              ORDER BY p.PaymentDate DESC",
            new { From = from, To = to, LoanId = loanId });
    }

    public async Task<LoanInterestPayment?> GetByIdAsync(int id)
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryFirstOrDefaultAsync<LoanInterestPayment>(
            @"SELECT p.*, l.FullName AS BorrowerName, l.InterestAmount AS LoanInterestAmount, l.InterestType AS LoanInterestType
              FROM LoanInterestPayments p
              INNER JOIN Loans l ON l.LoanId = p.LoanId
              WHERE p.PaymentId = @id AND p.IsDeleted = 0", new { id });
    }

    public async Task<int> AddAsync(LoanInterestPayment payment)
    {
        using var db = new SqlConnection(_conn);
        var paymentId = await db.ExecuteScalarAsync<int>(
            @"INSERT INTO LoanInterestPayments(LoanId,PaymentDate,AmountPaid,PaymentMode,Notes)
              VALUES(@LoanId,@PaymentDate,@AmountPaid,@PaymentMode,@Notes);
              SELECT SCOPE_IDENTITY();", payment);

        // Auto-update InterestPaidDate on the Loan
        await db.ExecuteAsync(
            "UPDATE Loans SET InterestPaidDate=@PaymentDate WHERE LoanId=@LoanId",
            new { payment.PaymentDate, payment.LoanId });

        return paymentId;
    }

    public async Task UpdateAsync(LoanInterestPayment payment)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            @"UPDATE LoanInterestPayments SET
                LoanId=@LoanId, PaymentDate=@PaymentDate,
                AmountPaid=@AmountPaid, PaymentMode=@PaymentMode, Notes=@Notes
              WHERE PaymentId=@PaymentId", payment);

        // Re-sync InterestPaidDate to latest payment date for this loan
        await db.ExecuteAsync(
            @"UPDATE Loans SET InterestPaidDate=(
                SELECT MAX(PaymentDate) FROM LoanInterestPayments
                WHERE LoanId=@LoanId AND IsDeleted=0)
              WHERE LoanId=@LoanId", new { payment.LoanId });
    }

    public async Task SoftDeleteAsync(int id)
    {
        using var db = new SqlConnection(_conn);
        // Get LoanId before deleting
        var payment = await GetByIdAsync(id);
        await db.ExecuteAsync(
            "UPDATE LoanInterestPayments SET IsDeleted=1 WHERE PaymentId=@id", new { id });

        if (payment != null)
        {
            // Re-sync InterestPaidDate after deletion
            await db.ExecuteAsync(
                @"UPDATE Loans SET InterestPaidDate=(
                    SELECT MAX(PaymentDate) FROM LoanInterestPayments
                    WHERE LoanId=@LoanId AND IsDeleted=0)
                  WHERE LoanId=@LoanId", new { payment.LoanId });
        }
    }
}
