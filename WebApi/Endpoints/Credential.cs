﻿using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebApi.Base;

namespace WebApi.Endpoints;

public class Credential : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetCredentials)
            .MapGet(GetCredential, "{id}")
            .MapPost(CreateCredential)
            .MapPut(UpdateCredential, "{id}")
            .MapDelete(DeleteCredential, "{id}")
            .MapDelete(DeleteCredentials);
    }
    
    public async Task<IList<ZqAuth.Credential>> GetCredentials(IApplicationDbContext context)
    {
        return await context.Credentials.ToListAsync();
    }

    public async Task<ZqAuth.Credential> GetCredential(IApplicationDbContext context, CancellationToken cancellationToken, Guid id)
    {
        return await context.Credentials.FirstOrDefaultAsync(c => c.id == id) ?? new ZqAuth.Credential();
    }

    public record struct NewCredentialRequest(string UserId, string Password, string Domain);
    public async Task<Guid> CreateCredential(IApplicationDbContext context, CancellationToken cancellationToken, NewCredentialRequest credentialRequest)
    {
        var credential = new ZqAuth.Credential(credentialRequest.UserId, credentialRequest.Password, credentialRequest.Domain);
        if(!context.Credentials.Any(c => c.Domain == credential.Domain && c.UserId == credential.UserId))
            context.Credentials.Add(credential);
        await context.SaveChangesAsync(cancellationToken);
        return credential.id;
    }
    
    public record struct ExistingCredentialRequest(string UserId, string Password, string Domain);
    public async Task<Guid> UpdateCredential(IApplicationDbContext context, CancellationToken cancellationToken, Guid id, ExistingCredentialRequest credentialRequest)
    {
        var credential = await context.Credentials
            .FindAsync(new object[] { id }, cancellationToken);
        if (credential != null)
        {
            credential.UserId = credentialRequest.UserId;
            credential.Password = credentialRequest.Password;
            credential.Domain = credentialRequest.Domain;
            await context.SaveChangesAsync(cancellationToken);
        } 
        return credential.id;
    }

    public async Task<IResult> DeleteCredential(IApplicationDbContext context, CancellationToken cancellationToken, Guid id)
    {
        var entity = await context.Credentials
            .FindAsync(new object[] { id }, cancellationToken);

        context.Credentials.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Results.NoContent();
    }

    public async Task<IResult> DeleteCredentials(IApplicationDbContext context, CancellationToken cancellationToken)
    {
        var allRecords = context.Credentials.ToList();
        context.Credentials.RemoveRange(allRecords);
        await context.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}