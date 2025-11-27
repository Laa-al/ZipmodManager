using System;
using System.Linq;
using System.Linq.Expressions;

namespace Zmm.Zipmods;

public static class QueryExtensions
{
    public static IQueryable<ZipmodFile> Filter(this IQueryable<ZipmodFile> query, ZipmodFileRequestInput input)
    {
        return query
                .WhereIf(input.Path, str =>u => u.Path.ToLower().Contains(str))
                .WhereIf(input.Identifier, str =>u => u.Info.Identifier.ToLower().Contains(str))
                .WhereIf(input.Version, str =>u => u.Info.Version!.ToLower().Contains(str))
                .WhereIf(input.Author, str =>u => u.Info.Author!.ToLower().Contains(str))
                .WhereIf(input.Game, str =>u => u.Info.Game!.ToLower().Contains(str))
                .WhereIf(input.Content, str =>u => u.Info.Content.ToLower().Contains(str))
                .WhereIf(input.IsCharaMod is not null, u => u.Info.IsCharaMod == input.IsCharaMod)
                .WhereIf(input.IsStudioMod is not null, u => u.Info.IsStudioMod == input.IsStudioMod)
                .WhereIf(input.IsMapMod is not null, u => u.Info.IsMapMod == input.IsMapMod)
                .WhereIf(input.UpdateTimeStart is not null, u => u.Info.UpdateTime >= input.UpdateTimeStart)
                .WhereIf(input.UpdateTimeEnd is not null, u => u.Info.UpdateTime <= input.UpdateTimeEnd)
                .WhereIf(input.MinSize is not null, u => u.Info.FileSize >= input.MinSize * 1024 * 1024)
                .WhereIf(input.MaxSize is not null, u => u.Info.FileSize <= input.MaxSize)
            ;
    }

    public static IQueryable<ZipmodLink> Filter(this IQueryable<ZipmodLink> query, ZipmodLinkRequestInput input)
    {
        return query
                .WhereIf(input.Name, str =>u => u.Name!.ToLower().Contains(str))
                .WhereIf(input.Description, str =>u => u.Description!.ToLower().Contains(str))
                .WhereIf(input.Size, str =>u => u.Size!.ToLower().Contains(str))
                .WhereIf(input.LinkMinSize is not null, u => u.LinkSize >= input.LinkMinSize * 1024 * 1024)
                .WhereIf(input.LinkMaxSize is not null, u => u.LinkSize <= input.LinkMaxSize * 1024 * 1024)
                .WhereIf(input.UploadTimeStrat is not null, u => u.UploadTime >= input.UploadTimeStrat)
                .WhereIf(input.UploadTimeEnd is not null, u => u.UploadTime <= input.UploadTimeEnd)
                .WhereIf(input.IsInvalid is not null, u => u.IsInvalid == input.IsInvalid)
                .WhereIf(input.IsNoInfo is not null, u => (u.Info == null) == input.IsNoInfo)
                .WhereIf(input.Identifier, str =>u => u.Info!.Identifier.ToLower().Contains(str))
                .WhereIf(input.Version, str =>u => u.Info!.Version!.ToLower().Contains(str))
                .WhereIf(input.Author, str =>u => u.Info!.Author!.ToLower().Contains(str))
                .WhereIf(input.Game, str =>u => u.Info!.Game!.ToLower().Contains(str))
                .WhereIf(input.Content, str =>u => u.Info!.Content.ToLower().Contains(str))
                .WhereIf(input.IsDownload is not null, u => u.Info!.Files.Any())
                .WhereIf(input.IsCharaMod is not null, u => u.Info!.IsCharaMod == input.IsCharaMod)
                .WhereIf(input.IsStudioMod is not null, u => u.Info!.IsStudioMod == input.IsStudioMod)
                .WhereIf(input.IsMapMod is not null, u => u.Info!.IsMapMod == input.IsMapMod)
                .WhereIf(input.UpdateTimeStart is not null, u => u.Info!.UpdateTime >= input.UpdateTimeStart)
                .WhereIf(input.UpdateTimeEnd is not null, u => u.Info!.UpdateTime <= input.UpdateTimeEnd)
                .WhereIf(input.MinSize is not null, u => u.Info!.FileSize >= input.MinSize * 1024 * 1024)
                .WhereIf(input.MaxSize is not null, u => u.Info!.FileSize <= input.MaxSize * 1024 * 1024)
            ;
    }

    private static IQueryable<T> WhereIf<T>(this IQueryable<T> query, string? str,
        Func<string, Expression<Func<T, bool>>> predicate)
    {
        return str.IsNullOrWhiteSpace() ? query : query.Where(predicate(str.ToLower()));
    }
}