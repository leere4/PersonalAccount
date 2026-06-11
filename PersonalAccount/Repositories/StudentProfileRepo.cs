using Microsoft.EntityFrameworkCore;
using PersonalAccount.Data;
using PersonalAccount.Data.Entities;
using PersonalAccount.Mappers;
using PersonalAccount.Models;

namespace PersonalAccount.Repositories;

public class StudentProfileRepo(AppDbContext ctx, IMapper<StudentProfileEntity, StudentProfileModel> mapper) : IStudentProfileRepo
{
	private DbSet<StudentProfileEntity> StudentProfiles => ctx.StudentProfiles;

	public async Task<StudentProfileModel?> GetByAccountIdAsync(int accountId)
	{
		var entity = await StudentProfiles.AsNoTracking().FirstOrDefaultAsync(e => e.AccountId == accountId);
		return entity == null ? null : mapper.ToModel(entity);
	}

	public async Task<List<StudentProfileModel>> GetAllAsync()
	{
		var entities = await StudentProfiles.AsNoTracking().ToListAsync();
		return entities.Select(e => mapper.ToModel(e)).ToList();
	}
}