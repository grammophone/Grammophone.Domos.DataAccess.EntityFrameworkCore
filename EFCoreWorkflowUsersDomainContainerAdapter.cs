using Grammophone.DataAccess;
using Grammophone.DataAccess.EntityFrameworkCore;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Workflow;

namespace Grammophone.Domos.DataAccess.EntityFrameworkCore
{
	/// <summary>
	/// Adapts an Entity Framework Core workflow users domain container to <see cref="IWorkflowUsersDomainContainer{U, BST}"/>.
	/// </summary>
	public class EFCoreWorkflowUsersDomainContainerAdapter<U, BST, D> : EFCoreUsersDomainContainerAdapter<U, D>, IWorkflowUsersDomainContainer<U, BST>
		where U : User
		where BST : StateTransition<U>
		where D : EFCoreWorkflowUsersDomainContainer<U, BST>
	{
		#region Private fields

		private IEntitySet<State> states;

		private IEntitySet<StateGroup> stateGroups;

		private IEntitySet<StatePath> statePaths;

		private IEntitySet<BST> stateTransitions;

		private IEntitySet<WorkflowGraph> workflowGraphs;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="innerContainer">The adapted EF Core domain container.</param>
		public EFCoreWorkflowUsersDomainContainerAdapter(D innerContainer)
			: base(innerContainer)
		{
		}

		#endregion

		#region IWorkflowUsersDomainContainer implementation

		/// <inheritdoc/>
		public IEntitySet<State> States => states ??= new EFCoreSet<State>(this.InnerDomainContainer.States, this);

		/// <inheritdoc/>
		public IEntitySet<StateGroup> StateGroups => stateGroups ??= new EFCoreSet<StateGroup>(this.InnerDomainContainer.StateGroups, this);

		/// <inheritdoc/>
		public IEntitySet<StatePath> StatePaths => statePaths ??= new EFCoreSet<StatePath>(this.InnerDomainContainer.StatePaths, this);

		/// <inheritdoc/>
		public IEntitySet<BST> StateTransitions => stateTransitions ??= new EFCoreSet<BST>(this.InnerDomainContainer.StateTransitions, this);

		/// <inheritdoc/>
		public IEntitySet<WorkflowGraph> WorkflowGraphs => workflowGraphs ??= new EFCoreSet<WorkflowGraph>(this.InnerDomainContainer.WorkflowGraphs, this);

		#endregion
	}
}
