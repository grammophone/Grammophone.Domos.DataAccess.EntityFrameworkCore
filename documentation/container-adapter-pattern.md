# Container And Adapter Pattern

The EF Core implementation uses the same separation as the EF6 implementation.

Concrete containers own EF Core model configuration and expose `DbSet<T>`. Adapter classes implement the Domos contracts and wrap native sets as `IEntitySet<T>`.

Adapter classes:

- `EFCoreUsersDomainContainerAdapter<U, D>`.
- `EFCoreWorkflowUsersDomainContainerAdapter<U, BST, D>`.
- `EFCoreDomosDomainContainerAdapter<...>` for accounting and invoices.

Consumers should register adapters as `IUsersDomainContainer`, `IWorkflowUsersDomainContainer` or `IDomosDomainContainer` implementations.
