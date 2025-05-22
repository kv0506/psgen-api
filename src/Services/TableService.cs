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

    public Task CreateOrUpdateTokenDocumentAsync(TokenDocument document)
    {
        var tableClient = CreateTableClient();

        var entity = new DocumentEntity
        {
            PartitionKey = AuthTokenPartitionKey,
            RowKey = document.Id,
            Timestamp = DateTimeOffset.UtcNow,
            UserId = document.UserId,
            Document = SerializationService.Serialize(document)
        };

        return tableClient.UpsertEntityAsync(entity);
    }

    public async Task<TokenDocument?> GetTokenDocumentAsync(string tokenId)
    {
        var tableClient = CreateTableClient();

        var entities = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == AuthTokenPartitionKey && ent.RowKey == tokenId);
        await foreach (var entity in entities)
        {
            return SerializationService.Deserialize<TokenDocument>(entity.Document);
        }

        return null;
    }

    public Task CreateOrUpdateUserDocumentAsync(UserDocument document)
    {
        var tableClient = CreateTableClient();

        var entity = new DocumentEntity
        {
            PartitionKey = UsersPartitionKey,
            RowKey = document.Username,
            Timestamp = DateTimeOffset.UtcNow,
            UserId = document.Id,
            Document = SerializationService.Serialize(document)
        };

        return tableClient.UpsertEntityAsync(entity);
    }

    public async Task<UserDocument?> GetUserDocumentByUsernameAsync(string username)
    {
        var tableClient = CreateTableClient();

        var entities = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == UsersPartitionKey && ent.RowKey == username);

        await foreach (var entity in entities)
        {
            return SerializationService.Deserialize<UserDocument>(entity.Document);
        }

        return null;
    }

    public async Task<UserDocument?> GetUserDocumentByUserIdAsync(string userId)
    {
        var tableClient = CreateTableClient();

        var entities = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == UsersPartitionKey && ent.UserId == userId);

        await foreach (var entity in entities)
        {
            return SerializationService.Deserialize<UserDocument>(entity.Document);
        }

        return null;
    }

    public Task CreateOrUpdateAccountDocumentAsync(string userId, AccountDocument document)
    {
        var tableClient = CreateTableClient();

        var entity = new DocumentEntity
        {
            PartitionKey = AccountsPartitionKey,
            RowKey = document.Id,
            Timestamp = DateTimeOffset.UtcNow,
            UserId = userId,
            Document = SerializationService.Serialize(document)
        };

        return tableClient.UpsertEntityAsync(entity);
    }

    public Task DeleteAccountDocumentAsync(string accountId)
    {
        var tableClient = CreateTableClient();
        return tableClient.DeleteEntityAsync(AccountsPartitionKey, accountId);
    }

    public async Task<AccountDocument?> GetAccountDocumentAsync(string accountId)
    {
        var tableClient = CreateTableClient();

        var entities = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == AccountsPartitionKey && ent.RowKey == accountId);
        await foreach (var entity in entities)
        {
            return SerializationService.Deserialize<AccountDocument>(entity.Document);
        }

        return null;
    }

    public async Task<IList<AccountDocument>> GetAccountDocumentsAsync(string userId)
    {
        var tableClient = CreateTableClient();

        var documents = new List<AccountDocument>();

        var entities = tableClient.QueryAsync<DocumentEntity>(ent => ent.PartitionKey == AccountsPartitionKey && ent.UserId == userId);
        await foreach (var entity in entities)
        {
            var document = SerializationService.Deserialize<AccountDocument>(entity.Document);
            if (document != null)
            {
                documents.Add(document);
            }
        }

        return documents;
    }

    private TableClient CreateTableClient()
    {
        return new TableClient(_configuration["StorageConnectionString"], _configuration["TableName"]);
    }
}