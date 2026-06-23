using Grammophone.DataAccess;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Workflow;
using Microsoft.EntityFrameworkCore;

namespace Grammophone.Domos.DataAccess.EntityFrameworkCore
{
	/// <summary>
	/// Entity Framework Core implementation of a Domos repository
	/// containing users, roles, workflow, managers and permissions.
	/// </summary>
	public class EFCoreWorkflowUsersDomainContainer<U, BST> : EFCoreUsersDomainContainer<U>
		where U : User
		where BST : StateTransition<U>
	{
		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		public EFCoreWorkflowUsersDomainContainer(DbContextOptions options, bool useChangeTracking = true)
			: base(options, useChangeTracking)
		{
		}

		/// <summary>
		/// Create.
		/// </summary>
		public EFCoreWorkflowUsersDomainContainer(DbContextOptions options, TransactionMode transactionMode, bool useChangeTracking = true)
			: base(options, transactionMode, useChangeTracking)
		{
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Entity set of workflow states in the system.
		/// </summary>
		public DbSet<State> States { get; set; }

		/// <summary>
		/// Entity set of workflow state groups in the system.
		/// </summary>
		public DbSet<StateGroup> StateGroups { get; set; }

		/// <summary>
		/// Entity set of workflow state paths in the system.
		/// </summary>
		public DbSet<StatePath> StatePaths { get; set; }

		/// <summary>
		/// Entity set of transitions occurred between workflow states in the system.
		/// </summary>
		public DbSet<BST> StateTransitions { get; set; }

		/// <summary>
		/// Entity set of workflow graphs in the system.
		/// </summary>
		public DbSet<WorkflowGraph> WorkflowGraphs { get; set; }

		#endregion

		#region Protected methods

		/// <inheritdoc/>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region WorkflowGraph

			modelBuilder.Entity<WorkflowGraph>()
				.HasIndex(wg => wg.CodeName)
				.IsUnique();

			#endregion

			#region StateGroup

			//modelBuilder.Entity<StateGroup>()
			//	.HasIndex(sg => new { sg.WorkflowGraphID, sg.CodeName })
			//	.IsUnique();

			modelBuilder.Entity<StateGroup>()
				.HasIndex(sg => sg.CodeName);

			#endregion

			#region State

			//modelBuilder.Entity<State>()
			//	.HasIndex(s => new { s.GroupID, s.CodeName })
			//	.IsUnique();

			modelBuilder.Entity<State>()
				.HasIndex(s => s.CodeName);

			#endregion

			#region StatePath

			modelBuilder.Entity<StatePath>()
				.HasIndex(sp => sp.CodeName)
				.IsUnique()
				.HasDatabaseName("IX_StatePath_CodeName");

			#endregion
		}

		#endregion
	}
}
