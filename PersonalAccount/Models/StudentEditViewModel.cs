using System.ComponentModel.DataAnnotations;

namespace PersonalAccount.Models;

public class StudentEditViewModel
{
	[Required(ErrorMessage = "ФИО обязательно для заполнения")]
	public string FullName { get; set; } = string.Empty;

	[Required(ErrorMessage = "Название группы обязательно для заполнения")]
	public string GroupName { get; set; } = string.Empty;

	public string? PhotoUrl { get; set; }
}