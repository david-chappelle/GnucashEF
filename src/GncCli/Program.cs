using System.CommandLine;
using GncEF;
using GncEF.Models;

namespace GncCli;

internal class Program
{
    private static readonly Option<FileInfo> _dbOption;
    private static readonly Option<DateOnly?> _startDateOption;
    private static readonly Option<DateOnly?> _endDateOption;
    private static readonly Command _accountCommand;
    private static readonly Argument<string> _accountNameArgument;
    private static readonly Option<bool> _recursiveOption;

    private static FileInfo? _dbFile = null;
    private static DateOnly? _startDate = null;
    private static DateOnly? _endDate = null;
    private static GncContext? _db = null;

    static Program()
    {
        _dbOption = new Option<FileInfo>("--database-file", "-db")
        {
            Description = "Path to the Gnucash sqlite database.",
            Recursive = true,
            Required = true
        }.AcceptExistingOnly();

        _startDateOption = new Option<DateOnly?>("--start-date", "-sd")
        {
            Description = "Starting date filter for searching.",
            Recursive = true,
            Required = false
        };

        _endDateOption = new Option<DateOnly?>("--end-date", "-ed")
        {
            Description = "End date filter for searching.",
            Recursive = true,
            Required = false
        };

        _accountNameArgument = new Argument<string>("account-name")
        {
            Description = "The name of the top level account to process"
        };

        _recursiveOption = new Option<bool>("--recursive", "-r")
        {
            Description = "Search recursively through subaccounts."
        };

        _accountCommand = new Command("account", "Get value of named account.")
        {
            _accountNameArgument,
            _recursiveOption
        };

    }

    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Gnucash database query tool")
        {
            _dbOption,
            _startDateOption,
            _endDateOption
        };

        rootCommand.Subcommands.Add(_accountCommand);
        _accountCommand.SetAction(ProcessAccount);
        var result = rootCommand.Parse(args);
        var ret = await result.InvokeAsync();
        _db?.Dispose();
        return ret;
    }

    private static void processGlobalOptions(ParseResult result)
    {
        _dbFile = result.GetValue(_dbOption);
        if (_dbFile is null)
            throw new NullReferenceException("The Gnucash database file was not found.");
        
        _db = new GncContext(_dbFile.FullName);
        
        _startDate = result.GetValue(_startDateOption);
        _endDate = result.GetValue(_endDateOption);
    }

    private static void ProcessAccount(ParseResult result)
    {
        Console.WriteLine("inside ProcessOptions");
        processGlobalOptions(result);

        var accountName = result.GetValue(_accountNameArgument);
        var isRecursive = result.GetValue(_recursiveOption);

        var gncAccount = _db.AccountFromAbsolutePath(accountName);
        var usdCurrency = _db.GetUsdCurrency();
        var amt = _db.GetAccountValueChange(gncAccount, _startDate, _endDate, isRecursive, usdCurrency);
        //Console.WriteLine($"{amt.ToDecimal():F2}");
        Console.WriteLine(amt.ToUsdString());
    }
}
