# Model Mappings

The EF Core mappings translate the EF6 Domos model-building methods to EF Core `ModelBuilder` calls.

The mappings include user indexes, `UsersToRoles`, cascade user relationships, workflow indexes, accounting indexes, funds transfer indexes and optional invoice relationships.

`FundsTransferRequestGroup.EncryptedBankAccountInfo` is configured as a complex value object. The composite request-group index uses owned-property shadow names alongside account-holder and effective-date fields.

EF Core decimal precision is configured for invoice line rates, quantities and invoice tax percentage factors.

Set-based mutations use EF Core's native set operation support through the DataAccess EF Core query translator.
