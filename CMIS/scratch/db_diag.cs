using CMIS.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMIS.Diagnostics;

public class DbCheck
{
    private readonly ApplicationDbContext _db;

    public DbCheck(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Run()
    {
        var incomes = await _db.FinancialIncomes.ToListAsync();
        Console.WriteLine($"Total Income Records: {incomes.Count}");
        Console.WriteLine($"Total Income Amount: {incomes.Sum(i => i.Amount)}");
        
        foreach (var income in incomes.Take(5))
        {
            Console.WriteLine($"- {income.IncomeType}: {income.Amount} on {income.EntryDate}");
        }

        var summaries = await _db.RecordSummaries.ToListAsync();
        Console.WriteLine($"Total Summary Records: {summaries.Count}");
    }
}
