using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Synchronizator.Database;
using Synchronizator.Database.Tables;
using Synchronizator.Lib.Models;

namespace Synchronizator.Lib.Services;

public class DatabaseService
{
    private readonly ApplicationDbContext _database;
    public DatabaseService(ApplicationDbContext database)
    {
        _database = database;
    }

    public List<ApplicationUserHasFile> GetUserFilesByToken(string userId, string token)
    {
        return _database.Files.Where(x => x.UserId == userId && x.Token == token).ToList();
    }
    public List<ApplicationUserHasFile> GetUserFiles(string userId)
    {
        return _database.Files.Where(x => x.UserId == userId).ToList();
    }
    public List<ApplicationUserHasToken> GetUserTokens(string userId)
    {
        return _database.Tokens.Where(x => x.UserId == userId).ToList();
    }

    public OperationResult AddFileToUser(UserHasFileDto dto)
    {
        var entity = new ApplicationUserHasFile()
        {
            DiskPath = dto.DiskPath,
            Id = Guid.NewGuid(),
            Token = dto.Token,
            LastUpdated = DateTimeOffset.UtcNow,
            UserId = dto.UserId
        };
        try
        {
            _database.Files.Add(entity);
            _database.SaveChanges();
        }
        catch (Exception e)
        {
            return new OperationResult() { Message = e.Message };
        }

        return new OperationResult() { Completed = true };
    }

    public OperationResult UpdateFileSynchronizationDate(Guid fileId)
    {
        var entity = _database.Files.FirstOrDefault(x => x.Id == fileId);
        if (entity == null)
            return new OperationResult() { Message = $"Cannot find file with id {fileId} to update date" };
        try
        {
            entity.LastUpdated = DateTime.Now;
            _database.Files.Update(entity);
            _database.SaveChanges();
        }
        catch (Exception e)
        {
            return new OperationResult() { Message = e.Message };
        }

        return new OperationResult() { Completed = true };
    }

    public OperationResult AddTokenToUser(UserHasTokenDto dto)
    {
        var entity = new ApplicationUserHasToken()
        {
            Id = Guid.NewGuid(),
            Token = dto.Token,
            UserId = dto.UserId
        };
        try
        {
            _database.Tokens.Add(entity);
            _database.SaveChanges();
        }
        catch (Exception e)
        {
            return new OperationResult() { Message = e.Message };
        }

        return new OperationResult() { Completed = true };
    }

    public OperationResult RemoveTokenFromUser(Guid tokenId)
    {
        try
        {
            var entity =_database.Tokens.FirstOrDefault(x => x.Id == tokenId);
            if (entity != null) _database.Tokens.Remove(entity);
            else
            {
                return new OperationResult() { Message = $"Cannot remove token relation {tokenId} - id not found" };
            }
            _database.SaveChanges();
        }
        catch (Exception e)
        {
            return new OperationResult() { Message = e.Message };
        }

        return new OperationResult() { Completed = true };
    }

    public ApplicationUser? GetUserByToken(string token)
    {
        var tokenEntity = _database.Tokens.Include(x => x.User).FirstOrDefault(x => x.Token == token);
        return tokenEntity?.User;
    }

    public bool IsFileAsociatedWithToken(string path, string token)
    {
        return _database.Files.Any(x => x.Token == token && x.DiskPath == path);
    }
}