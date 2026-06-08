using PersonalAccount.Models.Student;

namespace PersonalAccount.Services.Profile;

public interface IStudentService
{
	Task<StudentModel?> GetByIdAsync(int id);
	Task UpdateByIdAsync(int id, StudentModel student);
}