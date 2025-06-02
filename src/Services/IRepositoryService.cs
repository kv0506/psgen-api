using PsGenApi.Document;

namespace PsGenApi.Services;

public interface IRepositoryService
{
	// User Operations
	Task<User?> GetUserByIdAsync(string id);
	Task<User?> GetUserByUsernameAsync(string username);
	Task<User?> GetUserByEmailAsync(string email);
	Task<User> CreateUserAsync(User user);
	Task<User?> UpdateUserAsync(User user);
	Task<bool> DeleteUserAsync(string id);

	// Account Operations
	Task<List<Account>> GetAccountsByUserIdAsync(string userId);
	Task<Account?> GetAccountByIdAsync(string id);
	Task<Account> CreateAccountAsync(Account account);
	Task<Account?> UpdateAccountAsync(Account account);
	Task<bool> DeleteAccountAsync(string id);

	// Token Operations
	Task<Token?> GetTokenByIdAsync(string id);
	Task<Token?> GetTokenBySecretAsync(string secret);
	Task<List<Token>> GetTokensByUserIdAsync(string userId);
	Task<Token> CreateTokenAsync(Token token);
	Task<bool> DeleteTokenAsync(string id);
	Task<bool> DeleteExpiredTokensAsync();
}