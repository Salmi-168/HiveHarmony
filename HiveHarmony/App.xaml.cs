using System.Configuration;
using System.Data;
using System.Windows;

namespace HiveHarmony;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public void ChangeDesign(string _theme)
    {
        Resources.MergedDictionaries[0].MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(_theme, UriKind.Relative) });
        Resources.MergedDictionaries[0].MergedDictionaries.RemoveAt(0);
    }
}