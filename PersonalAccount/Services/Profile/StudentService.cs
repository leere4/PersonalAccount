using PersonalAccount.Models.Student;
using PersonalAccount.Repository;

namespace PersonalAccount.Services.Profile;

public class StudentService(IStudentRepo<StudentModel> repo) : IStudentService
{
	public async Task<StudentModel?> GetByIdAsync(int id)
	{
		return await repo.GetByIdAsync(id);
	}

	public async Task UpdateByIdAsync(int id, StudentModel student)
	{
		await repo.UpdateByIdAsync(id, student);
	}
}