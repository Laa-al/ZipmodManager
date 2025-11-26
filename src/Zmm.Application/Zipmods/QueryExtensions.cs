using System;
using System.Linq;

namespace Zmm.Zipmods;

public static class QueryExtensions
{
    public static IQueryable<ZipmodFile> Filter(this IQueryable<ZipmodFile> query, ZipmodFileRequestInput input)
    {
        return query
                .WhereIf(!input.Path.IsNullOrEmpty(), u => u.Path.Contains(input.Path!))
                .WhereIf(!input.Identifier.IsNullOrEmpty(), u => u.Info.Identifier.Contains(input.Identifier!))
                .WhereIf(!input.Version.IsNullOrEmpty(), u => u.Info.Version!.Contains(input.Version!))
                .WhereIf(!input.Author.IsNullOrEmpty(), u => u.Info.Author!.Contains(input.Author!))
                .WhereIf(!input.Game.IsNullOrEmpty(), u => u.Info.Game!.Contains(input.Game!))
                .WhereIf(!input.Content.IsNullOrEmpty(), u => u.Info.Content.Contains(input.Content!))
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
                .WhereIf(!input.Name.IsNullOrEmpty(), u => u.Name!.Contains(input.Name!))
                .WhereIf(!input.Description.IsNullOrEmpty(), u => u.Description!.Contains(input.Description!))
                .WhereIf(!input.Size.IsNullOrEmpty(), u => u.Size!.Contains(input.Size!))
                .WhereIf(input.UploadTimeStrat is not null, u => u.UploadTime >= input.UploadTimeStrat)
                .WhereIf(input.UploadTimeEnd is not null, u => u.UploadTime <= input.UploadTimeEnd)
                .WhereIf(input.IsInvalid is not null, u => u.IsInvalid == input.IsInvalid)
                .WhereIf(input.IsNoInfo is not null, u => (u.Info == null) == input.IsNoInfo)
                .WhereIf(!input.Identifier.IsNullOrEmpty(), u => u.Info!.Identifier.Contains(input.Identifier!))
                .WhereIf(!input.Version.IsNullOrEmpty(), u => u.Info!.Version!.Contains(input.Version!))
                .WhereIf(!input.Author.IsNullOrEmpty(), u => u.Info!.Author!.Contains(input.Author!))
                .WhereIf(!input.Game.IsNullOrEmpty(), u => u.Info!.Game!.Contains(input.Game!))
                .WhereIf(input.IsDownload is not null, u => u.Info!.Files.Any())
                .WhereIf(!input.Content.IsNullOrEmpty(), u => u.Info!.Content.Contains(input.Content!))
                .WhereIf(input.IsCharaMod is not null, u => u.Info!.IsCharaMod == input.IsCharaMod)
                .WhereIf(input.IsStudioMod is not null, u => u.Info!.IsStudioMod == input.IsStudioMod)
                .WhereIf(input.IsMapMod is not null, u => u.Info!.IsMapMod == input.IsMapMod)
                .WhereIf(input.UpdateTimeStart is not null, u => u.Info!.UpdateTime >= input.UpdateTimeStart)
                .WhereIf(input.UpdateTimeEnd is not null, u => u.Info!.UpdateTime <= input.UpdateTimeEnd)
                .WhereIf(input.MinSize is not null, u => u.Info!.FileSize >= input.MinSize * 1024 * 1024)
                .WhereIf(input.MaxSize is not null, u => u.Info!.FileSize <= input.MaxSize * 1024 * 1024)
            ;
    }
}