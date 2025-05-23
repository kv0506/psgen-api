using Azure.Data.Tables;
using PsGenApi.Document;
using PsGenApi.Entity;

namespace PsGenApi.Services;

public class TableService
{
    private static readonly string UsersPartitionKey = "Users";
    private static readonly string AccountsPartitionKey = "Accounts";
    private static readonly string AuthTokenPartitionKey = "AuthTokens";

    private readonly IConfiguration _configuration;

    public TableService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task CreateOrUpdateTokenDocumentAsync(Token token)
    {
        var tableClient = CreateTableClient();

        var entity = new DocumentEntity
        {
            PartitionKey = AuthTokenPartitionKey,
            RowKey = token.Id,
            Timestamp = DateTimeOffset.UtcNow,
            UserId = token.UserId,
            Document = SerializationService.Serialize(token)
        };

        return tableClient.UpsertEntityAsync(entity);
    }

    public async Task<Token?> GetTokenDocumentAsync(string tokenId)
    {
        var tableClient = CreateTableClient();

        var entities = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == AuthTokenPartitionKey && ent.RowKey == tokenId);
        await foreach (var entity in entities)
        {
            return SerializationService.Deserialize<Token>(entity.Document);
        }

        return null;
    }

    public Task CreateOrUpdateUserDocumentAsync(User user)
    {
        var tableClient = CreateTableClient();

        var entity = new DocumentEntity
        {
            PartitionKey = UsersPartitionKey,
            RowKey = user.Username,
            Timestamp = DateTimeOffset.UtcNow,
            UserId = user.Id,
            Document = SerializationService.Serialize(user)
        };

        return tableClient.UpsertEntityAsync(entity);
    }

    public async Task<User?> GetUserDocumentByUsernameAsync(string username)
    {
        var tableClient = CreateTableClient();

        var entities = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == UsersPartitionKey && ent.RowKey == username);

        await foreach (var entity in entities)
        {
            return SerializationService.Deserialize<User>(entity.Document);
        }

        return null;
    }

    public async Task<User?> GetUserDocumentByUserIdAsync(string userId)
    {
        var tableClient = CreateTableClient();

        var entities = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == UsersPartitionKey && ent.UserId == userId);

        await foreach (var entity in entities)
        {
            return SerializationService.Deserialize<User>(entity.Document);
        }

        return null;
    }

    public Task CreateOrUpdateAccountDocumentAsync(string userId, Account account)
    {
        var tableClient = CreateTableClient();

        var entity = new DocumentEntity
        {
            PartitionKey = AccountsPartitionKey,
            RowKey = account.Id,
            Timestamp = DateTimeOffset.UtcNow,
            UserId = userId,
            Document = SerializationService.Serialize(account)
        };

        return tableClient.UpsertEntityAsync(entity);
    }

    public Task DeleteAccountDocumentAsync(string accountId)
    {
        var tableClient = CreateTableClient();
        return tableClient.DeleteEntityAsync(AccountsPartitionKey, accountId);
    }

    public async Task<Account?> GetAccountDocumentAsync(string accountId)
    {
        var tableClient = CreateTableClient();

        var entities = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == AccountsPartitionKey && ent.RowKey == accountId);
        await foreach (var entity in entities)
        {
            return SerializationService.Deserialize<Account>(entity.Document);
        }

        return null;
    }

    public async Task<IList<Account>> GetAccountDocumentsAsync(string userId)
    {
        var tableClient = CreateTableClient();

        var documents = new List<Account>();

        var entities = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == AccountsPartitionKey && ent.UserId == userId);
        await foreach (var entity in entities)
        {
            var document = SerializationService.Deserialize<Account>(entity.Document);
            if (document != null)
            {
                documents.Add(document);
            }
        }

        return documents;
    }    public TableClient CreateTableClient()
    {
        return new TableClient(_configuration["StorageConnectionString"], _configuration["TableName"]);
    }
}