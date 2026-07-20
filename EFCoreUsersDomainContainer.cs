using System;
using System.Collections.Generic;
using System.Linq;
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
		public EFCoreUsersDomainContainer(DbContextOptions options)
			: base(options)
		{
		}

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="options">The context options.</param>
		/// <param name="transactionMode">The transaction behavior.</param>
		public EFCoreUsersDomainContainer(DbContextOptions options, TransactionMode transactionMode)
			: base(options, transactionMode)
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

		/// <summary>
		/// Scans the model for all entities exposing CreatorUser / LastModifierUser navigation properties
		/// (from <see cref="TrackingEntity{U}"/>, <see cref="UserTrackingEntity{U}"/>, <see cref="Disposition"/> etc.) and configures the
		/// one-sided relationships. This replaces hundreds of manual HasOne lines that EF Core does not infer.
		/// Call this early in OnModelCreating (after base.OnModelCreating) so that later global
		/// SetDefaultDeleteBehavior can adjust any cascades.
		/// </summary>
		/// <typeparam name="TU">The type parameter for finding <see cref="ITrackingEntity{TU}"/> and <see cref="IUserTrackingEntity{TU}"/> implmenetatoins.</typeparam>
		protected static void ConfigureAllTrackingEntitiesNavigations<TU>(ModelBuilder modelBuilder)
			where TU : User
		{
			if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				var clrType = entityType.ClrType;
				if (clrType == null) continue;

				// Only configure for concrete mapped types.
				if (clrType.IsAbstract) continue;

				if (!typeof(ITrackingEntity<TU>).IsAssignableFrom(clrType)) continue;

				// Look for the navigation properties declared on Tracking* base classes.
				// If CreatorUser navigation exists and is not yet backed by a relationship, configure it.
				modelBuilder.Entity(clrType)
					.HasOne(nameof(ITrackingEntity<TU>.CreatorUser))
					.WithMany();

				// Same for LastModifierUser.
				modelBuilder.Entity(clrType)
					.HasOne(nameof(ITrackingEntity<TU>.LastModifierUser))
					.WithMany();

				// Do the same for IUserTrackingEntity<U>, if implemented.
				if (typeof(IUserTrackingEntity<TU>).IsAssignableFrom(clrType))
				{
					modelBuilder.Entity(clrType)
						.HasOne(nameof(IUserTrackingEntity<TU>.OwningUser))
						.WithMany();
				}
			}
		}

		/// <summary>
		/// Enable lazy loading and change tracking.
		/// </summary>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseLazyLoadingProxies();
		}

		/// <inheritdoc/>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region User

			modelBuilder.Ignore<User>();

			modelBuilder.Entity<U>().ToTable("Users");

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

			modelBuilder.Entity<U>()
				.HasMany(u => u.Roles)
				.WithMany()
				.UsingEntity<ManyToMany<Role, long, U, long>>(
					"UsersToRoles",
					l => l.HasOne(mm => mm.Left).WithMany().HasForeignKey(mm => mm.LeftID),
					r => r.HasOne(mm => mm.Right).WithMany().HasForeignKey(mm => mm.RightID),
					j =>
					{
						j.Property(mm => mm.LeftID).HasColumnName("Role_ID");
						j.Property(mm => mm.RightID).HasColumnName("User_ID");
					});

			modelBuilder.Entity(typeof(U))
				.HasMany(nameof(User.Dispositions))
				.WithOne(nameof(Disposition.OwningUser))
				.HasForeignKey(nameof(Disposition.OwningUserID))
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity(typeof(U))
				.HasMany(nameof(User.Registrations))
				.WithOne(nameof(Registration.User))
				.HasForeignKey(nameof(Registration.UserID))
				.OnDelete(DeleteBehavior.Cascade);

			#endregion

			#region Dispositions

			modelBuilder.Entity<Disposition>()
				.HasOne(typeof(U), nameof(Disposition.CreatorUser))
				.WithMany()
				.HasForeignKey(nameof(Disposition.CreatorUserID));

			modelBuilder.Entity<Disposition>()
				.HasOne(typeof(U), nameof(Disposition.LastModifierUser))
				.WithMany()
				.HasForeignKey(nameof(Disposition.LastModifierUserID));

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
				.HasOne(typeof(U), nameof(WebAuthnCredential.Owner))
				.WithMany(nameof(User.WebAuthnCredentials))
				.HasForeignKey(nameof(WebAuthnCredential.OwnerID));

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
				.Navigation(bs => bs.IPAddresses)
				.HasField("ipAddresses");

			modelBuilder.Entity<BrowserSession>()
				.HasOne(typeof(U), nameof(BrowserSession.User))
				.WithMany(nameof(User.Sessions))
				.HasForeignKey(nameof(BrowserSession.UserID));

			modelBuilder.Entity<BrowserSession>()
				.HasIndex(bs => new { bs.UserID, bs.LastSeenOn });

			modelBuilder.Entity<BrowserSession>()
				.HasIndex(bs => new { bs.UserID, bs.FirstSignInOn });

			#endregion
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Returns every concrete (non-abstract) type that derives from the given base type.
		/// We first scan the assembly that defines the abstract type (most reliable),
		/// then fall back to other loaded assemblies. This is required because EF Core
		/// will not automatically discover derived entity types referenced only via DbSet&lt;Abstract&gt;.
		/// </summary>
		private static IEnumerable<Type> FindAllConcreteDerivedTypes(Type baseType)
		{
			if (baseType == null) yield break;

			// Primary: the assembly that actually declares the abstract base (guaranteed loaded)
			foreach (var t in baseType.Assembly.GetTypes())
			{
				if (t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
					yield return t;
			}

			// Fallback: other assemblies currently in the AppDomain
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (asm == baseType.Assembly || asm.IsDynamic) continue;

				Type[] types;
				try { types = asm.GetTypes(); }
				catch { continue; }

				foreach (var t in types)
				{
					if (t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
						yield return t;
				}
			}
		}

		#endregion
	}
}
