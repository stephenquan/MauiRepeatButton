// MainPage.xaml.cs
using CommunityToolkit.Mvvm.Input;
namespace MauiRepeatButton
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        public MainPage()
        {
            InitializeComponent();
        }
        [RelayCommand]
        private void IncrementCounter()
        {
            count++;
            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";
            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}