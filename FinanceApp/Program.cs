using System;
using System.Collections.Generic;

public record Transaction(int Id, DateTime Date, decimal Amount, string Category);


public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction t) =>
        Console.WriteLine($"[BankTransfer] Processed {t.Amount:C} for {t.Category} on {t.Date:d}");
}
public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction t) =>
        Console.WriteLine($"[MoMo] Processed {t.Amount:C} for {t.Category} on {t.Date:d}");
}
public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction t) =>
        Console.WriteLine($"[Crypto] Processed {t.Amount:C} for {t.Category} on {t.Date:d}");
}

public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}


public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
            return;
        }
        Balance -= transaction.Amount;
        Console.WriteLine($"New balance: {Balance:C}");
    }
}


public class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    public void Run()
    {
        
        var acct = new SavingsAccount("SV-001", 1000m);

        
        var t1 = new Transaction(1, DateTime.Today, 120m, "Groceries");
        var t2 = new Transaction(2, DateTime.Today, 250m, "Utilities");
        var t3 = new Transaction(3, DateTime.Today, 80m, "Entertainment");

        
        ITransactionProcessor p1 = new MobileMoneyProcessor();
        ITransactionProcessor p2 = new BankTransferProcessor();
        ITransactionProcessor p3 = new CryptoWalletProcessor();

        p1.Process(t1);
        p2.Process(t2);
        p3.Process(t3);

        
        acct.ApplyTransaction(t1);
        acct.ApplyTransaction(t2);
        acct.ApplyTransaction(t3);

       
        _transactions.AddRange(new[] { t1, t2, t3 });
    }
}

public class Program
{
    public static void Main() => new FinanceApp().Run();
}
