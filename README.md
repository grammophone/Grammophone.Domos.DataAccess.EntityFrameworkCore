# Grammophone.Domos.DataAccess.EntityFrameworkCore

Entity Framework Core implementation of the `Grammophone.Domos.DataAccess` domain-container contracts.

The concrete `EFCore*DomainContainer` classes expose native EF Core `DbSet<T>` properties. The corresponding `EFCore*DomainContainerAdapter` classes expose provider-neutral `IEntitySet<T>` properties for application logic.

## Main Features

- `EFCoreUsersDomainContainer<U>` maps users, roles, dispositions, registrations, credentials, sessions and file metadata.
- `EFCoreWorkflowUsersDomainContainer<U, BST>` adds workflow graph, state, path and transition mappings.
- `EFCoreDomosDomainContainer<...>` adds accounting, funds transfer and optional invoice mappings.
- `EFCore*DomainContainerAdapter` classes expose `IEntitySet<T>` properties for application logic.
- EF Core query shaping, async terminal methods and set operations are exposed through `Grammophone.DataAccess.QueryExtensions`.

## Documentation

- [Overview](documentation/overview.md)
- [Container and adapter pattern](documentation/container-adapter-pattern.md)
- [Model mappings](documentation/model-mappings.md)
- [Consumer migration notes](documentation/consumer-migration-notes.md)
