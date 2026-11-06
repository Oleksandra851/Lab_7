using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Lab_7.Data;
using Lab_7.Models;
using Lab_7.Services;
using Microsoft.EntityFrameworkCore;

namespace Lab_7.ViewModels
{
    public class SubjectSelectionItem : BaseViewModel
    {
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;

        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }

        private int _hoursRead;
        public int HoursRead { get => _hoursRead; set => SetProperty(ref _hoursRead, value); }
    }

    [QueryProperty(nameof(TeacherID), nameof(TeacherID))]
    public class TeacherEditViewModel : BaseViewModel
    {
        private readonly TeacherService _teacherService;

        private int _teacherID;
        public int TeacherID
        {
            get => _teacherID;
            set
            {
                _teacherID = value;
                LoadTeacherData(value);
            }
        }

        private string _lastName = string.Empty;
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }

        private string _firstName = string.Empty;
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }

        private string _middleName = string.Empty;
        public string MiddleName { get => _middleName; set => SetProperty(ref _middleName, value); }

        private string _phone = string.Empty;
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }

        private string _workplace = string.Empty;
        public string Workplace { get => _workplace; set => SetProperty(ref _workplace, value); }

        private string _homeAddress = string.Empty;
        public string HomeAddress { get => _homeAddress; set => SetProperty(ref _homeAddress, value); }

        private string _characteristic = string.Empty;
        public string Characteristic { get => _characteristic; set => SetProperty(ref _characteristic, value); }

        private Position? _selectedPosition;
        public Position? SelectedPosition { get => _selectedPosition; set => SetProperty(ref _selectedPosition, value); }

        public ObservableCollection<Position> Positions { get; } = new();
        public ObservableCollection<SubjectSelectionItem> SubjectSelections { get; } = new();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public TeacherEditViewModel()
        {
            _teacherService = new TeacherService();
            Title = "Додавання викладача";

            SaveCommand = new Command(async () => await OnSave());
            CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

            _ = InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            await LoadPositionsAsync();
            if (TeacherID == 0)
            {
                await LoadSubjectsAsync(null);
            }
        }

        private async Task LoadPositionsAsync()
        {
            Positions.Clear();
            var posList = await _teacherService.GetPositionsAsync();
            foreach (var p in posList)
            {
                Positions.Add(p);
            }
        }

        private async Task LoadSubjectsAsync(List<TeacherSubject>? existingSubjects)
        {
            SubjectSelections.Clear();
            using var context = new UniversityDbContext();
            var allSubjects = await context.Subjects.ToListAsync();

            var existingDict = existingSubjects?.ToDictionary(ts => ts.SubjectID, ts => ts.HoursRead)
                               ?? new Dictionary<int, int>();

            foreach (var s in allSubjects)
            {
                bool isSelected = existingDict.ContainsKey(s.SubjectID);
                int hours = isSelected ? existingDict[s.SubjectID] : 0;

                SubjectSelections.Add(new SubjectSelectionItem
                {
                    SubjectID = s.SubjectID,
                    SubjectName = s.SubjectName,
                    IsSelected = isSelected,
                    HoursRead = hours
                });
            }
        }

        private async void LoadTeacherData(int id)
        {
            if (id <= 0) return;

            Title = "Редагування викладача";
            var t = await _teacherService.GetTeacherByIdAsync(id);
            if (t != null)
            {
                LastName = t.LastName;
                FirstName = t.FirstName;
                MiddleName = t.MiddleName ?? "";
                Phone = t.Phone ?? "";
                Workplace = t.Workplace ?? "";
                HomeAddress = t.HomeAddress ?? "";
                Characteristic = t.Characteristic ?? "";

                await LoadPositionsAsync();
                SelectedPosition = Positions.FirstOrDefault(p => p.PositionID == t.PositionID);

                await LoadSubjectsAsync(t.TeacherSubjects?.ToList());
            }
        }

        private async Task OnSave()
        {
            if (string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(FirstName))
            {
                await Shell.Current.DisplayAlertAsync("Помилка", "Прізвище та Ім'я є обов'язковими!", "OK");
                return;
            }

            if (SelectedPosition == null)
            {
                await Shell.Current.DisplayAlertAsync("Помилка", "Будь ласка, оберіть посаду!", "OK");
                return;
            }

            using var context = new UniversityDbContext();

            Teacher teacher;
            if (TeacherID == 0)
            {
                teacher = new Teacher();
                context.Teachers.Add(teacher);
            }
            else
            {
                teacher = await context.Teachers.Include(t => t.TeacherSubjects).FirstOrDefaultAsync(t => t.TeacherID == TeacherID);

                if (teacher == null) return;
                context.TeacherSubjects.RemoveRange(teacher.TeacherSubjects);
            }

            teacher.LastName = LastName;
            teacher.FirstName = FirstName;
            teacher.MiddleName = MiddleName;
            teacher.Phone = Phone;
            teacher.Workplace = Workplace;
            teacher.PositionID = SelectedPosition.PositionID;
            teacher.HomeAddress = HomeAddress;
            teacher.Characteristic = Characteristic;

            await context.SaveChangesAsync();
            foreach (var item in SubjectSelections)
            {
                if (item.IsSelected)
                {
                    context.TeacherSubjects.Add(new TeacherSubject
                    {
                        TeacherID = teacher.TeacherID,
                        SubjectID = item.SubjectID,
                        HoursRead = item.HoursRead < 0 ? 0 : item.HoursRead
                    });
                }
            }

            await context.SaveChangesAsync();
            await Shell.Current.GoToAsync("..");
        }
    }
}