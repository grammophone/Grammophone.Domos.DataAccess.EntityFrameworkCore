using Grammophone.DataAccess;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Accounting;
using Grammophone.Domos.Domain.Workflow;
using Microsoft.EntityFrameworkCore;

namespace Grammophone.Domos.DataAccess.EntityFrameworkCore
{
	/// <summary>
	/// Entity Framework Core implementation of a Domos repository,
	/// containing users, roles, accounting, workflow, managers and permissions.
	/// </summary>
	public abstract class EFCoreDomosDomainContainer<U, BST, P, R, J> : EFCoreWorkflowUsersDomainContainer<U, BST>
		where U : User
		where BST : StateTransition<U>
		where P : Posting<U>
		where R : Remittance<U>
		where J : Journal<U, BST, P, R>
	{
		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		public EFCoreDomosDomainContainer(DbContextOptions options, bool useChangeTracking = true)
			: base(options, useChangeTracking)
		{
		}

		/// <summary>
		/// Create.
		/// </summary>
		public EFCoreDomosDomainContainer(DbContextOptions options, TransactionMode transactionMode, bool useChangeTracking = true)
			: base(options, transactionMode, useChangeTracking)
		{
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Entity set of accounts in the system.
		/// </summary>
		public DbSet<Account> Accounts { get; set; }

		/// <summary>
		/// Entity set of credit systems in the system.
		/// </summary>
		public DbSet<CreditSystem> CreditSystems { get; set; }

		/// <summary>
		/// Entity set of accounting journals in the system.
		/// </summary>
		public DbSet<J> Journals { get; set; }

		/// <summary>
		/// Entity set of the accounting postings in the system.
		/// </summary>
		public DbSet<P> Postings { get; set; }

		/// <summary>
		/// Entity set of the accounting remittances in the system.
		/// </summary>
		public DbSet<R> Remittances { get; set; }

		/// <summary>
		/// The Electronic Funds Transfer (EFT/ACH) requests in the system.
		/// </summary>
		public DbSet<FundsTransferRequest> FundsTransferRequests { get; set; }

		/// <summary>
		/// The events taking place for funds transfer requests in the system.
		/// </summary>
		public DbSet<FundsTransferEvent> FundsTransferEvents { get; set; }

		/// <summary>
		/// Batches of funds transfer requests.
		/// </summary>
		public DbSet<FundsTransferBatch> FundsTransferBatches { get; set; }

		/// <summary>
		/// Messages recording the history of funds transfer batches.
		/// </summary>
		public DbSet<FundsTransferBatchMessage> FundsTransferBatchMessages { get; set; }

		/// <summary>
		/// The set of funds transfer request groups in the system.
		/// </summary>
		public DbSet<FundsTransferRequestGroup> FundsTransferRequestGroups { get; set; }

		#endregion

		#region Protected methods

		/// <inheritdoc/>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region CreditSystem

			modelBuilder.Entity<CreditSystem>()
				.HasIndex(cs => cs.CodeName)
				.IsUnique()
				.HasDatabaseName("IX_CreditSystem_CodeName");

			#endregion

			#region Journal

			modelBuilder.Entity<J>()
				.HasIndex(p => p.CreationDate)
				.HasDatabaseName("IX_Journal_CreationDate");

			#endregion

			#region Posting

			modelBuilder.Entity<J>()
				.HasMany(j => j.Postings)
				.WithOne()
				.HasForeignKey(p => p.JournalID);

			modelBuilder.Entity<P>()
				.HasIndex(p => p.CreationDate)
				.HasDatabaseName("IX_Posting_CreationDate");

			#endregion

			#region Remittance

			modelBuilder.Entity<J>()
				.HasMany(j => j.Remittances)
				.WithOne()
				.HasForeignKey(p => p.JournalID);

			modelBuilder.Entity<R>()
				.HasIndex(r => new { r.TransactionID, r.CreditSystemID })
				.IsUnique()
				.HasDatabaseName("IX_Remittance_TransactionID_CreditSystemID");

			modelBuilder.Entity<R>()
				.HasIndex(r => r.BatchID)
				.HasDatabaseName("IX_Remittance_BatchID");

			modelBuilder.Entity<R>()
				.HasIndex(r => r.CreationDate)
				.HasDatabaseName("IX_Remittance_CreationDate");

			#endregion

			#region FundsTransferEvent

			modelBuilder.Entity<FundsTransferEvent>()
				.HasIndex(fte => fte.TraceCode)
				.HasDatabaseName("IX_FundsTransferEvent_TraceCode");

			modelBuilder.Entity<FundsTransferEvent>()
				.HasIndex(fte => fte.ResponseCode)
				.HasDatabaseName("IX_FundsTransferEvent_ResponseCode");

			modelBuilder.Entity<FundsTransferEvent>()
				.HasIndex(fte => new { fte.Type, fte.ResponseCode })
				.HasDatabaseName("IX_FundsTransferEvent_Type_ResponseCode");

			modelBuilder.Entity<FundsTransferEvent>()
				.HasIndex(fte => fte.Time)
				.HasDatabaseName("IX_FundsTransferEvent_Time");

			#endregion

			#region FundsTransferRequest

			modelBuilder.Entity<FundsTransferRequest>()
				.HasIndex(ftr => ftr.GUID)
				.IsUnique()
				.HasDatabaseName("IX_FundsTransferRequest_GUID");

			modelBuilder.Entity<FundsTransferRequest>()
				.HasIndex(ftr => new { ftr.BatchID, ftr.GroupID })
				.HasDatabaseName("IX_FundsTransferRequest_BatchID_GroupID");

			modelBuilder.Entity<FundsTransferRequest>()
				.HasIndex(ftr => new { ftr.Category, ftr.CreationDate });

			modelBuilder.Entity<FundsTransferRequest>()
				.HasIndex(ftr => ftr.CreationDate);

			#endregion

			#region FundsTransferRequestGroup

			modelBuilder.Entity<FundsTransferRequestGroup>()
				.ComplexProperty(ftrg => ftrg.EncryptedBankAccountInfo).IsRequired();

			// Composite index (including owner properties)
			/* The following has no way to work in EF Core. Make a migration to add it */
			//modelBuilder.Entity<FundsTransferRequestGroup>()
			//	.HasIndex(new string[]
			//	{
			//		"EncryptedBankAccountInfo_EncryptedAccountNumber",   // Use the actual column name
			//		"EncryptedBankAccountInfo_EncryptedTransitNumber",
			//		"EncryptedBankAccountInfo_Type",
			//		"EncryptedBankAccountInfo_BankNumber",
			//		"EncryptedBankAccountInfo_AccountCode",
			//		"AccountHolderName",
			//		"AccountHolderToken",
			//		"EffectiveDate"
			//	})
			//	.HasDatabaseName("IX_FundsTransferRequestGroup_EncryptedBankingInfo_AccountHolder");

			#endregion

			#region FundsTransferBatch

			modelBuilder.Entity<FundsTransferBatch>()
				.HasIndex(ftb => ftb.CreationDate)
				.HasDatabaseName("IX_FundsTransferBatch_CreationDate");

			modelBuilder.Entity<FundsTransferBatch>()
				.HasIndex(ftb => ftb.GUID)
				.IsUnique()
				.HasDatabaseName("IX_FundsTransferBatch_GIUD");

			#endregion

			#region FundsTransferBatchMessage

			modelBuilder.Entity<FundsTransferBatchMessage>()
				.HasIndex(ftbm => ftbm.Time)
				.HasDatabaseName("IX_FundsTransferBatchMessage_Time");

			modelBuilder.Entity<FundsTransferBatchMessage>()
				.HasIndex(ftbm => ftbm.MessageCode)
				.HasDatabaseName("IX_FundsTransferBatchMessage_MessageCode");

			modelBuilder.Entity<FundsTransferBatchMessage>()
				.HasIndex(ftbm => ftbm.GUID)
				.IsUnique()
				.HasDatabaseName("IX_FundsTransferBatchMessage_GUID");

			#endregion

			#region StateTransition

			/* Restore the funds transfer event relation which was ignored by the base implementation, as this domain can support accounting entities. */
			modelBuilder.Entity<BST>()
				.HasOne(st => st.FundsTransferEvent)
				.WithMany()
				.HasForeignKey(st => st.FundsTransferEventID)
				.IsRequired(false);

			#endregion
		}

		#endregion
	}

