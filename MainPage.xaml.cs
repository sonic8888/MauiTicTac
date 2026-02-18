using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace MauiTicTac;

public partial class MainPage : ContentPage
{
    private const int Rows = 20;
    private const int Cols = 20;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnGridTapped(object sender, TappedEventArgs e)
    {
        if (sender is GraphicsView graphicsView)
        {
            // Get the tap position
            var tapPoint = e.GetPosition(graphicsView);
            
            if (tapPoint != null)
            {
                // Use GridDrawer to convert point to cell
                var (row, col) = GridDrawer.Instance.GetCellFromPoint(tapPoint.Value);
                
                // Calculate cell index (0-399)
                int cellIndex = row * Cols + col;
                
                // Show message with cell number
                await DisplayAlert("Cell Clicked", $"You clicked cell {cellIndex} (Row: {row}, Col: {col})", "OK");
            }
        }
    }
}
