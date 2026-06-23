using Grammophone.DataAccess;
using Grammophone.DataAccess.EntityFrameworkCore;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Files;

namespace Grammophone.Domos.DataAccess.EntityFrameworkCore
{
	/// <summary>
	/// Adapts an Entity Framework Core users domain container to <see cref="IUsersDomainContainer{U}"/>.
	/// </summary>
	public class EFCoreUsersDomainContainerAdapter<U, D> : EFCoreDomainContainerAdapter<D>, IUsersDomainContainer<U>
		where U : User
		where D : EFCoreUsersDomainContainer<U>
	{
		#region Private fields

		private IEntitySet<U> users;

		private IEntitySet<Registration> registrations;

		private IEntitySet<Role> roles;

		private IEntitySet<Disposition> dispositions;

		private IEntitySet<ContentType> contentTypes;

		private IEntitySet<DispositionType> dispositionTypes;

		private IEntitySet<WebAuthnCredential> webAuthnCredentials;

		private IEntitySet<BrowserSession> browserSessions;

		private IEntitySet<ClientIpAddress> clientIpAddresses;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="innerContainer">The adapted EF Core domain container.</param>
		public EFCoreUsersDomainContainerAdapter(D innerContainer)
			: base(innerContainer)
		{
		}

		#endregion

		#region IUsersDomainContainer implementation

		/// <inheritdoc/>
		public IEntitySet<U> Users => users ??= new EFCoreSet<U>(this.InnerDomainContainer.Users, this);

		/// <inheritdoc/>
		public IEntitySet<Registration> Registrations => registrations ??= new EFCoreSet<Registration>(this.InnerDomainContainer.Registrations, this);

		/// <inheritdoc/>
		public IEntitySet<Role> Roles => roles ??= new EFCoreSet<Role>(this.InnerDomainContainer.Roles, this);

		/// <inheritdoc/>
		public IEntitySet<Disposition> Dispositions => dispositions ??= new EFCoreSet<Disposition>(this.InnerDomainContainer.Dispositions, this);

		/// <inheritdoc/>
		public IEntitySet<ContentType> ContentTypes => contentTypes ??= new EFCoreSet<ContentType>(this.InnerDomainContainer.ContentTypes, this);

		/// <inheritdoc/>
		public IEntitySet<DispositionType> DispositionTypes => dispositionTypes ??= new EFCoreSet<DispositionType>(this.InnerDomainContainer.DispositionTypes, this);

		/// <inheritdoc/>
		public IEntitySet<WebAuthnCredential> WebAuthnCredentials => webAuthnCredentials ??= new EFCoreSet<WebAuthnCredential>(this.InnerDomainContainer.WebAuthnCredentials, this);

		/// <inheritdoc/>
		public IEntitySet<BrowserSession> BrowserSessions => browserSessions ??= new EFCoreSet<BrowserSession>(this.InnerDomainContainer.BrowserSessions, this);

		/// <inheritdoc/>
		public IEntitySet<ClientIpAddress> ClientIpAddresses => clientIpAddresses ??= new EFCoreSet<ClientIpAddress>(this.InnerDomainContainer.ClientIpAddresses, this);

		#endregion
	}
}