	/// <summary>
	/// Entity Framework Core implementation of a Domos repository with invoicing.
	/// </summary>
	public abstract class EFCoreDomosDomainContainer<U, BST, P, R, J, ILTC, IL, IE, I> :
		EFCoreDomosDomainContainer<U, BST, P, R, J>
		where U : User
		where BST : StateTransition<U>
		where P : Posting<U>
		where R : Remittance<U>
		where J : Journal<U, BST, P, R>
		where ILTC : InvoiceLineTaxComponent<U, P, R>
		where IL : InvoiceLine<U, P, R, ILTC>
		where IE : InvoiceEvent<U, P, R>
		where I : Invoice<U, P, R, ILTC, IL, IE>
	{
		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		public EFCoreDomosDomainContainer(DbContextOptions options, bool useChangeTracking = true)
			: base(options, useChangeTracking)
		{
		}

		/// <summary>
		/// Create.
		/// </summary>
		public EFCoreDomosDomainContainer(DbContextOptions options, TransactionMode transactionMode, bool useChangeTracking = true)
			: base(options, transactionMode, useChangeTracking)
		{
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The set of invoices in the system.
		/// </summary>
		public DbSet<I> Invoices { get; set; }

		/// <summary>
		/// The set of invoice events in the system.
		/// </summary>
		public DbSet<IE> InvoiceEvents { get; set; }

		/// <summary>
		/// The set of invoice lines in the system.
		/// </summary>
		public DbSet<IL> InvoiceLines { get; set; }

		/// <summary>
		/// The set of invoice line tax components in the system.
		/// </summary>
		public DbSet<ILTC> InvoiceLineTaxComponents { get; set; }

		#endregion

		#region Protected methods

		/// <inheritdoc/>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region Invoice

			modelBuilder.Entity<I>()
				.HasIndex(i => i.IssueDate)
				.HasDatabaseName("IX_Invoice_IssueDate");

			modelBuilder.Entity<I>()
				.HasIndex(i => i.DueDate)
				.HasDatabaseName("IX_Invoice_DueDate");

			modelBuilder.Entity<I>()
				.HasMany(i => i.ServicingFundsTransferRequests)
				.WithMany();

			modelBuilder.Entity<I>()
				.HasMany(i => i.Lines)
				.WithOne()
				.HasForeignKey(l => l.InvoiceID)
				.OnDelete(DeleteBehavior.Cascade);

			#endregion

			#region InvoiceLine

			modelBuilder.Entity<IL>()
				.HasMany(il => il.TaxComponents)
				.WithOne()
				.HasForeignKey(iltc => iltc.LineID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<IL>()
				.Property(il => il.Rate)
				.HasPrecision(19, 3);

			modelBuilder.Entity<IL>()
				.Property(il => il.Quantity)
				.HasPrecision(19, 6);

			#endregion

			#region InvoiceLineTaxComponent

			modelBuilder.Entity<ILTC>()
				.Property(iltc => iltc.RatePercentFactor)
				.HasPrecision(5, 3);

			#endregion

			#region InvoiceEvent

			modelBuilder.Entity<IE>()
				.HasIndex(ie => ie.Time)
				.HasDatabaseName("IX_InvoiceEvent_Time");

			modelBuilder.Entity<IE>()
				.HasIndex(ie => ie.InvoiceState)
				.HasDatabaseName("IX_InvoiceEvent_InvoiceState");

			#endregion
		}

		#endregion
	}
}
