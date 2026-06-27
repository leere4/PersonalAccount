using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalAccount.Constants;
using PersonalAccount.Services.Cabinet;
using PersonalAccount.Services.Smtp;
using PersonalAccount.Types;
using PersonalAccount.ViewModels;

namespace PersonalAccount.Controllers;

[Authorize(Roles = AccountRoleConstants.Admin)]
public class AdminPanelController(
	IAdminPanelService panelService,
	ITeacherCabinetService teacherCabinetService,
	ISmtpClientService smtpClientService
) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var accounts = (await panelService.GetAllAccountsAsync(AccountRoles.Student | AccountRoles.Teacher))
			.ToDictionary(account => account.Id);
		var groups = (await panelService.GetAllGroupsAsync())
			.ToDictionary(group => group.Id);
		var disciplines = await panelService.GetAllDisciplinesAsync(); 
		var studentProfiles = await panelService.GetAllStudentProfilesAsync();
		var teacherProfiles = await panelService.GetAllTeacherProfilesAsync();

		return View(new AdminPanelViewModel
		{
			Groups = groups.Values.ToList(), 
			Disciplines = disciplines,       
			Students = studentProfiles.Select(studentProfile => new AdminPanelStudentViewModel
			{
				AccountId = studentProfile.AccountId, 
				GroupId = studentProfile.GroupId,     
				FullName = studentProfile.FullName,
				GroupName = groups[studentProfile.GroupId].Name,
				PhotoUrl = studentProfile.PhotoUrl?.ToString(),
				Email = accounts[studentProfile.AccountId].Email
			}).ToList(),
			Teachers = teacherProfiles.Select(teacherProfile => new AdminPanelTeacherViewModel
			{
				AccountId = teacherProfile.AccountId,
				FullName = teacherProfile.FullName,
				PhotoUrl = teacherProfile.PhotoUrl?.ToString(),
				Email = accounts[teacherProfile.AccountId].Email
			}).ToList()
		});
	}



	[HttpGet]
	public IActionResult AddGroup() => View(new AddGroupViewModel());

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> AddGroup(AddGroupViewModel model)
	{
		if (!ModelState.IsValid) return View(model);
		await panelService.AddGroupAsync(model.Name);
		return RedirectToAction("Index");
	}

	[HttpGet]
	public IActionResult AddDiscipline() => View(new AddDisciplineViewModel());

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> AddDiscipline(AddDisciplineViewModel model)
	{
		if (!ModelState.IsValid) return View(model);
		await panelService.AddDisciplineAsync(model.Name);
		return RedirectToAction("Index");
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ChangeStudentGroup(int studentAccountId, int groupId)
	{
		await panelService.ChangeStudentGroupAsync(studentAccountId, groupId);
		return RedirectToAction("Index");
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteGroup(int groupId)
	{
		await panelService.DeleteGroupAsync(groupId);
		return RedirectToAction("Index");
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteDiscipline(int disciplineId)
	{
		await panelService.DeleteDisciplineAsync(disciplineId);
		return RedirectToAction("Index");
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteStudent(int accountId)
	{
		await panelService.DeleteStudentAsync(accountId);
		return RedirectToAction("Index");
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteTeacher(int accountId)
	{
		await panelService.DeleteTeacherAsync(accountId);
		return RedirectToAction("Index");
	}


	[HttpGet]
	public IActionResult RegisterStudent() => View(new RegisterStudentViewModel());

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> RegisterStudent(RegisterStudentViewModel model)
	{
		if (!ModelState.IsValid) return View(model);

		var isUnique = await panelService.CheckEmailUniqueAsync(model.Email);
		if (!isUnique)
		{
			ModelState.AddModelError(string.Empty, $"Email {model.Email} is already taken.");
			return View(model);
		}

		var password = await panelService.RegisterAccountWithGeneratedPasswordAsync(model.Email, AccountRoles.Student);
		await panelService.RegisterStudentProfileForEmailAsync(model.Email, model.FullName);

		await smtpClientService.SendEmailAsync(model.ContactEmail, "Данные для входа в систему", $"""
             <body>
                 <p>{model.Email}</p>
                 <p>{password}</p>
             </body>
             """);

		return RedirectToAction("Index");
	}

	[HttpGet]
	public IActionResult RegisterTeacher() => View(new RegisterTeacherViewModel());

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> RegisterTeacher(RegisterTeacherViewModel model)
	{
		if (!ModelState.IsValid) return View(model);

		var isUnique = await panelService.CheckEmailUniqueAsync(model.Email);
		if (!isUnique)
		{
			ModelState.AddModelError(string.Empty, $"Email {model.Email} is already taken.");
			return View(model);
		}

		var password = await panelService.RegisterAccountWithGeneratedPasswordAsync(model.Email, AccountRoles.Teacher);
		await panelService.RegisterTeacherProfileForEmailAsync(model.Email, model.FullName);

		await smtpClientService.SendEmailAsync(model.ContactEmail, "Данные для входа в систему", $"""
             <body>
                 <p>{model.Email}</p>
                 <p>{password}</p>
             </body>
             """);

		return RedirectToAction("Index");
	}

	[HttpGet]
	public async Task<IActionResult> EditTeacher(int teacherAccountId)
	{
		var teacherProfile = await teacherCabinetService.GetProfileAsync(teacherAccountId);
		if (teacherProfile == null) return RedirectToAction("Error", "Home");
		var groupsByDisciplines = await teacherCabinetService.GetGroupsByDisciplineAsync(teacherProfile.AccountId);
		var groups = await panelService.GetAllGroupsAsync();

		return View(new EditTeacherViewModel
		{
			AccountId = teacherAccountId,
			DisciplineIdsOrder = groupsByDisciplines.Keys
				.OrderBy(discipline => discipline.Name)
				.Select(discipline => discipline.Id).ToList(),
			Disciplines = groupsByDisciplines.Keys
				.ToDictionary(discipline => discipline.Id, discipline => new EditTeacherDisciplineViewModel
				{
					Id = discipline.Id,
					Name = discipline.Name,
				}),
			AllGroupOptions = groups.Select(group => new EditTeacherGroupOptionViewModel
			{
				Id = group.Id,
				Name = group.Name,
			}).Where(group => group.Id != GroupConstants.NoGroup.Id).ToList(),
			GroupsByDisciplines = groupsByDisciplines.ToDictionary(
				groupsByDiscipline => groupsByDiscipline.Key.Id,
				groupsByDiscipline => groupsByDiscipline.Value
					.Select(group => new EditTeacherGroupViewModel
					{
						Id = group.Id,
						Name = group.Name,
						ImageUrl = group.ImageUrl?.ToString(),
					})
					.ToList())
		});
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> AddTeacherGroupDiscipline(int teacherAccountId, int disciplineId, int groupId)
	{
		await panelService.AddTeacherGroupDiscipline(teacherAccountId, disciplineId, groupId);
		return RedirectToAction("EditTeacher", new { teacherAccountId });
	}
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> RemoveTeacherGroupDiscipline(int teacherAccountId, int disciplineId, int groupId)
	{
		await panelService.RemoveTeacherGroupDiscipline(teacherAccountId, disciplineId, groupId);
		return RedirectToAction("EditTeacher", new { teacherAccountId });
	}
}