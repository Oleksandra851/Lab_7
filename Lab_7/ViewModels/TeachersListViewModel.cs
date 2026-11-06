using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Lab_7.Models;
using Lab_7.Services;
using Lab_7.Views;

namespace Lab_7.ViewModels
{
    public class TeachersListViewModel : BaseViewModel
    {
        private readonly TeacherService _teacherService;
        public ObservableCollection<Teacher> Teachers { get; } = new();

        public ICommand LoadTeachersCommand { get; }
        public ICommand AddTeacherCommand { get; }
        public ICommand EditTeacherCommand { get; }
        public ICommand DeleteTeacherCommand { get; }
        public ICommand DetailsTeacherCommand { get; }

        public TeachersListViewModel()
        {
            Title = "Викладачі університету (MAUI)";
            _teacherService = new TeacherService();

            LoadTeachersCommand = new Command(async () => await LoadTeachersAsync());
            AddTeacherCommand = new Command(async () => await OnAddTeacher());
            EditTeacherCommand = new Command<Teacher>(async (t) => await OnEditTeacher(t));
            DeleteTeacherCommand = new Command<Teacher>(async (t) => await OnDeleteTeacher(t));
            DetailsTeacherCommand = new Command<Teacher>(async (t) => await OnDetailsTeacher(t));
        }

        public async Task LoadTeachersAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                Teachers.Clear();
                var list = await _teacherService.GetTeachersAsync();
                foreach (var item in list)
                {
                    Teachers.Add(item);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnAddTeacher()
        {
            await Shell.Current.GoToAsync(nameof(TeacherEditPage));
        }

        private async Task OnEditTeacher(Teacher teacher)
        {
            if (teacher == null) return;
            await Shell.Current.GoToAsync($"{nameof(TeacherEditPage)}?TeacherID={teacher.TeacherID}");
        }

        private async Task OnDetailsTeacher(Teacher teacher)
        {
            if (teacher == null) return;
            await Shell.Current.GoToAsync($"{nameof(TeacherDetailsPage)}?TeacherID={teacher.TeacherID}");
        }

        private async Task OnDeleteTeacher(Teacher teacher)
        {
            if (teacher == null) return;
            bool confirm = await Shell.Current.DisplayAlertAsync("Підтвердження", 
                $"Ви дійсно бажаєте видалити викладача {teacher.FullName}?", "Так", "Ні");

            if (confirm)
            {
                await _teacherService.DeleteTeacherAsync(teacher.TeacherID);
                await LoadTeachersAsync();
            }
        }
    }
}