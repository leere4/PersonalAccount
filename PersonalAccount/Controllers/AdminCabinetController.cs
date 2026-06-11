using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalAccount.Services.Cabinet;
using PersonalAccount.Services.Tokens;
using PersonalAccount.Types;
using PersonalAccount.ViewModels;

namespace PersonalAccount.Controllers;

[Authorize(Roles = AccountRoleConstants.Admin)]
public class AdminCabinetController(
	IAdminCabinetService cabinetService,
	IConfirmationTokenService confirmationTokenService) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var accountsList = await cabinetService.GetAllStudentAccountsAsync();
		var studentProfiles = await cabinetService.GetAllStudentProfilesAsync();
		var groupsList = await cabinetService.GetAllGroupsAsync();

		var accounts = accountsList.ToDictionary(acc => acc.Id);
		var groups = groupsList.ToDictionary(g => g.Id);

		var studentsViewModel = new List<AdminCabinetStudentViewModel>();

		foreach (var studentProfile in studentProfiles)
		{
			if (!accounts.TryGetValue(studentProfile.AccountId, out var account)) continue;

			var groupName = "Без группы";
			if (studentProfile.GroupId != null && groups.ContainsKey(studentProfile.GroupId.Value))
			{
				groupName = groups[studentProfile.GroupId.Value].Name;
			}

			var isConfirmed = await confirmationTokenService.HasConfirmedTokenAsync(studentProfile.AccountId);

			studentsViewModel.Add(new AdminCabinetStudentViewModel
			{
				AccountId = studentProfile.AccountId,
				FullName = studentProfile.FullName,
				GroupName = groupName,
				PhotoUrl = studentProfile.PhotoUrl?.ToString(),
				Email = account.Email,
				IsEmailConfirmed = isConfirmed
			});
		}

		return View(new AdminCabinetViewModel { Students = studentsViewModel });
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ConfirmStudentEmail(int id)
	{
		await cabinetService.ConfirmStudentEmailAsync(id);
		return RedirectToAction("Index");
	}
}