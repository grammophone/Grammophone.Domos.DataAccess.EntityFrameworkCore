using Grammophone.DataAccess;
using Grammophone.DataAccess.EntityFrameworkCore;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Files;
using Microsoft.EntityFrameworkCore;

namespace Grammophone.Domos.DataAccess.EntityFrameworkCore
{
	/// <summary>
	/// Entity Framework Core implementation of a Domos repository
	/// containing users, roles, managers and permissions.
	/// </summary>
	/// <typeparam name="U">The type of users. Must be derived from <see cref="User"/>.</typeparam>
	public class EFCoreUsersDomainContainer<U> : EFCoreDomainContainer
		where U : User
	{
		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="options">The context options.</param>
		/// <param name="useChangeTracking">If true, enable EF Core change-tracking proxies.</param>
		public EFCoreUsersDomainContainer(DbContextOptions options, bool useChangeTracking = true)
			: base(options, useChangeTracking)
		{
		}

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="options">The context options.</param>
		/// <param name="transactionMode">The transaction behavior.</param>
		/// <param name="useChangeTracking">If true, enable EF Core change-tracking proxies.</param>
		public EFCoreUsersDomainContainer(DbContextOptions options, TransactionMode transactionMode, bool useChangeTracking = true)
			: base(options, transactionMode, useChangeTracking)
		{
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Entity set of users in the system.
		/// </summary>
		public DbSet<U> Users { get; set; }

		/// <summary>
		/// Entity set of registrations in the system.
		/// </summary>
		public DbSet<Registration> Registrations { get; set; }

		/// <summary>
		/// Entity set of roles in the system.
		/// </summary>
		public DbSet<Role> Roles { get; set; }

		/// <summary>
		/// Entity set of dispositions in the system.
		/// </summary>
		public DbSet<Disposition> Dispositions { get; set; }

		/// <summary>
		/// The MIME content types in the system.
		/// </summary>
		public DbSet<ContentType> ContentTypes { get; set; }

		/// <summary>
		/// The disposition types in the system.
		/// </summary>
		public DbSet<DispositionType> DispositionTypes { get; set; }

		/// <summary>
		/// The WebAuthn users' credentials stored in the system.
		/// </summary>
		public DbSet<WebAuthnCredential> WebAuthnCredentials { get; set; }

		/// <summary>
		/// The browser sessions of the users.
		/// </summary>
		public DbSet<BrowserSession> BrowserSessions { get; set; }

		/// <summary>
		/// The IP addresses of clients of the application.
		/// </summary>
		public DbSet<ClientIpAddress> ClientIpAddresses { get; set; }

		#endregion

		#region Protected methods

		/// <inheritdoc/>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region User

			modelBuilder.Entity<U>()
				.HasIndex(u => u.Email)
				.IsUnique()
				.HasDatabaseName("IX_User_Email");

			modelBuilder.Entity<U>()
				.HasIndex(u => u.UserName)
				.IsUnique()
				.HasDatabaseName("IX_User_UserName");

			modelBuilder.Entity<U>()
				.HasIndex(u => u.CreationDate)
				.HasDatabaseName("IX_User_CreationDate");

			modelBuilder.Entity<User>()
				.HasMany(u => u.Roles)
				.WithMany()
				.UsingEntity(j => j.ToTable("UsersToRoles"));

			modelBuilder.Entity<User>()
				.HasMany(u => u.Dispositions)
				.WithOne(d => d.OwningUser)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<User>()
				.HasMany(u => u.Registrations)
				.WithOne(r => r.User)
				.OnDelete(DeleteBehavior.Cascade);

			#endregion

			#region Dispositions

			modelBuilder.Entity<Disposition>()
				.HasOne(d => d.CreatorUser)
				.WithMany();

			modelBuilder.Entity<Disposition>()
				.HasOne(d => d.LastModifierUser)
				.WithMany();

			#endregion

			#region Registration

			modelBuilder.Entity<Registration>()
				.HasIndex(r => new { r.Provider, r.ProviderKey })
				.IsUnique()
				.HasDatabaseName("IX_Registration_Provider_ProviderKey");

			modelBuilder.Entity<Registration>()
				.Property(r => r.ProviderKey)
				.HasMaxLength(128);

			#endregion

			#region ContentType

			modelBuilder.Entity<ContentType>()
				.HasIndex(ct => ct.MIME)
				.IsUnique()
				.HasDatabaseName("IX_ContentType_MIME");

			#endregion

			#region File

			// File is abstract; derive from it and define your own entity set.
			modelBuilder.Ignore<File>();

			#endregion

			#region WebAuthnCredential

			modelBuilder.Entity<WebAuthnCredential>()
				.HasIndex(c => c.UserHandle);

			modelBuilder.Entity<WebAuthnCredential>()
				.HasIndex(c => c.CredentialId);

			modelBuilder.Entity<WebAuthnCredential>()
				.HasOne(c => c.Owner)
				.WithMany(u => u.WebAuthnCredentials)
				.HasForeignKey(c => c.OwnerID);

			#endregion

			#region Client IP Addresses

			modelBuilder.Entity<ClientIpAddress>()
				.HasIndex(ipa => new { ipa.BrowserSessionID, ipa.LastSeen });

			modelBuilder.Entity<ClientIpAddress>()
				.HasIndex(ipa => ipa.LastSeen);

			modelBuilder.Entity<ClientIpAddress>()
				.HasIndex(ipa => ipa.IpAddress);

			#endregion

			#region Browser Sessions

			modelBuilder.Entity<BrowserSession>()
				.HasIndex(bs => bs.FingerPrint);

			modelBuilder.Entity<BrowserSession>()
				.HasIndex(bs => bs.LastSeenOn);

			modelBuilder.Entity<BrowserSession>()
				.HasIndex(bs => bs.FirstSignInOn);

			modelBuilder.Entity<BrowserSession>()
				.HasIndex(bs => new { bs.UserID, bs.LastSeenOn });

			modelBuilder.Entity<BrowserSession>()
				.HasIndex(bs => new { bs.UserID, bs.FirstSignInOn });

			#endregion
		}

		#endregion
	}
}
