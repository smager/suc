using SmagerUp.Core.API.DTOs;
using static Dapper.SqlMapper;

namespace SmagerUp.Core.API.Data.Core;

public class CoreDataRepository : BaseDataRepository, ICoreDataRepository
{
    private readonly CoreDbContext _ctx;
    private readonly CoreActionsRepository _actions;

    public CoreDataRepository(CoreDbContext ctx,CoreActionsRepository sqlCommands)
    {
        _ctx = ctx;
        _actions = sqlCommands;
        this.connection = _ctx.CreateConnection();
    }


   
    public async Task<object> ExecuteAsync(Guid userId,DataRequest request){
        try{
            this.clientId = Guid.Empty;
            var action =   await this.GetActionAsync(request.ActionCode);

            return action.ActionType.ToUpper() switch{
                "Q" => await RunQueryAsync(userId,request, action),

                "C" => await RunCommandAsync(userId,request, action),

                _ => throw new Exception(
                        $"Unsupported ActionType '{action.ActionType}'.")
            };

        }
        catch (Exception ex)
        {
            return new
            {
                isSuccess = false,
                errMsg =  ex.Message
            };
        }

    } 

}