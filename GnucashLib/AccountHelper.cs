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
	}
}
