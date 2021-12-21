using System;
using System.Collections.Generic;
using System.Linq;
using GnucashLib.Models;
using Microsoft.EntityFrameworkCore;

namespace GnucashLib
{
	public static class AccountHelper
	{
		public static Account AccountFromAbsolutePath(this GnucashContext context, string accountPath)
		{
			var s = context.Accounts.Where(a => a.AccountType == AccountType.ROOT && a.Name == "Root Account");

			var systemRootaccount = context.Accounts.FirstOrDefault(a => a.AccountType == AccountType.ROOT);
			return context.AccountFromRelativePath(systemRootaccount, accountPath);
		}

		public static Account AccountFromRelativePath(this GnucashContext context, Account rootAccount, string relativePath)
		{
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

		public static IEnumerable<Account> AccountRecursive(this GnucashContext db, string rootAccountPath)
		{
			var accounts = new List<Account>();
			var rootAccount = db.AccountFromAbsolutePath(rootAccountPath);
			if (rootAccount == null)
				return Array.Empty<Account>();

			accounts.Add(rootAccount);
			db.getChildAccounts(rootAccount, accounts);

			return accounts;
		}

		private static void getChildAccounts(this GnucashContext db, Account parentAccount, List<Account> accounts)
		{
			if (parentAccount == null)
				return;

			foreach (var account in parentAccount.ChildAccounts.ToArray())
			{
				accounts.Add(account);
				db.getChildAccounts(account, accounts);
			}
		}

		public static string? AccountIdFromAbsolutePath(this GnucashContext context, string accountPath)
		{
			if (string.IsNullOrWhiteSpace(accountPath))
				return null;

			var s = context.Accounts.Where(a => a.AccountType == AccountType.ROOT);

			var parentAccountId = context.Accounts.Single(a => a.AccountType == AccountType.ROOT && a.Name == "Root Account").AccountId;
			var accountNameSegments = accountPath.Split(':');
			string? accountId = null;
			foreach (var name in accountNameSegments)
			{
				accountId = context.Accounts.SingleOrDefault(a => a.ParentGuid == parentAccountId && a.Name == name)?.AccountId;
				if (accountId == null)
					break;

				parentAccountId = accountId;
			}

			return accountId;
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
