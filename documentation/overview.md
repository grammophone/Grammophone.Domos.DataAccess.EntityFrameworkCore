# Overview

`Grammophone.Domos.DataAccess.EntityFrameworkCore` provides the EF Core backing implementation for Domos data access.

Concrete containers expose native EF Core `DbSet<T>` properties. Adapter classes expose provider-neutral Domos contracts through `IEntitySet<T>` wrappers.

The implementation mirrors the EF6 hierarchy and the DataAccess EF Core test-domain pattern:

- Native EF Core contexts inherit from `EFCoreDomainContainer`.
- Application-facing adapters inherit from `EFCoreDomainContainerAdapter<T>`.
- Entity sets are adapted with `EFCoreSet<T>`.

The project targets `net8.0` and uses EF Core SQL Server package settings matching the DataAccess EF Core test project.
