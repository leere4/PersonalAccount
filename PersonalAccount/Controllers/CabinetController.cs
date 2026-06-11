using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalAccount.Models;
using PersonalAccount.Models.Student;
using PersonalAccount.Services.Profile;
using PersonalAccount.Utils;

namespace PersonalAccount.Controllers;

[Authorize]
public class CabinetController(IStudentService students) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		
		var studentId = User.GetId();
		if (studentId == null) return RedirectToAction("Error", "Home");

		
		var student = await students.GetByIdAsync(studentId.Value);
		if (student == null) return RedirectToAction("Error", "Home");

		return View(student);
	}

	[HttpGet]
	public async Task<IActionResult> Edit()
	{
		var studentId = User.GetId();
		if (studentId == null) return RedirectToAction("Error", "Home");

		var student = await students.GetByIdAsync(studentId.Value);
		if (student == null) return RedirectToAction("Error", "Home");

		return View(new StudentEditViewModel
		{
			
			FullName = student.FullName,
			GroupName = student.GroupName,
			PhotoUrl = student.PhotoUrl?.ToString()
		});
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(StudentEditViewModel model)
	{
		
		if (!ModelState.IsValid) return View(model);

		var studentId = User.GetId();
		if (studentId == null) return RedirectToAction("Error", "Home");

		
		Uri? photoUri = null;
		if (!string.IsNullOrEmpty(model.PhotoUrl))
		{
			Uri.TryCreate(model.PhotoUrl, UriKind.Absolute, out photoUri);
		}

		
		var studentToUpdate = new StudentModel
		{
			FullName = model.FullName,
			GroupName = model.GroupName,
			PhotoUrl = photoUri
		};

		await students.UpdateByIdAsync(studentId.Value, studentToUpdate);

		return RedirectToAction("Index");
	}
}