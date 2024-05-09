using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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


    Random random = new Random();


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
    #endregion

    #region Window Functions
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Load_Calendar();
        Load_TodoList();
        Load_Groups();
    }
    #endregion

    #region Extra Funxtions
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
    #endregion

    #region Loading Functions

    private async void Load_Calendar()
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
                string CalendarDayTemp = XamlWriter.Save(CalenderHexGrid.Children[0]);
                StringReader stringReader = new(CalendarDayTemp);
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

                    // Check if something is planned on this day
                    if (random.Next(100) < 33) // Just for testing
                    {
                        Border borderTemplate = (Border)((StackPanel)((Grid)newHexItem.Content).Children[1]).Children[0];
                        /* if more than one is needed this can be used to clone the bar
                        string gameButtonTemp = XamlWriter.Save(borderTemplate);
                        stringReader = new(gameButtonTemp);
                        xmlReader = XmlReader.Create(stringReader);
                        Border newBorder = (Border)XamlReader.Load(xmlReader);*/

                        borderTemplate.Visibility = Visibility.Visible;

                        borderTemplate.Background = random.Next(2) == 0 ? Brushes.LightBlue : Brushes.LightCoral;

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
        TodoListTextBlock.FontFamily = new FontFamily("Tahoma");
    }

    private void Load_Groups ()
    {
        // Load Data
        string[] someData = { "Sport", "Sushi" };

        for (int i = 0; i < someData.Length; i++)
        {
            string GroupTemp = XamlWriter.Save(GroupsPanel.Children[0]);
            StringReader stringReader = new(GroupTemp);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Border newBorder = (Border)XamlReader.Load(xmlReader);

            ((CheckBox)((Grid)newBorder.Child).Children[0]).Content = someData[i];
            ((CheckBox)((Grid)newBorder.Child).Children[0]).FontFamily = new FontFamily("Tahoma");

            ((CheckBox)((Grid)newBorder.Child).Children[0]).IsChecked = random.Next(2) == 0;

            // Load People
            string[] people = { "https://api.suhri.de/user/picture/3/128.png", "https://api.suhri.de/user/picture/1/128.png" }; // Just for testing

            for (int j = 0; j < people.Length; j++)
            {
                string PersonTemp = XamlWriter.Save(((WrapPanel)((Grid)newBorder.Child).Children[1]).Children[0]);
                stringReader = new(PersonTemp);
                xmlReader = XmlReader.Create(stringReader);
                Image newImage = (Image)XamlReader.Load(xmlReader);

                newImage.Source = new BitmapImage(new Uri(people[j]));

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
        HexItem hexItem = FindParent<HexItem>((Button)sender);

        string data = hexItem.Tag.ToString() ?? "";

        if (DateTime.TryParse(data, out DateTime val))
            WeekdayHeaderTextblock.Text = $"{val.DayOfWeek}\n{val:G}";
    }


}