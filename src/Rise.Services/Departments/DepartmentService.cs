using Microsoft.EntityFrameworkCore;
using Rise.Domain.Departments;
using Rise.Persistence;
using Rise.Shared.Common;
using Rise.Shared.Departments;
using Rise.Shared.Users;

namespace Rise.Services.Departments;

/// <summary>
/// Service for departments.
/// </summary>
/// <param name="dbContext"></param>
public class DepartmentService(ApplicationDbContext dbContext) : IDepartmentService
{
    public async Task<Result<DepartmentResponse.Create>> CreateAsync(DepartmentRequest.Create request,
        CancellationToken ctx = default)
    {
        if (await dbContext.Departments.AnyAsync(x => x.Name == request.Name))
        {
            Log.Warning("Department with name '{Name}' already exists.", request.Name);
            return Result.Conflict($"Department with name '{request.Name}'  already exists.");
        }

        var d = new Department
        {
            Name = request.Name!,
            Description = request.Description!
        };

        dbContext.Departments.Add(d);

        await dbContext.SaveChangesAsync(ctx);

        return Result.Created(new DepartmentResponse.Create
        {
            DepartmentId = d.Id,
        });
    }

    public async Task<Result<DepartmentResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request,
        CancellationToken ctx)
    {
        var query = dbContext.Departments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p => p.Name.Contains(request.SearchTerm) || p.Description.Contains(request.SearchTerm));
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
            query = query.OrderBy(d => d.Name);
        }

        var departments = await query.AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(d => new DepartmentDto.Index
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                DepartmentType = d.DepartmentType.ToString(),
                Manager = d.Manager == null
                    ? null
                    : new UserDto.Index
                    {
                        Id = d.Manager.Id,
                        Email = d.Manager.Email.Value,
                        FirstName = d.Manager.Firstname,
                        LastName = d.Manager.Lastname,
                        Title = d.Manager.Title,
                    }

            })
            .ToListAsync(ctx);

        return Result.Success(new DepartmentResponse.Index
        {
            Departments = departments,
            TotalCount = totalCount
        });
    }
}