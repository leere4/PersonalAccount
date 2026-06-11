using PersonalAccount.Models;

namespace PersonalAccount.Services.Cabinet;

public interface IAdminCabinetService
{
	Task<List<AccountModel>> GetAllStudentAccountsAsync();
	Task<List<StudentProfileModel>> GetAllStudentProfilesAsync();
	Task<List<GroupModel>> GetAllGroupsAsync(); 
	Task ConfirmStudentEmailAsync(int accountId); 
}