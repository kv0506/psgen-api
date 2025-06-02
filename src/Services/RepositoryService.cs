using PsGenApi.DataModel;
using PsGenApi.Document;
using PsGenApi.Extensions;

// ReSharper disable ConvertToPrimaryConstructor

namespace PsGenApi.Services;

public class RepositoryService : IRepositoryService
{
	private readonly PsGenDbContext _dbContext;

	public RepositoryService(PsGenDbContext dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	#region User Operations

	public async Task<User?> GetUserByIdAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var userId = Guid.Parse(id);
		var dbUser = await _dbContext.Users.FindAsync(userId);
		return dbUser?.ToEntity();
	}

	public async Task<User?> GetUserByUsernameAsync(string username)
	{
		if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

		var dbUser = await _dbContext.Users
			.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
		return dbUser?.ToEntity();
	}

	public async Task<User?> GetUserByEmailAsync(string email)
	{
		if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));

		var dbUser = await _dbContext.Users
			.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
		return dbUser?.ToEntity();
	}

	public async Task<User> CreateUserAsync(User user)
	{
		if (user == null) throw new ArgumentNullException(nameof(user));

		var dbUser = user.ToDataModel();

		await _dbContext.Users.AddAsync(dbUser);
		await _dbContext.SaveChangesAsync();

		return dbUser.ToEntity();
	}

	public async Task<User?> UpdateUserAsync(User user)
	{
		if (user == null) throw new ArgumentNullException(nameof(user));
		if (string.IsNullOrEmpty(user.Id))
			throw new ArgumentException("User ID is required", nameof(user));

		var userId = Guid.Parse(user.Id);
		var existingUser = await _dbContext.Users.FindAsync(userId);

		if (existingUser == null) return null;

		existingUser.Username = user.Username;
		existingUser.Email = user.Email;
		existingUser.Mobile = user.Mobile;
		existingUser.Salt = user.Salt;
		existingUser.Hash = user.Hash;

		await _dbContext.SaveChangesAsync();

		return existingUser.ToEntity();
	}

	public async Task<bool> DeleteUserAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var userId = Guid.Parse(id);
		var dbUser = await _dbContext.Users.FindAsync(userId);

		if (dbUser == null) return false;

		_dbContext.Users.Remove(dbUser);
		await _dbContext.SaveChangesAsync();

		return true;
	}

	#endregion

	#region Account Operations

	public async Task<List<Account>> GetAccountsByUserIdAsync(string userId)
	{
		if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

		var userGuid = Guid.Parse(userId);
		var dbAccounts = await _dbContext.Accounts
			.Where(a => a.UserId == userGuid)
			.ToListAsync();

		return dbAccounts.Select(a => a.ToEntity()).ToList();
	}

	public async Task<Account?> GetAccountByIdAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var accountId = Guid.Parse(id);
		var dbAccount = await _dbContext.Accounts.FindAsync(accountId);

		return dbAccount?.ToEntity();
	}

	public async Task<Account> CreateAccountAsync(Account account)
	{
		if (account == null) throw new ArgumentNullException(nameof(account));

		var dbAccount = account.ToDataModel();

		await _dbContext.Accounts.AddAsync(dbAccount);
		await _dbContext.SaveChangesAsync();

		return dbAccount.ToEntity();
	}

	public async Task<Account?> UpdateAccountAsync(Account account)
	{
		if (account == null) throw new ArgumentNullException(nameof(account));
		if (string.IsNullOrEmpty(account.Id))
			throw new ArgumentException("Account ID is required", nameof(account));

		var accountId = Guid.Parse(account.Id);
		var existingAccount = await _dbContext.Accounts.FindAsync(accountId);

		if (existingAccount == null) return null;

		existingAccount.Category = account.Category;
		existingAccount.Name = account.Name;
		existingAccount.Username = account.Username;
		existingAccount.Pattern = account.Pattern;
		existingAccount.Length = account.Length;
		existingAccount.IncludeSpecialCharacter = account.IncludeSpecialCharacter;
		existingAccount.UseCustomSpecialCharacter = account.UseCustomSpecialCharacter;
		existingAccount.CustomSpecialCharacter = account.CustomSpecialCharacter;
		existingAccount.Notes = account.Notes;
		existingAccount.IsFavorite = account.IsFavorite;

		await _dbContext.SaveChangesAsync();

		return existingAccount.ToEntity();
	}

	public async Task<bool> DeleteAccountAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var accountId = Guid.Parse(id);
		var dbAccount = await _dbContext.Accounts.FindAsync(accountId);

		if (dbAccount == null) return false;

		_dbContext.Accounts.Remove(dbAccount);
		await _dbContext.SaveChangesAsync();

		return true;
	}

	#endregion

	#region Token Operations

	public async Task<Token?> GetTokenByIdAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var tokenId = Guid.Parse(id);
		var dbToken = await _dbContext.Tokens.FindAsync(tokenId);

		return dbToken?.ToEntity();
	}

	public async Task<Token?> GetTokenBySecretAsync(string secret)
	{
		if (string.IsNullOrEmpty(secret)) throw new ArgumentNullException(nameof(secret));

		var dbToken = await _dbContext.Tokens
			.FirstOrDefaultAsync(t => t.Secret == secret);

		return dbToken?.ToEntity();
	}

	public async Task<List<Token>> GetTokensByUserIdAsync(string userId)
	{
		if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

		var userGuid = Guid.Parse(userId);
		var dbTokens = await _dbContext.Tokens
			.Where(t => t.UserId == userGuid)
			.ToListAsync();

		return dbTokens.Select(t => t.ToEntity()).ToList();
	}

	public async Task<Token> CreateTokenAsync(Token token)
	{
		if (token == null) throw new ArgumentNullException(nameof(token));

		var dbToken = token.ToDataModel();

		await _dbContext.Tokens.AddAsync(dbToken);
		await _dbContext.SaveChangesAsync();

		return dbToken.ToEntity();
	}

	public async Task<bool> DeleteTokenAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var tokenId = Guid.Parse(id);
		var dbToken = await _dbContext.Tokens.FindAsync(tokenId);

		if (dbToken == null) return false;

		_dbContext.Tokens.Remove(dbToken);
		await _dbContext.SaveChangesAsync();

		return true;
	}

	public async Task<bool> DeleteExpiredTokensAsync()
	{
		var now = DateTimeOffset.UtcNow;
		var expiredTokens = await _dbContext.Tokens
			.Where(t => t.ExpiresAt < now)
			.ToListAsync();

		if (!expiredTokens.Any()) return false;

		_dbContext.Tokens.RemoveRange(expiredTokens);
		await _dbContext.SaveChangesAsync();

		return true;
	}

	#endregion
}