using Lab_7.Models;
using Lab_7.Services;

namespace Lab_7.ViewModels
{
    [QueryProperty(nameof(TeacherID), nameof(TeacherID))]
    public class TeacherDetailsViewModel : BaseViewModel
    {
        private readonly TeacherService _teacherService;

        private int _teacherID;
        public int TeacherID
        {
            get => _teacherID;
            set
            {
                _teacherID = value;
                LoadTeacherDetails(value);
            }
        }

        private Teacher? _teacher;
        public Teacher? Teacher
        {
            get => _teacher;
            set => SetProperty(ref _teacher, value);
        }

        public TeacherDetailsViewModel()
        {
            Title = "Деталі викладача";
            _teacherService = new TeacherService();
        }

        private async void LoadTeacherDetails(int id)
        {
            Teacher = await _teacherService.GetTeacherByIdAsync(id);
        }
    }
}