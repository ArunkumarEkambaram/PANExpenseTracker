using PANExpenseTracker.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddScoped<ILoanRepository>(sp => new LoanRepository(connectionString));
builder.Services.AddScoped<ILoanInterestPaymentRepository>(sp => new LoanInterestPaymentRepository(connectionString));
builder.Services.AddScoped<IExpenseRepository>(sp => new ExpenseRepository(connectionString));
builder.Services.AddScoped<IIncomeRepository>(sp => new IncomeRepository(connectionString));
builder.Services.AddScoped<ICategoryRepository>(sp => new CategoryRepository(connectionString));
builder.Services.AddScoped<IDashboardRepository>(sp => new DashboardRepository(connectionString));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
