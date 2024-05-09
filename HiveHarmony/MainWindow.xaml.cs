using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using HexGridControl;

namespace HiveHarmony;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    enum windowThemes { dark, light }

    windowThemes currentTheme = windowThemes.dark;


    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y < 30)
            DragMove();
    }

    private void CommandBindingCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

    private void CloseWindow(object sender, ExecutedRoutedEventArgs e) => SystemCommands.CloseWindow(this);

    private void MaximizeAndRestoreWindow(object sender, ExecutedRoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            SystemCommands.RestoreWindow(this);
        else
            SystemCommands.MaximizeWindow(this);
    }

    private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e) => SystemCommands.MinimizeWindow(this);


    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.T)
        {
            currentTheme = currentTheme == windowThemes.dark ? windowThemes.light : windowThemes.dark;
            string ressourceName = currentTheme == windowThemes.dark ? "DarkModeColors.xaml" : "LightModeColors.xaml";
            ((App)Application.Current).ChangeDesign($"Themes/{ressourceName}");
        }

        if (e.Key == Key.P)
        {
            MessageBox.Show(Height + " x " + Width);
        }
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        CalenderMonthTextBlock.Text = DateTime.Today.ToString("MMMM");
        WeekdayHeaderTextblock.Text = $"{DateTime.Today.DayOfWeek}\n{DateTime.Today:G}";

        DateTime currentMonthStart = DateTime.Parse($"{1}.{DateTime.Today.Month}.{DateTime.Today.Year}");

        if (currentMonthStart.DayOfWeek != DayOfWeek.Monday)
            currentMonthStart = currentMonthStart.AddDays(-(int)currentMonthStart.DayOfWeek + 1);

        for (int y = 0; y < CalenderHexGrid.RowCount; y++)
        {
            for (int x = 0; x < CalenderHexGrid.ColumnCount; x++)
            {
                string gameButtonTemp = XamlWriter.Save(CalenderHexGrid.Children[0]);
                StringReader stringReader = new(gameButtonTemp);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                HexItem newHexItem = (HexItem)XamlReader.Load(xmlReader);

                TextBlock hexTextBlock = (TextBlock)((Grid)newHexItem.Content).Children[0];
                StackPanel hexStackPanel_appointment = (StackPanel)((Grid)newHexItem.Content).Children[1];

                if (y == 0)
                {
                    hexTextBlock.Text = ((DayOfWeek)((x + 1) % 7)).ToString()[..2];
                    newHexItem.Tag = currentMonthStart.DayOfWeek.ToString();
                    newHexItem.SetResourceReference(BackgroundProperty, "scb_Back_Primary_Mid");
                }
                else
                {
                    hexTextBlock.Text = currentMonthStart.ToString("dd");
                    newHexItem.Tag = $"{currentMonthStart:dd.MM.yyyy}";

                    if (currentMonthStart < DateTime.Today)
                        newHexItem.SetResourceReference(BackgroundProperty, "scb_Back_Primary_Low");
                    else if (currentMonthStart > DateTime.Today)
                        newHexItem.SetResourceReference(BackgroundProperty, "scb_Back_Primary_HighMid");
                    else
                        newHexItem.SetResourceReference(BackgroundProperty, "scb_Back_Primary_High");

                    if (currentMonthStart.Month != DateTime.Today.Month)
                        hexTextBlock.Opacity = 0.5;


                    Random random = new Random();
                    for (int i = 0; i < hexStackPanel_appointment.Children.Count; i++)
                    {
                        if (random.Next(10) < 2)
                            hexStackPanel_appointment.Children[i].Visibility = Visibility.Visible;
                    }

                }


                Grid.SetColumn(newHexItem, x);
                Grid.SetRow(newHexItem, y);

                CalenderHexGrid.Children.Add(newHexItem);

                if (y > 0)
                    currentMonthStart = currentMonthStart.AddDays(1);

                await Task.Delay(20);

                if (x >= CalenderHexGrid.ColumnCount - 1 && currentMonthStart.Month != DateTime.Today.Month && y > 1)
                    y = CalenderHexGrid.RowCount;

            }
        }
    }

    private void CalenderHex_Click(object sender, RoutedEventArgs e)
    {
        HexItem hexItem = FindParent<HexItem>((Button)sender);

        string data = hexItem.Tag.ToString() ?? "";

        if (DateTime.TryParse(data, out DateTime val))
            WeekdayHeaderTextblock.Text = $"{val.DayOfWeek}\n{val:G}";
    }

    private T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parentObject = VisualTreeHelper.GetParent(child);

        // Wenn das Elternelement null ist, gibt es kein passendes Element
        if (parentObject == null)
            return null;

        // Prüfe, ob das Elternelement den gewünschten Typ hat
        if (parentObject is T parent)
        {
            return parent;
        }
        else
        {
            // Rekursiv weiter nach oben im Baum suchen
            return FindParent<T>(parentObject);
        }
    }


}