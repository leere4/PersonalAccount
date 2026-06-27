using System.ComponentModel.DataAnnotations;

namespace PersonalAccount.ViewModels;

public class AddDisciplineViewModel : ViewModel
{
	[Required(ErrorMessage = "Введите название дисциплины")]
	public string Name { get; set; } = string.Empty;
}