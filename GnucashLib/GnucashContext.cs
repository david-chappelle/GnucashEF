using System;
using GnucashLib.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GnucashLib
{
	public class GnucashContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Commodity> Commodities { get; set; }
		public DbSet<Price> Prices { get; set; }
		public DbSet<Schedule> Schedules { get; set; }
		public DbSet<ScheduledTransaction> ScheduledTransactions { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<Split> Splits { get; set; }
		public DbSet<Lot> Lots { get; set; }

		public string DatabaseFile { get; private set; }

		public GnucashContext(string databaseFile)
		{
			DatabaseFile = databaseFile;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder opt)
		{
			var cs = new SqliteConnectionStringBuilder()
			{
				DataSource = DatabaseFile
			}.ToString();

			opt
				.UseLazyLoadingProxies()
				.UseSqlite(new SqliteConnection(cs));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Account>(e =>
			{
				e.ToTable("accounts");
				e.HasKey(t => t.AccountId);
				e.Property(t => t.AccountId).HasColumnName("guid").IsRequired();
				e.Property(t => t.Name).HasColumnName("name").IsRequired();
				e.Property(t => t.AccountType).HasColumnName("account_type").IsRequired();
				e.Property(t => t.CommodityId).HasColumnName("commodity_guid");
				e.Property(t => t.CommodityFraction).HasColumnName("commodity_scu").IsRequired();
				e.Property(t => t.NonStandardFraction).HasColumnName("non_std_scu").IsRequired();
				e.Property(t => t.ParentGuid).HasColumnName("parent_guid");
				e.Property(t => t.Code).HasColumnName("code");
				e.Property(t => t.Description).HasColumnName("description");
				e.Property(t => t.Hidden).HasColumnName("hidden");
				e.Property(t => t.Placeholder).HasColumnName("placeholder");

				e.HasOne(a => a.Commodity).WithMany().HasForeignKey(a => a.CommodityId);
				e.HasOne(a => a.ParentAccount).WithMany(pa => pa.ChildAccounts).HasForeignKey(a => a.ParentGuid);
			});

			modelBuilder.Entity<Commodity>(e =>
			{
				e.ToTable("commodities");
				e.HasKey(t => t.CommodityId);

				e.Property(t => t.CommodityId).HasColumnName("guid");
				e.Property(t => t.Namespace).HasColumnName("namespace");
				e.Property(t => t.Mnenomic).HasColumnName("mnemonic");
				e.Property(t => t.FullName).HasColumnName("fullname");
				e.Property(t => t.Cusip).HasColumnName("cusip");
				e.Property(t => t.Fraction).HasColumnName("fraction");
				e.Property(t => t.QuoteFlag).HasColumnName("quote_flag");
				e.Property(t => t.QuoteSource).HasColumnName("quote_source");
				e.Property(t => t.QuoteTimezone).HasColumnName("quote_tz");
			});

			modelBuilder.Entity<Price>(e =>
			{
				e.ToTable("prices");
				e.HasKey(p => p.PriceId);

				e.Property(p => p.PriceId).HasColumnName("guid").IsRequired();
				e.Property(p => p.CommodityId).HasColumnName("commodity_guid").IsRequired();
				e.Property(p => p.CurrencyId).HasColumnName("currency_guid").IsRequired();
				e.Property(p => p.Date).HasColumnName("date").IsRequired();
				e.Property(p => p.Source).HasColumnName("source");
				e.Property(p => p.Type).HasColumnName("type");
				e.Property(p => p.ValueNumerator).HasColumnName("value_num").IsRequired();
				e.Property(p => p.ValueDenominator).HasColumnName("value_denom").IsRequired();

				e.HasOne(p => p.Commodity).WithMany().HasForeignKey(p => p.CommodityId);
				e.HasOne(p => p.Currency).WithMany().HasForeignKey(p => p.CurrencyId);
			});

			modelBuilder.Entity<Schedule>(e =>
			{
				e.ToTable("recurrences");
				e.HasKey(s => s.ScheduleId);

				e.Property(s => s.ScheduleId).HasColumnName("id").IsRequired();
				e.Property(s => s.ObjectId).HasColumnName("obj_guid").IsRequired();
				e.Property(s => s.Frequency).HasColumnName("recurrence_mult").IsRequired();
				e.Property(s => s.Period).HasColumnName("recurrence_period_type").IsRequired();
				e.Property(s => s.Start).HasColumnName("recurrence_period_start").IsRequired();
				e.Property(s => s.WeekendAdjustment).HasColumnName("recurrence_weekend_adjust").IsRequired();

				e.HasOne(s => s.ScheduledTransaction).WithMany().HasForeignKey(s => s.ObjectId);
			});

			modelBuilder.Entity<ScheduledTransaction>(e =>
			{
				e.ToTable("schedxactions");
				e.HasKey(s => s.ScheduledTransactionId);

				e.Property(s => s.ScheduledTransactionId).HasColumnName("guid").IsRequired();
				e.Property(s => s.Name).HasColumnName("name");
				e.Property(s => s.Enabled).HasColumnName("enabled").IsRequired();
				e.Property(s => s.StartDate).HasColumnName("start_date");
				e.Property(s => s.EndDate).HasColumnName("end_date");
				e.Property(s => s.LastOccurence).HasColumnName("last_occur");
				e.Property(s => s.NumberOccurences).HasColumnName("num_occur").IsRequired();
				e.Property(s => s.RemainingOccurences).HasColumnName("rem_occur").IsRequired();
				e.Property(s => s.AutoCreate).HasColumnName("auto_create").IsRequired();
				e.Property(s => s.NotifyWhenCreated).HasColumnName("auto_notify").IsRequired();
				e.Property(s => s.AdvancedCreationDays).HasColumnName("adv_creation").IsRequired();
				e.Property(s => s.AdvancedNotifyDays).HasColumnName("adv_notify").IsRequired();
				e.Property(s => s.Count).HasColumnName("instance_count").IsRequired();
				e.Property(s => s.TemplateId).HasColumnName("template_act_guid").IsRequired();
			});

			modelBuilder.Entity<Transaction>(e =>
			{
				e.ToTable("transactions");
				e.HasKey(t => t.TransactionId);

				e.Property(t => t.TransactionId).HasColumnName("guid").IsRequired();
				e.Property(t => t.CurrencyId).HasColumnName("currency_guid").IsRequired();
				e.Property(t => t.Number).HasColumnName("num").IsRequired();
				e.Property(t => t.PostDate).HasColumnName("post_date");
				e.Property(t => t.EnteredDate).HasColumnName("enter_date");
				e.Property(t => t.Description).HasColumnName("description");

				e.HasOne(t => t.Currency).WithMany().HasForeignKey(t => t.CurrencyId);
			});

			modelBuilder.Entity<Split>(e =>
			{
				e.ToTable("splits");
				e.HasKey(s => s.SplitId);

				e.Property(s => s.SplitId).HasColumnName("guid").IsRequired();
				e.Property(s => s.TransactionId).HasColumnName("tx_guid").IsRequired();
				e.Property(s => s.AccountId).HasColumnName("account_guid").IsRequired();
				e.Property(s => s.Memo).HasColumnName("memo").IsRequired();
				e.Property(s => s.ActionName).HasColumnName("action").IsRequired();
				e.Property(s => s.ReconcileState).HasColumnName("reconcile_state").IsRequired();
				e.Property(s => s.ReconcileDate).HasColumnName("reconcile_date");
				e.Property(s => s.ValueNumerator).HasColumnName("value_num").IsRequired();
				e.Property(s => s.ValueDenominator).HasColumnName("value_denom").IsRequired();
				e.Property(s => s.QuantityNumerator).HasColumnName("quantity_num").IsRequired();
				e.Property(s => s.QuantityDenominator).HasColumnName("quantity_denom").IsRequired();
				e.Property(s => s.LotId).HasColumnName("lot_guid");

				e.HasOne(s => s.Transaction).WithMany(t => t.Splits).HasForeignKey(s => s.TransactionId);
				e.HasOne(s => s.Account).WithMany(a => a.Splits).HasForeignKey(s => s.AccountId);
			});

			modelBuilder.Entity<Lot>(e =>
			{
				e.ToTable("lots");
				e.HasKey(l => l.LotId);

				e.Property(l => l.LotId).HasColumnName("guid").IsRequired();
				e.Property(l => l.AccountId).HasColumnName("account_guid");
				e.Property(l => l.IsClosed).HasColumnName("is_closed").IsRequired();
			});
		}
	}
}
