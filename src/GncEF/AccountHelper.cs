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

		public static IEnumerable<GncAccount> AccountRecursive(this GncContext db, string rootAccountPath)
		{
			var accounts = new List<GncAccount>();
			var rootAccount = db.AccountFromAbsolutePath(rootAccountPath);
			if (rootAccount == null)
				return Array.Empty<GncAccount>();

			accounts.Add(rootAccount);
			db.getChildAccounts(rootAccount, accounts);

			return accounts;
		}

		private static void getChildAccounts(this GncContext db, GncAccount parentAccount, List<GncAccount> accounts)
		{
			if (parentAccount == null)
				return;

			foreach (var account in parentAccount.ChildAccounts.ToArray())
			{
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

		public static (long num, long denom) GetAccountValue(this GncContext context, GncAccount account, DateOnly? asOfDate = null)
		{
			return context.GetAccountValueChange(account, null, asOfDate);
		}

		public static (long num, long denom) GetAccountValueChange(this GncContext context, GncAccount account, DateOnly? startDate = null, DateOnly? endDate = null)
		{
			DateTime? start = startDate.HasValue ? new DateTime(startDate.Value, new TimeOnly(0,0,0)) : null;
			DateTime? end = endDate.HasValue ? new DateTime(endDate.Value.AddDays(1), new TimeOnly(0,0,0)) : null;

			var filteredSplits = context.Splits
				.Where(s => s.AccountId == account.AccountId &&
					(!start.HasValue || s.Transaction.PostDate >= start) &&
					(!end.HasValue || s.Transaction.PostDate < end))
				.OrderBy(s => s.Transaction.PostDate)
				.AsNoTracking();

			(long num, long denom) total = (0, account.CommodityFraction);
			foreach (var s in filteredSplits)
				total = AmountHelper.Add(total, (s.ValueNumerator, s.ValueDenominator));

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
