// RepeatButton.cs
using System.Windows.Input;
namespace MauiRepeatButton;
public partial class RepeatButton : Button
{
    public static BindableProperty AutoRepeatDelayProperty = BindableProperty.Create(nameof(AutoRepeatDelay), typeof(int), typeof(RepeatButton), 400);
    public int AutoRepeatDelay
    {
        get => (int)GetValue(AutoRepeatDelayProperty);
        set => SetValue(AutoRepeatDelayProperty, value);
    }

    public static BindableProperty AutoRepeatIntervalProperty = BindableProperty.Create(nameof(AutoRepeatInterval), typeof(int), typeof(RepeatButton), 50);
    public int AutoRepeatInterval
    {
        get => (int)GetValue(AutoRepeatIntervalProperty);
        set => SetValue(AutoRepeatIntervalProperty, value);
    }

    public static BindableProperty FinishRepeatingThresholdProperty = BindableProperty.Create(nameof(FinishRepeatingThreshold), typeof(int), typeof(RepeatButton), 200);
    public int FinishRepeatingThreshold
    {
        get => (int)GetValue(FinishRepeatingThresholdProperty);
        set => SetValue(FinishRepeatingThresholdProperty, value);
    }

    public static BindableProperty RepeatCommandProperty = BindableProperty.Create(nameof(RepeatCommand), typeof(ICommand), typeof(RepeatButton));
    public ICommand? RepeatCommand
    {
        get => (ICommand)GetValue(RepeatCommandProperty);
        set => SetValue(RepeatCommandProperty, value);
    }

    public event EventHandler Repeat;

    private bool _isRepeating = false;
    public bool IsRepeating
    {
        get => _isRepeating;
        internal set
        {
            _isRepeating = value;
            OnPropertyChanged(nameof(IsRepeating));
        }
    }

    private long _finishedTime = 0;
    public long FinishedTime
    {
        get => _finishedTime;
        internal set
        {
            _finishedTime = value;
            OnPropertyChanged(nameof(FinishedTime));
        }
    }

    private CancellationTokenSource? _cts;
    public CancellationTokenSource? Cts
    {
        get => _cts;
        internal set
        {
            _cts = value;
            OnPropertyChanged(nameof(Cts));
        }
    }

    public RepeatButton()
	{
        this.Pressed += RepeatButton_Pressed;
        this.Released += RepeatButton_Released;
        this.Clicked += RepeatButton_Clicked;
	}

    private async void RepeatButton_Clicked(object? sender, EventArgs e)
    {
        await Task.Delay(AutoRepeatInterval);
        if (!IsRepeating || (DateTime.Now.Ticks - FinishedTime) <= FinishRepeatingThreshold * 10000)
        {
            return;
        }
        Repeat?.Invoke(this, EventArgs.Empty);
        if (RepeatCommand != null && RepeatCommand.CanExecute(null)) RepeatCommand?.Execute(null);
    }

    private void RepeatButton_Released(object? sender, EventArgs e)
    {
        Cts?.Cancel();
    }

    private async void RepeatButton_Pressed(object? sender, EventArgs e)
    {
        IsRepeating = true;
        Cts = new CancellationTokenSource();
        try
        {
            Repeat?.Invoke(this, EventArgs.Empty);
            if (RepeatCommand != null && RepeatCommand.CanExecute(null)) RepeatCommand?.Execute(null);
            await Task.Delay(AutoRepeatDelay, Cts.Token);
            while (!Cts.IsCancellationRequested)
            {
                Repeat?.Invoke(this, EventArgs.Empty);
                if (RepeatCommand != null && RepeatCommand.CanExecute(null)) RepeatCommand?.Execute(null);
                await Task.Delay(AutoRepeatInterval, Cts.Token);
            };
        }
        catch (TaskCanceledException)
        {
        }
        IsRepeating = false;
        FinishedTime = DateTime.Now.Ticks;
    }
}