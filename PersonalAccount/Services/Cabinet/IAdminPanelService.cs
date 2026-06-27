using PersonalAccount.Models;
using PersonalAccount.Types;

namespace PersonalAccount.Services.Cabinet;

public interface IAdminPanelService
{
	Task<List<AccountModel>> GetAllAccountsAsync(AccountRoles role);
	Task<List<StudentProfileModel>> GetAllStudentProfilesAsync();
	Task<List<TeacherProfileModel>> GetAllTeacherProfilesAsync();
	Task<List<GroupModel>> GetAllGroupsAsync();
	Task<List<DisciplineModel>> GetAllDisciplinesAsync(); 
	Task<bool> CheckEmailUniqueAsync(string email);
	Task<string> RegisterAccountWithGeneratedPasswordAsync(string email, AccountRoles role);
	Task RegisterStudentProfileForEmailAsync(string email, string fullName);
	Task RegisterTeacherProfileForEmailAsync(string email, string fullName);
	Task AddTeacherGroupDiscipline(int teacherAccountId, int disciplineId, int groupId);
	Task RemoveTeacherGroupDiscipline(int teacherAccountId, int disciplineId, int groupId);
	Task AddGroupAsync(string name);
	Task AddDisciplineAsync(string name);
	Task ChangeStudentGroupAsync(int studentAccountId, int groupId);
	Task DeleteGroupAsync(int groupId);
	Task DeleteDisciplineAsync(int disciplineId);
	Task DeleteStudentAsync(int accountId);
	Task DeleteTeacherAsync(int accountId);
}