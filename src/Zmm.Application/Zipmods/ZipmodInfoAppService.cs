using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Zmm.Localization;

namespace Zmm.Zipmods;

public class ZipmodInfoAppService : CrudAppService<ZipmodInfo, ZipmodInfoDto, Guid, ZipmodInfoRequestInput>, IZipmodInfoAppService
{
    public ZipmodInfoAppService(IRepository<ZipmodInfo, Guid> repository) : base(repository)
    {
        LocalizationResource = typeof(ZmmResource);
        ObjectMapperContext = typeof(ZmmApplicationModule);
    }

    protected override async Task<IQueryable<ZipmodInfo>> CreateFilteredQueryAsync(ZipmodInfoRequestInput input)
    {
        var query = await ReadOnlyRepository.GetQueryableAsync();

        query = query
                .WhereIf(!input.Identifier.IsNullOrEmpty(), u => u.Identifier.Contains(input.Identifier!))
                .WhereIf(!input.Version.IsNullOrEmpty(), u => u.Version!.Contains(input.Version!))
                .WhereIf(!input.Author.IsNullOrEmpty(), u => u.Author!.Contains(input.Author!))
                .WhereIf(!input.Game.IsNullOrEmpty(), u => u.Game!.Contains(input.Game!))
                .WhereIf(input.IsCharaMod is not null, u => u.IsCharaMod == input.IsCharaMod)
                .WhereIf(input.IsStudioMod is not null, u => u.IsStudioMod == input.IsStudioMod)
                .WhereIf(input.IsMapMod is not null, u => u.IsMapMod == input.IsMapMod)
                .WhereIf(input.UpdateTimeStart is not null, u => u.UpdateTime >= input.UpdateTimeStart)
                .WhereIf(input.UpdateTimeEnd is not null, u => u.UpdateTime <= input.UpdateTimeEnd)
            ;

        return query;
    }
}