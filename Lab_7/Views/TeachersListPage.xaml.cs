using Lab_7.ViewModels;

namespace Lab_7.Views
{
    public partial class TeachersListPage : ContentPage
    {
        public TeachersListPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is TeachersListViewModel vm)
            {
                await vm.LoadTeachersAsync();
            }
        }
    }
}