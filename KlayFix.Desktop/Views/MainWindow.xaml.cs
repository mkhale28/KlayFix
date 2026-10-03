using System.Windows;
using System.Windows.Media.Animation;
using KlayFix.Desktop.ViewModels;

namespace KlayFix.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedViewModel))
                AnimatePageIn();
        };

        viewModel.LogoutRequested += () =>
        {
            Close();
        };
    }

    private void AnimatePageIn()
    {
        var storyboard = new Storyboard();
        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(System.TimeSpan.FromMilliseconds(250)),
            EasingFunction = new PowerEase { Power = 2 }
        };
        var slideUp = new DoubleAnimation
        {
            From = 20,
            To = 0,
            Duration = new Duration(System.TimeSpan.FromMilliseconds(250)),
            EasingFunction = new PowerEase { Power = 2 }
        };

        Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));
        Storyboard.SetTargetProperty(slideUp, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));

        storyboard.Children.Add(fadeIn);
        storyboard.Children.Add(slideUp);

        if (PageContent.RenderTransform is not System.Windows.Media.TranslateTransform)
            PageContent.RenderTransform = new System.Windows.Media.TranslateTransform();

        PageContent.BeginStoryboard(storyboard);
    }
}
