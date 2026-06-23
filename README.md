# Grammophone.Domos.DataAccess.EntityFrameworkCore

Entity Framework Core implementation of the `Grammophone.Domos.DataAccess` domain-container contracts.

The concrete `EFCore*DomainContainer` classes expose native EF Core `DbSet<T>` properties. The corresponding `EFCore*DomainContainerAdapter` classes expose provider-neutral `IEntitySet<T>` properties for application logic.
