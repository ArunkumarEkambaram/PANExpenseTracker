-- ============================================================
-- PANExpenseTracker - Full Database Setup Script
-- Server=EGAIK-PC, Integrated Security=true, Database=PANExpenseTracker
-- ============================================================

USE PANExpenseTracker;
GO

-- ============================================================
-- EXPENSE CATEGORIES
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ExpenseCategories' AND xtype='U')
CREATE TABLE ExpenseCategories (
    CategoryId    INT IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(100) NOT NULL,
    Icon          NVARCHAR(50)  NULL,
    IsDeleted     BIT NOT NULL DEFAULT 0,
    CreatedAt     DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- INCOME CATEGORIES
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='IncomeCategories' AND xtype='U')
CREATE TABLE IncomeCategories (
    CategoryId    INT IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(100) NOT NULL,
    Icon          NVARCHAR(50)  NULL,
    IsDeleted     BIT NOT NULL DEFAULT 0,
    CreatedAt     DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- LOANS
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Loans' AND xtype='U')
CREATE TABLE Loans (
    LoanId              INT IDENTITY(1,1) PRIMARY KEY,
    FullName            NVARCHAR(200) NOT NULL,
    Address             NVARCHAR(500) NULL,
    MobileNo            NVARCHAR(20)  NULL,
    Amount              DECIMAL(18,2) NOT NULL,
    GivenDate           DATE          NOT NULL,
    InterestType        NVARCHAR(10)  NOT NULL CHECK (InterestType IN ('Monthly','Yearly')),
    InterestAmount      DECIMAL(18,2) NOT NULL DEFAULT 0,
    InterestPaidDate    DATE          NULL,
    Notes               NVARCHAR(1000) NULL,
    IsSettled           BIT NOT NULL DEFAULT 0,
    SettledDate         DATE NULL,
    IsDeleted           BIT NOT NULL DEFAULT 0,
    CreatedAt           DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt           DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- EXPENSES
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Expenses' AND xtype='U')
CREATE TABLE Expenses (
    ExpenseId       INT IDENTITY(1,1) PRIMARY KEY,
    ExpenseDate     DATE          NOT NULL,
    Details         NVARCHAR(500) NOT NULL,
    CategoryId      INT           NOT NULL REFERENCES ExpenseCategories(CategoryId),
    Amount          DECIMAL(18,2) NOT NULL,
    Notes           NVARCHAR(500) NULL,
    IsDeleted       BIT NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt       DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- INCOME
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Income' AND xtype='U')
CREATE TABLE Income (
    IncomeId        INT IDENTITY(1,1) PRIMARY KEY,
    IncomeDate      DATE          NOT NULL,
    Details         NVARCHAR(500) NOT NULL,
    CategoryId      INT           NOT NULL REFERENCES IncomeCategories(CategoryId),
    Amount          DECIMAL(18,2) NOT NULL,
    Notes           NVARCHAR(500) NULL,
    IsDeleted       BIT NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt       DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- SEED DATA - EXPENSE CATEGORIES
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM ExpenseCategories)
BEGIN
    INSERT INTO ExpenseCategories (Name, Icon) VALUES
    ('Food & Dining', '🍽️'),
    ('Transport', '🚗'),
    ('Shopping', '🛍️'),
    ('Utilities', '💡'),
    ('Healthcare', '🏥'),
    ('Entertainment', '🎬'),
    ('Education', '📚'),
    ('Rent', '🏠'),
    ('Others', '📦');
END
GO

-- ============================================================
-- SEED DATA - INCOME CATEGORIES
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM IncomeCategories)
BEGIN
    INSERT INTO IncomeCategories (Name, Icon) VALUES
    ('Salary', '💰'),
    ('Freelance', '💻'),
    ('Business', '🏢'),
    ('Investment Returns', '📈'),
    ('Rental Income', '🏘️'),
    ('Gift', '🎁'),
    ('Bonus', '🎯'),
    ('Others', '📦');
END
GO

PRINT 'PANExpenseTracker database setup complete!';

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='LoanInterestPayments' AND xtype='U')
CREATE TABLE LoanInterestPayments (
    PaymentId    INT IDENTITY(1,1) PRIMARY KEY,
    LoanId       INT           NOT NULL REFERENCES Loans(LoanId),
    PaymentDate  DATE          NOT NULL,
    AmountPaid   DECIMAL(18,2) NOT NULL,
    PaymentMode  NVARCHAR(20)  NOT NULL DEFAULT 'Cash', -- Cash | UPI | Bank
    Notes        NVARCHAR(500) NULL,
    IsDeleted    BIT           NOT NULL DEFAULT 0,
    CreatedAt    DATETIME2     NOT NULL DEFAULT GETDATE()
);
GO

Alter table Loans add InterestPercentage decimal(18,2)
Go