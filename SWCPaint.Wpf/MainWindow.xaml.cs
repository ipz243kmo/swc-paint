using System.Windows;

namespace SWCPaint.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}