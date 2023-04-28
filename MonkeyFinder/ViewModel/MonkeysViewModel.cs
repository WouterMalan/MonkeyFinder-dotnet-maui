using System.Net.Http.Json;
using MonkeyFinder.Services;

namespace MonkeyFinder.ViewModel;

public partial class MonkeysViewModel : BaseViewModel
{
    MonkeyService monkeyService;


    public ObservableCollection<Monkey> Monkeys { get; set; }= new ObservableCollection<Monkey>();
    
    public Command GetMonkeysCommand { get; set; }
    public MonkeysViewModel(MonkeyService monkeyService)
    {
        Title = "Monkey Finder";
        this.monkeyService = monkeyService;
        
    }

    [RelayCommand]
    async Task GetMonkeysAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var monkeys = await monkeyService.GetMonkeysAsync();

            if (monkeys.Count != 0)
            {
                Monkeys.Clear();

            }

            foreach (var monkey in monkeys)
            {
                Monkeys.Add(monkey);
            }
        }
        catch (Exception ex)
        {

            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error", $"Unable to get monkeys: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

}
