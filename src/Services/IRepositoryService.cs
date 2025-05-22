using PsGenApi.Document;

namespace PsGenApi.Services;

public interface IRepositoryService
{
    // User Operations
    Task<UserDocument?> GetUserByIdAsync(string id);
    Task<UserDocument?> GetUserByUsernameAsync(string username);
    Task<UserDocument?> GetUserByEmailAsync(string email);
    Task<UserDocument> CreateUserAsync(UserDocument userDocument);
    Task<UserDocument?> UpdateUserAsync(UserDocument userDocument);
    Task<bool> DeleteUserAsync(string id);

    // Account Operations
    Task<List<AccountDocument>> GetAccountsByUserIdAsync(string userId);
    Task<AccountDocument?> GetAccountByIdAsync(string id);
    Task<AccountDocument> CreateAccountAsync(AccountDocument accountDocument);
    Task<AccountDocument?> UpdateAccountAsync(AccountDocument accountDocument);
    Task<bool> DeleteAccountAsync(string id);

    // Token Operations
    Task<TokenDocument?> GetTokenByIdAsync(string id);
    Task<TokenDocument?> GetTokenBySecretAsync(string secret);
    Task<List<TokenDocument>> GetTokensByUserIdAsync(string userId);
    Task<TokenDocument> CreateTokenAsync(TokenDocument tokenDocument);
    Task<bool> DeleteTokenAsync(string id);
    Task<bool> DeleteExpiredTokensAsync();
}
