# Consumer Migration Notes

When migrating a concrete Domos application to EF Core backing:

- Keep the concrete EF Core context as infrastructure.
- Register the matching `EFCore*DomainContainerAdapter` as the Domos domain-container contract.
- Use `Grammophone.DataAccess.QueryExtensions` in application/query code.
- Ensure entity collection navigations support change notifications if EF Core change-tracking proxies are enabled.
- Review provider-specific model differences, especially owned-value mapping and cascade delete behavior, against the application's database conventions.
