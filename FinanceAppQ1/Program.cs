// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;

namespace FinanceManagementQ1
{
    // a) Record for Transaction
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // b) Interface for processors
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c) Concrete processors
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Processed transaction #{transaction.Id}: {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Processed transaction #{transaction.Id}: {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Processed transaction #{transaction.Id}: {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}");
        }
    }

    // d) Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        // virtual deduction behavior
        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account] Applied transaction #{transaction.Id}. Deducted {transaction.Amount:C}. New balance: {Balance:C}");
        }
    }

    // e) Sealed SavingsAccount with override
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine($"[SavingsAccount] Transaction #{transaction.Id} failed: Insufficient funds (Attempted {transaction.Amount:C}, Available {Balance:C})");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"[SavingsAccount] Transaction #{transaction.Id} successful. Deducted {transaction.Amount:C}. Updated balance: {Balance:C}");
        }
    }

    // f) FinanceApp to integrate & simulate
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            Console.WriteLine("=== Finance Management System (Q1) ===\n");

            // i. Instantiate SavingsAccount
            var account = new SavingsAccount("SA-2025-001", 1000m);
            Console.WriteLine($"Created SavingsAccount {account.AccountNumber} with initial balance {account.Balance:C}\n");

            // ii. Create 3 transactions
            var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 300m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 700m, "Entertainment");

            // iii. Assign processors
            ITransactionProcessor proc1 = new MobileMoneyProcessor();   // Transaction 1
            ITransactionProcessor proc2 = new BankTransferProcessor();  // Transaction 2
            ITransactionProcessor proc3 = new CryptoWalletProcessor();  // Transaction 3

            // Process and apply T1
            proc1.Process(t1);
            account.ApplyTransaction(t1);
            _transactions.Add(t1);
            Console.WriteLine();

            // Process and apply T2
            proc2.Process(t2);
            account.ApplyTransaction(t2);
            _transactions.Add(t2);
            Console.WriteLine();

            // Process and apply T3
            proc3.Process(t3);
            account.ApplyTransaction(t3);
            _transactions.Add(t3);
            Console.WriteLine();

            // v. Summary
            Console.WriteLine("=== Transaction Log ===");
            foreach (var t in _transactions)
            {
                Console.WriteLine($"#{t.Id} | {t.Date:g} | {t.Category} | {t.Amount:C}");
            }

            Console.WriteLine($"\nFinal balance for account {account.AccountNumber}: {account.Balance:C}");
        }
    }

    // Main entry
    class Program
    {
        static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
