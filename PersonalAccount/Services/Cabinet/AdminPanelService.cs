using Microsoft.AspNetCore.Identity;
using PersonalAccount.Constants;
using PersonalAccount.Models;
using PersonalAccount.Repositories;
using PersonalAccount.Types;
using System.Security.Cryptography;

namespace PersonalAccount.Services.Cabinet;

public class AdminPanelService(
	IAccountRepo accountRepo,
	IStudentProfileRepo studentProfileRepo,
	ITeacherProfileRepo teacherProfileRepo,
	ITeacherGroupDisciplineRepo teacherGroupDisciplineRepo,
	IGroupRepo groupRepo,
	IDisciplineRepo disciplineRepo, 
	IPasswordHasher<AccountModel> hasher
) : IAdminPanelService
{
	public async Task<List<AccountModel>> GetAllAccountsAsync(AccountRoles role) =>
		await accountRepo.GetAllByRoleAsync(role);

	public async Task<List<StudentProfileModel>> GetAllStudentProfilesAsync() =>
		await studentProfileRepo.GetAllAsync();

	public async Task<List<TeacherProfileModel>> GetAllTeacherProfilesAsync() => await teacherProfileRepo.GetAllAsync();

	public async Task<List<GroupModel>> GetAllGroupsAsync() => await groupRepo.GetAllAsync();

	public async Task<List<DisciplineModel>> GetAllDisciplinesAsync() => await disciplineRepo.GetAllAsync(); 

	public async Task<bool> CheckEmailUniqueAsync(string email)
	{
		var account = await accountRepo.GetByEmailAsync(email);
		return account == null;
	}

	public async Task<string> RegisterAccountWithGeneratedPasswordAsync(string email, AccountRoles role)
	{
		var password = RandomNumberGenerator.GetHexString(12);
		var account = new AccountModel
		{
			Email = email,
			Role = role,
		};
		account.PasswordHash = hasher.HashPassword(account, password);
		await accountRepo.AddAsync(account);
		return password;
	}

	public async Task RegisterStudentProfileForEmailAsync(string email, string fullName) =>
		await RegisterProfileForEmailAsync(studentProfileRepo, email, fullName);

	public async Task RegisterTeacherProfileForEmailAsync(string email, string fullName) =>
		await RegisterProfileForEmailAsync(teacherProfileRepo, email, fullName);

	public async Task AddTeacherGroupDiscipline(int teacherAccountId, int disciplineId, int groupId) =>
		await teacherGroupDisciplineRepo.AddAsync(new TeacherGroupDisciplineModel
		{
			TeacherAccountId = teacherAccountId,
			DisciplineId = disciplineId,
			GroupId = groupId
		});

	public async Task RemoveTeacherGroupDiscipline(int teacherAccountId, int disciplineId, int groupId) =>
		await teacherGroupDisciplineRepo.RemoveByTeacherAccountIdAndGroupIdAndDisciplineIdAsync(teacherAccountId,
			disciplineId, groupId);


	public async Task AddGroupAsync(string name) =>
		await groupRepo.AddAsync(new GroupModel { Name = name });

	public async Task AddDisciplineAsync(string name) =>
		await disciplineRepo.AddAsync(new DisciplineModel { Name = name });

	public async Task ChangeStudentGroupAsync(int studentAccountId, int groupId) =>
		await studentProfileRepo.UpdateGroupByAccountIdAsync(studentAccountId, groupId);

	public async Task DeleteGroupAsync(int groupId)
	{
		if (groupId == GroupConstants.NoGroup.Id) return;
		await groupRepo.RemoveByIdAsync(groupId);
	}

	public async Task DeleteDisciplineAsync(int disciplineId) =>
		await disciplineRepo.RemoveByIdAsync(disciplineId);

	public async Task DeleteStudentAsync(int accountId) =>
		await accountRepo.RemoveByIdAsync(accountId);

	public async Task DeleteTeacherAsync(int accountId) =>
		await accountRepo.RemoveByIdAsync(accountId);


	private async Task RegisterProfileForEmailAsync<TProfileModel>(IProfileRepo<TProfileModel> profileRepo,
		string email, string fullName) where TProfileModel : ProfileModel, new()
	{
		var account = await accountRepo.GetByEmailAsync(email);
		if (account == null) throw new KeyNotFoundException($"No account with email {email} found.");

		var studentProfile = new TProfileModel
		{
			FullName = fullName,
			AccountId = account.Id,
		};
		await profileRepo.AddAsync(studentProfile);
	}
}