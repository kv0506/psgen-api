using PsGenApi.DataModel;
using PsGenApi.Document;
using System;

namespace PsGenApi.Extensions;

public static class MappingExtensions
{
    // User mapping extensions
    public static User ToEntity(this UserDocument document)
    {
        if (document == null) return null;

        return new User
        {
            Id = string.IsNullOrEmpty(document.Id) ? Guid.NewGuid() : Guid.Parse(document.Id),
            Username = document.Username,
            Email = document.Email,
            Mobile = document.Mobile,
            Salt = document.Salt,
            Hash = document.Hash
        };
    }

    public static UserDocument ToDocument(this User entity)
    {
        if (entity == null) return null;

        return new UserDocument
        {
            Id = entity.Id.ToString(),
            Username = entity.Username,
            Email = entity.Email,
            Mobile = entity.Mobile,
            Salt = entity.Salt,
            Hash = entity.Hash
        };
    }

    // Account mapping extensions
    public static Account ToEntity(this AccountDocument document)
    {
        if (document == null) return null;

        return new Account
        {
            Id = string.IsNullOrEmpty(document.Id) ? Guid.NewGuid() : Guid.Parse(document.Id),
            UserId = Guid.Parse(document.UserId),
            Category = document.Category,
            Name = document.Name,
            Username = document.Username,
            Pattern = document.Pattern,
            Length = document.Length,
            IncludeSpecialCharacter = document.IncludeSpecialCharacter,
            UseCustomSpecialCharacter = document.UseCustomSpecialCharacter,
            CustomSpecialCharacter = document.CustomSpecialCharacter,
            Notes = document.Notes,
            IsFavorite = document.IsFavorite
        };
    }

    public static AccountDocument ToDocument(this Account entity)
    {
        if (entity == null) return null;

        return new AccountDocument
        {
            Id = entity.Id.ToString(),
            UserId = entity.UserId.ToString(),
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

    // Token mapping extensions
    public static Token ToEntity(this TokenDocument document)
    {
        if (document == null) return null;

        return new Token
        {
            Id = string.IsNullOrEmpty(document.Id) ? Guid.NewGuid() : Guid.Parse(document.Id),
            UserId = Guid.Parse(document.UserId),
            Secret = document.Secret,
            ExpiresAt = document.ExpiresAt
        };
    }

    public static TokenDocument ToDocument(this Token entity)
    {
        if (entity == null) return null;

        return new TokenDocument
        {
            Id = entity.Id.ToString(),
            UserId = entity.UserId.ToString(),
            Secret = entity.Secret,
            ExpiresAt = entity.ExpiresAt
        };
    }
}
