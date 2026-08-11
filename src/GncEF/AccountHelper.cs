using GncEF.Models;
using Microsoft.EntityFrameworkCore;

namespace GncEF
{
	public static class AccountHelper
	{
		public static GncAccount AccountFromAbsolutePath(this GncContext context, string accountPath)
		{
			var systemRootaccount = context.Accounts.FirstOrDefault(a => a.AccountType == AccountType.ROOT);
			return context.AccountFromRelativePath(systemRootaccount, accountPath);
		}

		public static GncAccount AccountFromRelativePath(this GncContext context, GncAccount rootAccount, string relativePath)
		{
			if (string.IsNullOrEmpty(relativePath))
				return rootAccount;

			var accountNameSegments = relativePath.Split(':');
			var account = rootAccount;

			foreach (var accountName in accountNameSegments)
			{
				if (account == null)
					break;

				account = context.Accounts.FirstOrDefault(a => a.Name == accountName && a.ParentAccount == account);
			}

			return account;
		}

		public static IEnumerable<GncAccount> AccountRecursive(this GncContext db, string rootAccountPath, bool matchingCommodityOnly = true)
		{
			var accounts = new List<GncAccount>();
			var rootAccount = db.AccountFromAbsolutePath(rootAccountPath);
			if (rootAccount == null)
				return Array.Empty<GncAccount>();

			accounts.Add(rootAccount);

			var commodityFilter = matchingCommodityOnly ? rootAccount.Commodity : null;
			db.getChildAccounts(rootAccount, accounts, commodityFilter);

			return accounts;
		}

		private static void getChildAccounts(this GncContext db, GncAccount parentAccount, List<GncAccount> accounts, GncCommodity commodityFilter = null)
		{
			if (parentAccount == null)
				return;

			foreach (var account in parentAccount.ChildAccounts.ToArray())
			{
				if (commodityFilter != null && account.Commodity != commodityFilter)
					continue;
				
				accounts.Add(account);
				db.getChildAccounts(account, accounts);
			}
		}

		public static string AccountIdFromAbsolutePath(this GncContext context, string accountPath)
		{
			if (string.IsNullOrWhiteSpace(accountPath))
				return null;

			var parentAccountId = context.Accounts.Single(a => a.AccountType == AccountType.ROOT && a.Name == "Root Account").AccountId;
			var accountNameSegments = accountPath.Split(':');
			string accountId = null;
			foreach (var name in accountNameSegments)
			{
				accountId = context.Accounts.SingleOrDefault(a => a.ParentGuid == parentAccountId && a.Name == name)?.AccountId;
				if (accountId == null)
					break;

				parentAccountId = accountId;
			}

			return accountId;
		}

		/// <summary>
		/// Get an account's value
		/// </summary>
		/// <param name="context">Database context</param>
		/// <param name="account">Account to inspect</param>
		/// <param name="asOfDate">As of this date, or null to look at all transactions</param>
		/// <returns></returns>
		public static Ratio GetAccountValue(this GncContext context, GncAccount account, DateOnly? asOfDate = null, bool includeSubaccounts = false)
		{
			// Current value is the amount of change from before the first transaction (0) to the given date
			return context.GetAccountValueChange(account, null, asOfDate, includeSubaccounts);
		}

		/// <summary>
		/// Get the change in value for an account for a given date range
		/// </summary>
		/// <param name="context">Database context</param>
		/// <param name="account">Account to inspect</param>
		/// <param name="startDate">Beginning of date range, or null to start with the first transaction</param>
		/// <param name="endDate">End of date range, or null to end with the last transaction</param>
		/// <returns></returns>
		public static Ratio GetAccountValueChange(this GncContext context, GncAccount account, DateOnly? startDate = null, DateOnly? endDate = null, bool includeSubaccounts = false)
		{
			DateTime? start = startDate.HasValue ? new DateTime(startDate.Value, new TimeOnly(0,0,0)) : null;
			DateTime? end = endDate.HasValue ? new DateTime(endDate.Value.AddDays(1), new TimeOnly(0,0,0)) : null;

			var filteredSplits = context.Splits
				.Where(s => s.AccountId == account.AccountId &&
					(!start.HasValue || s.Transaction.PostDate >= start) &&
					(!end.HasValue || s.Transaction.PostDate < end))
				.OrderBy(s => s.Transaction.PostDate)
				.AsNoTracking();

			var total = new Ratio(0, account.CommodityFraction);
			foreach (var s in filteredSplits)
				total += s.ValueRatio;

			if (includeSubaccounts)
			{
				foreach (var childAccount in account.ChildAccounts.Where(a => a.Commodity == account.Commodity))
					total += context.GetAccountValueChange(childAccount, startDate, endDate, true);
			}

			return total;
		}
	}

	public static class AccountType
	{
		public const string ROOT = "ROOT";
		public const string CASH = "CASH";
		public const string BANK = "BANK";
		public const string ASSET = "ASSET";
		public const string LIABILITY = "LIABILITY";
		public const string CREDIT = "CREDIT";
		public const string EXPENSE = "EXPENSE";
		public const string INCOME = "INCOME";
		public const string MUTUALFUND = "MUTUAL";
		public const string STOCK = "STOCK";
		public const string EQUITY = "EQUITY";
	}

	public static class Action
	{
		public const string BUY = "Buy";
		public const string SELL = "Sell";
		public const string FEE = "Fee";
	}
}
