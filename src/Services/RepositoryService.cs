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

	public async Task<UserDocument?> GetUserByIdAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var userId = Guid.Parse(id);
		var user = await _dbContext.Users.FindAsync(userId);
		return user?.ToDocument();
	}

	public async Task<UserDocument?> GetUserByUsernameAsync(string username)
	{
		if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

		var user = await _dbContext.Users
			.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
		return user?.ToDocument();
	}

	public async Task<UserDocument?> GetUserByEmailAsync(string email)
	{
		if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));

		var user = await _dbContext.Users
			.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
		return user?.ToDocument();
	}

	public async Task<UserDocument> CreateUserAsync(UserDocument userDocument)
	{
		if (userDocument == null) throw new ArgumentNullException(nameof(userDocument));

		var user = userDocument.ToEntity();

		await _dbContext.Users.AddAsync(user);
		await _dbContext.SaveChangesAsync();

		return user.ToDocument();
	}

	public async Task<UserDocument?> UpdateUserAsync(UserDocument userDocument)
	{
		if (userDocument == null) throw new ArgumentNullException(nameof(userDocument));
		if (string.IsNullOrEmpty(userDocument.Id))
			throw new ArgumentException("User ID is required", nameof(userDocument));

		var userId = Guid.Parse(userDocument.Id);
		var existingUser = await _dbContext.Users.FindAsync(userId);

		if (existingUser == null) return null;

		existingUser.Username = userDocument.Username;
		existingUser.Email = userDocument.Email;
		existingUser.Mobile = userDocument.Mobile;
		existingUser.Salt = userDocument.Salt;
		existingUser.Hash = userDocument.Hash;

		await _dbContext.SaveChangesAsync();

		return existingUser.ToDocument();
	}

	public async Task<bool> DeleteUserAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var userId = Guid.Parse(id);
		var user = await _dbContext.Users.FindAsync(userId);

		if (user == null) return false;

		_dbContext.Users.Remove(user);
		await _dbContext.SaveChangesAsync();

		return true;
	}

	#endregion

	#region Account Operations

	public async Task<List<AccountDocument>> GetAccountsByUserIdAsync(string userId)
	{
		if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

		var userGuid = Guid.Parse(userId);
		var accounts = await _dbContext.Accounts
			.Where(a => a.UserId == userGuid)
			.ToListAsync();

		return accounts.Select(a => a.ToDocument()).ToList();
	}

	public async Task<AccountDocument?> GetAccountByIdAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var accountId = Guid.Parse(id);
		var account = await _dbContext.Accounts.FindAsync(accountId);

		return account?.ToDocument();
	}

	public async Task<AccountDocument> CreateAccountAsync(AccountDocument accountDocument)
	{
		if (accountDocument == null) throw new ArgumentNullException(nameof(accountDocument));

		var account = accountDocument.ToEntity();

		await _dbContext.Accounts.AddAsync(account);
		await _dbContext.SaveChangesAsync();

		return account.ToDocument();
	}

	public async Task<AccountDocument?> UpdateAccountAsync(AccountDocument accountDocument)
	{
		if (accountDocument == null) throw new ArgumentNullException(nameof(accountDocument));
		if (string.IsNullOrEmpty(accountDocument.Id))
			throw new ArgumentException("Account ID is required", nameof(accountDocument));

		var accountId = Guid.Parse(accountDocument.Id);
		var existingAccount = await _dbContext.Accounts.FindAsync(accountId);

		if (existingAccount == null) return null;

		existingAccount.Category = accountDocument.Category;
		existingAccount.Name = accountDocument.Name;
		existingAccount.Username = accountDocument.Username;
		existingAccount.Pattern = accountDocument.Pattern;
		existingAccount.Length = accountDocument.Length;
		existingAccount.IncludeSpecialCharacter = accountDocument.IncludeSpecialCharacter;
		existingAccount.UseCustomSpecialCharacter = accountDocument.UseCustomSpecialCharacter;
		existingAccount.CustomSpecialCharacter = accountDocument.CustomSpecialCharacter;
		existingAccount.Notes = accountDocument.Notes;
		existingAccount.IsFavorite = accountDocument.IsFavorite;

		await _dbContext.SaveChangesAsync();

		return existingAccount.ToDocument();
	}

	public async Task<bool> DeleteAccountAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var accountId = Guid.Parse(id);
		var account = await _dbContext.Accounts.FindAsync(accountId);

		if (account == null) return false;

		_dbContext.Accounts.Remove(account);
		await _dbContext.SaveChangesAsync();

		return true;
	}

	#endregion

	#region Token Operations

	public async Task<TokenDocument?> GetTokenByIdAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var tokenId = Guid.Parse(id);
		var token = await _dbContext.Tokens.FindAsync(tokenId);

		return token?.ToDocument();
	}

	public async Task<TokenDocument?> GetTokenBySecretAsync(string secret)
	{
		if (string.IsNullOrEmpty(secret)) throw new ArgumentNullException(nameof(secret));

		var token = await _dbContext.Tokens
			.FirstOrDefaultAsync(t => t.Secret == secret);

		return token?.ToDocument();
	}

	public async Task<List<TokenDocument>> GetTokensByUserIdAsync(string userId)
	{
		if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

		var userGuid = Guid.Parse(userId);
		var tokens = await _dbContext.Tokens
			.Where(t => t.UserId == userGuid)
			.ToListAsync();

		return tokens.Select(t => t.ToDocument()).ToList();
	}

	public async Task<TokenDocument> CreateTokenAsync(TokenDocument tokenDocument)
	{
		if (tokenDocument == null) throw new ArgumentNullException(nameof(tokenDocument));

		var token = tokenDocument.ToEntity();

		await _dbContext.Tokens.AddAsync(token);
		await _dbContext.SaveChangesAsync();

		return token.ToDocument();
	}

	public async Task<bool> DeleteTokenAsync(string id)
	{
		if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

		var tokenId = Guid.Parse(id);
		var token = await _dbContext.Tokens.FindAsync(tokenId);

		if (token == null) return false;

		_dbContext.Tokens.Remove(token);
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
