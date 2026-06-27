using PersonalAccount.Models;

namespace PersonalAccount.ViewModels;

public class AdminPanelStudentViewModel : ViewModel
{
	
	public int AccountId { get; set; }
	public int GroupId { get; set; }
	public string FullName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string GroupName { get; set; } = string.Empty;
	public string? PhotoUrl { get; set; }
}

public class AdminPanelTeacherViewModel : ViewModel
{
	public string FullName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string? PhotoUrl { get; set; }
	public int AccountId { get; set; }
}

public class AdminPanelViewModel : ViewModel
{
	public List<AdminPanelStudentViewModel> Students { get; set; } = [];
	public List<AdminPanelTeacherViewModel> Teachers { get; set; } = [];
	public List<GroupModel> Groups { get; set; } = [];
	public List<DisciplineModel> Disciplines { get; set; } = [];
}