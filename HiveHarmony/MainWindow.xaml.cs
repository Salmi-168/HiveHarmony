using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using HexGridControl;
using HiveHarmony.Styles.Themes;

namespace HiveHarmony;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private WindowThemes _currentTheme = WindowThemes.Dark;

    public MainWindow()
    {
        InitializeComponent();
    }

    #region Toolbar

    private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
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

    #endregion


    #region Debugging / Testing

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e)
        {
            case { Key: Key.T }:
            {
                _currentTheme = _currentTheme == WindowThemes.Dark ? WindowThemes.Light : WindowThemes.Dark;
                string resourceName =
                    _currentTheme == WindowThemes.Dark ? "DarkModeColors.xaml" : "LightModeColors.xaml";
                ((App)Application.Current).ChangeDesign($"Themes/{resourceName}");
                break;
            }
            case { Key: Key.P }:
                MessageBox.Show(Height + " x " + Width);
                break;
        }
    }

    #endregion

    #region Window Functions

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await Load_Calendar();
        Load_TodoList();
        Load_Groups();
    }

    #endregion

    #region Extra Funxtions

    private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject? parentObject = VisualTreeHelper.GetParent(child);

        return parentObject switch
        {
            // Wenn das Elternelement null ist, gibt es kein passendes Element
            null => null,
            // Prüfe, ob das Elternelement den gewünschten Typ hat
            T parent => parent,
            _ => FindParent<T>(parentObject)
        };
    }

    #endregion

    #region Loading Functions

    private async Task Load_Calendar()
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
                string calendarDayTemp = XamlWriter.Save(CalenderHexGrid.Children[0]);
                StringReader stringReader = new(calendarDayTemp);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                HexItem newHexItem = (HexItem)XamlReader.Load(xmlReader);

                TextBlock hexTextBlock = (TextBlock)((Grid)newHexItem.Content).Children[0];
                StackPanel hexStackPanelAppointment = (StackPanel)((Grid)newHexItem.Content).Children[1];

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

                    // Check if something is planned on this day
                    if (Random.Shared.Next(100) < 33) // Just for testing
                    {
                        Border borderTemplate =
                            (Border)((StackPanel)((Grid)newHexItem.Content).Children[1]).Children[0];
                        /* if more than one is needed this can be used to clone the bar
                        string gameButtonTemp = XamlWriter.Save(borderTemplate);
                        stringReader = new(gameButtonTemp);
                        xmlReader = XmlReader.Create(stringReader);
                        Border newBorder = (Border)XamlReader.Load(xmlReader);*/

                        borderTemplate.Visibility = Visibility.Visible;

                        borderTemplate.Background = Random.Shared.Next(2) == 0 ? Brushes.LightBlue : Brushes.LightCoral;
                    }
                }

                Grid.SetColumn(newHexItem, x);
                Grid.SetRow(newHexItem, y);

                newHexItem.Visibility = Visibility.Visible;
                CalenderHexGrid.Children.Add(newHexItem);

                if (y > 0)
                    currentMonthStart = currentMonthStart.AddDays(1);

                await Task.Delay(10);

                if (x >= CalenderHexGrid.ColumnCount - 1 && currentMonthStart.Month != DateTime.Today.Month && y > 1)
                    y = CalenderHexGrid.RowCount;
            }
        }
    }

    private void Load_TodoList()
    {
        // Load Todos from somewhere
        TodoListTextBlock.Text = "- Arbeit\n- Essen";
        TodoListTextBlock.FontFamily = new("Tahoma");
    }

    private void Load_Groups()
    {
        // Load Data
        string[] groups = ["Sport", "Sushi"];

        foreach (string group in groups)
        {
            string groupTemp = XamlWriter.Save(GroupsPanel.Children[0]);
            StringReader stringReader = new(groupTemp);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Border newBorder = (Border)XamlReader.Load(xmlReader);

            ((CheckBox)((Grid)newBorder.Child).Children[0]).Content = group;
            ((CheckBox)((Grid)newBorder.Child).Children[0]).FontFamily = new("Tahoma");

            ((CheckBox)((Grid)newBorder.Child).Children[0]).IsChecked = Random.Shared.Next(2) == 0;

            // Load People
            string[] people =
            [
                "https://api.suhri.de/user/picture/3/128.png", "https://api.suhri.de/user/picture/1/128.png"
            ]; // Just for testing

            foreach (string p in people)
            {
                string personTemp = XamlWriter.Save(((WrapPanel)((Grid)newBorder.Child).Children[1]).Children[0]);
                stringReader = new(personTemp);
                xmlReader = XmlReader.Create(stringReader);
                Image newImage = (Image)XamlReader.Load(xmlReader);

                newImage.Source = new BitmapImage(new(p));

                newImage.Visibility = Visibility.Visible;
                ((WrapPanel)((Grid)newBorder.Child).Children[1]).Children.Add(newImage);
            }

            newBorder.Visibility = Visibility.Visible;
            GroupsPanel.Children.Add(newBorder);
        }
    }

    #endregion

    private void CalenderHex_Click(object sender, RoutedEventArgs e)
    {
        HexItem? hexItem = FindParent<HexItem>((Button)sender);

        string data = hexItem?.Tag.ToString() ?? string.Empty;

        if (DateTime.TryParse(data, out DateTime val))
            WeekdayHeaderTextblock.Text = $"{val.DayOfWeek}\n{val:G}";
    }
}