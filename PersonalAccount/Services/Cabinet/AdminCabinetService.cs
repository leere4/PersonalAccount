using PersonalAccount.Models;
using PersonalAccount.Repositories;
using PersonalAccount.Services.Tokens; 
using PersonalAccount.Types;

namespace PersonalAccount.Services.Cabinet;

public class AdminCabinetService(
	IAccountRepo accountRepo,
	IStudentProfileRepo studentProfileRepo,
	IGroupRepo groupRepo,
	IConfirmationTokenService confirmationTokenService 
) : IAdminCabinetService
{
	public async Task<List<AccountModel>> GetAllStudentAccountsAsync() =>
		await accountRepo.GetAllByRole(AccountRoles.Student);

	public async Task<List<StudentProfileModel>> GetAllStudentProfilesAsync() =>
		await studentProfileRepo.GetAllAsync();

	public async Task<List<GroupModel>> GetAllGroupsAsync() => await groupRepo.GetAllAsync();

	
	public async Task ConfirmStudentEmailAsync(int accountId)
	{
		var token = await confirmationTokenService.GenerateTokenAsync(accountId);
		await confirmationTokenService.ValidateTokenAsync(accountId, token);
	}
}