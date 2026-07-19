using SmagerUp.Core.API.DTOs;
using static Dapper.SqlMapper;

namespace SmagerUp.Core.API.Data.Admin;

public class AdminDataRepository : BaseDataRepository, ICoreDataRepository
{
    private readonly AdminDbContext _ctx;

    public AdminDataRepository(AdminDbContext ctx)
    {
        _ctx = ctx;
        this.connection = _ctx.CreateConnection();
    }

    public async Task<object> ExecuteAsync(Guid userId,DataRequest request){
        try{
            //this.clientId = Guid.Empty;
            var action =   await this.GetActionAsync(request.ActionCode);
            return await RunActionAsync(userId,request, action);
        }
        catch (Exception ex)
        {
            return new
            {
                ok = false,
                errMsg =  ex.Message
            };
        }

    } 

}