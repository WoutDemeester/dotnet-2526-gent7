
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Entities;
using Rise.Domain.Extensions;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.Common;
using Rise.Shared.Identity;
using Rise.Shared.Projects;
using Rise.Shared.Resto;

namespace Rise.Services.Resto;

/// <summary>
/// Service for projects. Note the use of <see cref="ISessionContextProvider"/> to get the current user in this layer of the application.
/// </summary>
/// <param name="dbContext"></param>
/// <param name="sessionContextProvider"></param>
public class RestoService(ApplicationDbContext dbContext, ISessionContextProvider sessionContextProvider) : IRestoService
{
    public async Task<Result<RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx)
    {
        var query = dbContext.Restos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p => p.Name.Equals(request.SearchTerm) || p.Building.Name.Equals(request.SearchTerm)|| p.Building.Campus.Name.Equals(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(ctx);

        // Apply ordering
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, request.OrderBy))
                : query.OrderBy(e => EF.Property<object>(e, request.OrderBy));
        }
        else
        {
            // Default order
            query = query.OrderBy(d => d.Id);
        }

        var restos = await dbContext.Restos
    .Include(r => r.Menu)
    .ToListAsync(); 

        var result = restos.Select(d => new RestoDto
        {
            Menu = new MenuDto
            {
                Items = d.Menu.Items.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Select(item => new MenuItemDto
                    {
                        Name = item.Name,
                        Description = item.Description,
                        IsVeganAndHalal = item.IsVeganAndHalal,
                        Id = item.Id,
                        FoodCategory = item.Category.ToString()
                    }).ToList()),
                Id = d.Menu.Id
            },
            Id = d.Id,
            Name = d.Name,
            Coordinates = d.Coordinates
        }).ToList();


        return Result.Success(new RestoResponse.Index
        {
            Restos = restos.Select(r=> r.MapToRestoDto()),
            TotalCount = totalCount
        });
    }

    public async Task<Result> EditAsync(ProjectRequest.Edit req, CancellationToken ctx = default)
    {/*
        var project = await dbContext.Restos
            .Include(x => x.Id)
            .SingleOrDefaultAsync(x => x.Id == req.ProjectId, ctx);

        if (project is null)
            return Result.NotFound($"Project with Id '{req.ProjectId}' was not found.");

        // Currently logged-in user must be the same as the project's technician.
        var loggedInTechnician = await dbContext.Technicians
            .SingleOrDefaultAsync(x => x.AccountId == sessionContextProvider.User!.GetUserId(), ctx);

        if (loggedInTechnician is null || !project.CanBeEditedBy(loggedInTechnician))
            return Result.Unauthorized("You are not authorized to edit this project.");

        project.Edit(req.Name);

        await dbContext.SaveChangesAsync(ctx);
*/
        return Result.Success();
    }
}
