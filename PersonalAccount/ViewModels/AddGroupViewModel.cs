using System.ComponentModel.DataAnnotations;

namespace PersonalAccount.ViewModels;

public class AddGroupViewModel : ViewModel
{
	[Required(ErrorMessage = "Введите название группы")]
	public string Name { get; set; } = string.Empty;
}