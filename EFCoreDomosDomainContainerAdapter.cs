using Grammophone.DataAccess;
using Grammophone.DataAccess.EntityFrameworkCore;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Accounting;
using Grammophone.Domos.Domain.Workflow;

namespace Grammophone.Domos.DataAccess.EntityFrameworkCore
{
	/// <summary>
	/// Adapts an Entity Framework Core Domos domain container to <see cref="IDomosDomainContainer{U, BST, P, R, J}"/>.
	/// </summary>
	public class EFCoreDomosDomainContainerAdapter<U, BST, P, R, J, D> :
		EFCoreWorkflowUsersDomainContainerAdapter<U, BST, D>,
		IDomosDomainContainer<U, BST, P, R, J>
		where U : User
		where BST : StateTransition<U>
		where P : Posting<U>
		where R : Remittance<U>
		where J : Journal<U, BST, P, R>
		where D : EFCoreDomosDomainContainer<U, BST, P, R, J>
	{
		#region Private fields

		private IEntitySet<Account> accounts;

		private IEntitySet<CreditSystem> creditSystems;

		private IEntitySet<J> journals;

		private IEntitySet<P> postings;

		private IEntitySet<R> remittances;

		private IEntitySet<FundsTransferRequest> fundsTransferRequests;

		private IEntitySet<FundsTransferEvent> fundsTransferEvents;

		private IEntitySet<FundsTransferBatch> fundsTransferBatches;

		private IEntitySet<FundsTransferBatchMessage> fundsTransferBatchMessages;

		private IEntitySet<FundsTransferRequestGroup> fundsTransferRequestGroups;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="innerContainer">The adapted EF Core domain container.</param>
		public EFCoreDomosDomainContainerAdapter(D innerContainer)
			: base(innerContainer)
		{
		}

		#endregion

		#region IDomosDomainContainer implementation

		/// <inheritdoc/>
		public IEntitySet<Account> Accounts => accounts ??= new EFCoreSet<Account>(this.InnerDomainContainer.Accounts, this);

		/// <inheritdoc/>
		public IEntitySet<CreditSystem> CreditSystems => creditSystems ??= new EFCoreSet<CreditSystem>(this.InnerDomainContainer.CreditSystems, this);

		/// <inheritdoc/>
		public IEntitySet<J> Journals => journals ??= new EFCoreSet<J>(this.InnerDomainContainer.Journals, this);

		/// <inheritdoc/>
		public IEntitySet<P> Postings => postings ??= new EFCoreSet<P>(this.InnerDomainContainer.Postings, this);

		/// <inheritdoc/>
		public IEntitySet<R> Remittances => remittances ??= new EFCoreSet<R>(this.InnerDomainContainer.Remittances, this);

		/// <inheritdoc/>
		public IEntitySet<FundsTransferRequest> FundsTransferRequests => fundsTransferRequests ??= new EFCoreSet<FundsTransferRequest>(this.InnerDomainContainer.FundsTransferRequests, this);

		/// <inheritdoc/>
		public IEntitySet<FundsTransferEvent> FundsTransferEvents => fundsTransferEvents ??= new EFCoreSet<FundsTransferEvent>(this.InnerDomainContainer.FundsTransferEvents, this);

		/// <inheritdoc/>
		public IEntitySet<FundsTransferBatch> FundsTransferBatches => fundsTransferBatches ??= new EFCoreSet<FundsTransferBatch>(this.InnerDomainContainer.FundsTransferBatches, this);

		/// <inheritdoc/>
		public IEntitySet<FundsTransferBatchMessage> FundsTransferBatchMessages => fundsTransferBatchMessages ??= new EFCoreSet<FundsTransferBatchMessage>(this.InnerDomainContainer.FundsTransferBatchMessages, this);

		/// <inheritdoc/>
		public IEntitySet<FundsTransferRequestGroup> FundsTransferRequestGroups => fundsTransferRequestGroups ??= new EFCoreSet<FundsTransferRequestGroup>(this.InnerDomainContainer.FundsTransferRequestGroups, this);

		#endregion
	}

	/// <summary>
	/// Adapts an Entity Framework Core Domos domain container with invoicing to
	/// <see cref="IDomosDomainContainer{U, BST, P, R, J, ILTC, IL, IE, I}"/>.
	/// </summary>
	public class EFCoreDomosDomainContainerAdapter<U, BST, P, R, J, ILTC, IL, IE, I, D> :
		EFCoreDomosDomainContainerAdapter<U, BST, P, R, J, D>,
		IDomosDomainContainer<U, BST, P, R, J, ILTC, IL, IE, I>
		where U : User
		where BST : StateTransition<U>
		where P : Posting<U>
		where R : Remittance<U>
		where J : Journal<U, BST, P, R>
		where ILTC : InvoiceLineTaxComponent<U, P, R>
		where IL : InvoiceLine<U, P, R, ILTC>
		where IE : InvoiceEvent<U, P, R>
		where I : Invoice<U, P, R, ILTC, IL, IE>
		where D : EFCoreDomosDomainContainer<U, BST, P, R, J, ILTC, IL, IE, I>
	{
		#region Private fields

		private IEntitySet<I> invoices;

		private IEntitySet<IE> invoiceEvents;

		private IEntitySet<IL> invoiceLines;

		private IEntitySet<ILTC> invoiceLineTaxComponents;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="innerContainer">The adapted EF Core domain container.</param>
		public EFCoreDomosDomainContainerAdapter(D innerContainer)
			: base(innerContainer)
		{
		}

		#endregion

		#region IDomosDomainContainer implementation

		/// <inheritdoc/>
		public IEntitySet<I> Invoices => invoices ??= new EFCoreSet<I>(this.InnerDomainContainer.Invoices, this);

		/// <inheritdoc/>
		public IEntitySet<IE> InvoiceEvents => invoiceEvents ??= new EFCoreSet<IE>(this.InnerDomainContainer.InvoiceEvents, this);

		/// <inheritdoc/>
		public IEntitySet<IL> InvoiceLines => invoiceLines ??= new EFCoreSet<IL>(this.InnerDomainContainer.InvoiceLines, this);

		/// <inheritdoc/>
		public IEntitySet<ILTC> InvoiceLineTaxComponents => invoiceLineTaxComponents ??= new EFCoreSet<ILTC>(this.InnerDomainContainer.InvoiceLineTaxComponents, this);

		#endregion
	}
}
