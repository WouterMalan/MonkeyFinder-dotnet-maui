﻿using System.Net.Http.Json;
using Microsoft.Maui.Devices.Sensors;
using MonkeyFinder.Services;

namespace MonkeyFinder.ViewModel;

public partial class MonkeysViewModel : BaseViewModel
{
    MonkeyService monkeyService;

    public ObservableCollection<Monkey> Monkeys { get; set; }= new ObservableCollection<Monkey>();

    IConnectivity connectivity;
    IGeolocation geolocation;

    [ObservableProperty]
    bool isRefreshing;  


    public MonkeysViewModel(MonkeyService monkeyService, IConnectivity connectivity, IGeolocation geolocation)
    {
        Title = "Monkey Finder";
        this.monkeyService = monkeyService;
        this.connectivity = connectivity;
        this.geolocation = geolocation;

    }



    [RelayCommand]
    async Task GetClosestMonkey()
    {

        if (IsBusy || Monkeys.Count == 0)
        {
            if (Monkeys.Count == 0)
            {
                await Shell.Current.DisplayAlert("Error", "No monkeys in database, please press Get Monkeys button", "OK");
            }
            return;
        }

        try
        {
            var location = await geolocation.GetLastKnownLocationAsync();
            if (location is null)
            {
                location = await geolocation.GetLocationAsync(
                    new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30),
                    });
                
            }

            if (location is null)
            {
                return;
            }

            var first = Monkeys.OrderBy(x => location.CalculateDistance(x.Latitude, x.Longitude, DistanceUnits.Kilometers)).FirstOrDefault();

            if (first is null)
            {
                return;
            }

            await Shell.Current.DisplayAlert("Closest Monkey", $"{first.Name} in {first.Location}", "OK");

        }
        catch (Exception ex)
        {

            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error", $"Unable to get closest monkey {ex.Message}", "OK");
        }
    }




    [RelayCommand]
    async Task GoToDetailsAsync(Monkey monkey)
    {
        if (monkey is null)
        {
            return;
        }

        await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true, 
            new Dictionary<string, object>
            {
                {"Monkey" , monkey }
            });
    }



    [RelayCommand]
    async Task GetMonkeysAsync()
    {
        if (IsBusy) return;

        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Internet Connection", "Check your internet and try again", "OK");
            }

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
            IsRefreshing = false;
        }
    }

}
