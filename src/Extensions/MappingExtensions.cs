using PsGenApi.DataModel;
using PsGenApi.Document;

namespace PsGenApi.Extensions;

public static class MappingExtensions
{
	// User mapping extensions
	public static DbUser ToDataModel(this User? entity)
	{
		if (entity == null)
			return null!;

		return new DbUser
		{
			Id = string.IsNullOrEmpty(entity.Id) ? Guid.NewGuid() : Guid.Parse(entity.Id),
			Username = entity.Username,
			Email = entity.Email,
			Mobile = entity.Mobile,
			Salt = entity.Salt,
			Hash = entity.Hash
		};
	}

	public static User ToEntity(this DbUser? dataModel)
	{
		if (dataModel == null)
			return null!;

		return new User
		{
			Id = dataModel.Id.ToString(),
			Username = dataModel.Username ?? string.Empty,
			Email = dataModel.Email ?? string.Empty,
			Mobile = dataModel.Mobile ?? string.Empty,
			Salt = dataModel.Salt ?? string.Empty,
			Hash = dataModel.Hash ?? string.Empty
		};
	}

	// Account mapping extensions
	public static DbAccount ToDataModel(this Account? entity)
	{
		if (entity == null)
			return null!;

		return new DbAccount
		{
			Id = string.IsNullOrEmpty(entity.Id) ? Guid.NewGuid() : Guid.Parse(entity.Id),
			UserId = Guid.Parse(entity.UserId),
			Category = entity.Category,
			Name = entity.Name,
			Username = entity.Username,
			Pattern = entity.Pattern,
			Length = entity.Length,
			IncludeSpecialCharacter = entity.IncludeSpecialCharacter,
			UseCustomSpecialCharacter = entity.UseCustomSpecialCharacter,
			CustomSpecialCharacter = entity.CustomSpecialCharacter,
			Notes = entity.Notes,
			IsFavorite = entity.IsFavorite
		};
	}

	public static Account ToEntity(this DbAccount? dataModel)
	{
		if (dataModel == null)
			return null!;

		return new Account
		{
			Id = dataModel.Id.ToString(),
			UserId = dataModel.UserId.ToString(),
			Category = dataModel.Category ?? string.Empty,
			Name = dataModel.Name ?? string.Empty,
			Username = dataModel.Username ?? string.Empty,
			Pattern = dataModel.Pattern ?? string.Empty,
			Length = dataModel.Length,
			IncludeSpecialCharacter = dataModel.IncludeSpecialCharacter,
			UseCustomSpecialCharacter = dataModel.UseCustomSpecialCharacter,
			CustomSpecialCharacter = dataModel.CustomSpecialCharacter ?? string.Empty,
			Notes = dataModel.Notes ?? string.Empty,
			IsFavorite = dataModel.IsFavorite
		};
	}

	// Token mapping extensions
	public static DbToken ToDataModel(this Token? entity)
	{
		if (entity == null)
			return null!;

		return new DbToken
		{
			Id = string.IsNullOrEmpty(entity.Id) ? Guid.NewGuid() : Guid.Parse(entity.Id),
			UserId = Guid.Parse(entity.UserId),
			Secret = entity.Secret,
			ExpiresAt = entity.ExpiresAt
		};
	}

	public static Token ToEntity(this DbToken? dataModel)
	{
		if (dataModel == null)
			return null!;

		return new Token
		{
			Id = dataModel.Id.ToString(),
			UserId = dataModel.UserId.ToString(),
			Secret = dataModel.Secret ?? string.Empty,
			ExpiresAt = dataModel.ExpiresAt
		};
	}
}