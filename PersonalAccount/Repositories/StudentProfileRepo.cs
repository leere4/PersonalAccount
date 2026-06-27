using Microsoft.EntityFrameworkCore;
using PersonalAccount.Constants;
using PersonalAccount.Data;
using PersonalAccount.Data.Entities;
using PersonalAccount.Mappers;
using PersonalAccount.Models;

namespace PersonalAccount.Repositories;

public class StudentProfileRepo(
	AppDbContext ctx,
	IMapper<StudentProfileEntity, StudentProfileModel> mapper
) : ProfileRepo<StudentProfileEntity, StudentProfileModel>(ctx, mapper, c => c.StudentProfiles),
	IStudentProfileRepo
{
	public async Task UpdateGroupByAccountIdAsync(int accountId, int groupId)
	{
		
		var entity = await Ctx.StudentProfiles.FirstOrDefaultAsync(p => p.AccountId == accountId)
					 ?? throw new KeyNotFoundException("Студент не найден");
		entity.GroupId = groupId == GroupConstants.NoGroup.Id ? null : groupId;

		await Ctx.SaveChangesAsync();
	}
}