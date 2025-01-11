using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Synchronizator.Lib.Models;

namespace Synchronizator.Lib.Services;

public class FileService(DatabaseService databaseService, IHostEnvironment environment)
{
    public const string SYNCER_PATH = "synchronized";
    
    public async Task<OperationResult> SaveFile(FileDataDto data)
    {
        PrepareEnv(SYNCER_PATH);
        
        var path = Path.Combine(environment.ContentRootPath,SYNCER_PATH, data.File.FileName);
        await data.File.CopyToAsync(File.Open(path, FileMode.OpenOrCreate));
        var userId = databaseService.GetUserByToken(data.Details.UserToken)?.Id;
        if (userId != null)
        {
            var entity = new UserHasFileDto()
            {
                Token = data.Details.UserToken,
                DiskPath = path,
                UserId = userId
            };
            var res = databaseService.AddFileToUser(entity);
            return !res.Completed
                ? new OperationResult() { Message = res.Message }
                : new OperationResult() { Completed = true };
        }

        return new OperationResult()
            { Message = $"cannot save file with name {data.File.FileName} and user token {data.Details.UserToken}" };
    }
    
    

    private void PrepareEnv(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}