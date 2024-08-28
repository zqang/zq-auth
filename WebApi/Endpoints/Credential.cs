using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebApi.Base;

namespace WebApi.Endpoints;

public class Credential : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetCredentials)
            .MapPost(CreateCredential)
            .MapPut(UpdateCredential, "id")
            .MapDelete(DeleteCredential, "id");
    }
    
    public async Task<IList<ZqAuth.Credential>> GetCredentials(IApplicationDbContext context)
    {
        return await context.Credentials.ToListAsync();
    }
    
    public record struct NewCredentialRequest(string UserId, string Password, string Domain);
    public async Task<Guid> CreateCredential(IApplicationDbContext context, CancellationToken cancellationToken, NewCredentialRequest credentialRequest)
    {
        var credential = new ZqAuth.Credential(credentialRequest.UserId, credentialRequest.Password, credentialRequest.Domain);
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
}