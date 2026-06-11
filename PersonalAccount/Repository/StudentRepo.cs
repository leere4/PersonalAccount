using Microsoft.EntityFrameworkCore;
using PersonalAccount.Data;
using PersonalAccount.Data.Entities;
using PersonalAccount.Mappers;
using PersonalAccount.Models.Student;

namespace PersonalAccount.Repository;

public class StudentRepo<T>(AppDbContext ctx, IMapper<StudentEntity, T> mapper) : IStudentRepo<T> where T : StudentModel
{
	private DbSet<StudentEntity> Students => ctx.Students;

	public async Task<T?> GetByEmailAsync(string email)
	{
		var entity = await Students
			.AsNoTracking()
			.FirstOrDefaultAsync(entity => entity.Email == email);

		return entity == null ? null : mapper.ToModel(entity);
	}

	public async Task<T?> GetByIdAsync(int id)
	{
		var entity = await Students.FindAsync(id);
		return entity == null ? null : mapper.ToModel(entity);
	}

	
	public async Task UpdateByIdAsync(int id, StudentModel student)
	{
		var entity = await Students.FindAsync(id);
		if (entity == null) return;

		entity.FullName = student.FullName;
		entity.GroupName = student.GroupName;

		entity.PhotoUrl = student.PhotoUrl?.ToString();

		await ctx.SaveChangesAsync();
	}
}