using System.Runtime.CompilerServices;

namespace MonkeyFinder.ViewModel;

[INotifyPropertyChanged]
public partial class BaseViewModel
{
    public BaseViewModel()
    {
        
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;

    public bool IsNotBusy => !IsBusy;


}
