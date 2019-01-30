# User Controls und Menüs
Dieses Beispiel demonstriert, wie ein UserControl und ein Menü eingesetzt werden kann. Das UserControl ist in eine eigene Datei 
ausgelagerter XAML Code. Es kann ein eigenes ViewModel besitzen und ist wie ein getrenntes Fenster zu betrachten. 
Beim Verwenden von Menüpunkten wird häufig zwischen verschiedenen User Controls umgeschulten. Durch den Einsatz eines 
Converters kann dies deklerativ in XAML erfolgen.

## Funktionsprinzip
In XAML wird mit *MenuItem* ein Menü definiert. Beim Klicken wird der Command ausgeführt, welcher in *MainViewModel*
den aktiven Menüpunkt setzt. Mit *CommandParameter* wird als zusätzlicher Parameter der (eindeutige) Namen
des Menüpunktes mitgegeben, den wir angeklickt haben:

```xml
<MenuItem Header="_File">
    <MenuItem Header="Add Person" Command="{Binding ActivateMenuitem}" CommandParameter="Add" />
    <MenuItem Header="Edit Person" Command="{Binding ActivateMenuitem}" CommandParameter="Edit" />
    <Separator />
    <MenuItem x:Name="Exit" Header="Exit"  Click="Exit_Click"/>
</MenuItem>
```

Der Command *ActivateMenuitem* setzt einfach den in *CommandParameter* übergebenen String im ViewModel:
``´c#
ActivateMenuitem = new RelayCommand((param) => ActiveMenuitem = param?.ToString());
```
            
Weiter unten im Content Bereich von *MainWindow* werden die einzelnen Controls, die hinter den Menüpunkten
stehen, geladen. Da sie natürlich nicht alle gleichzeitig sichtbar sein sollen, wird die Visibility mit
einem Converter festgelegt. Der Converter bekommt einerseits als Value den in *MainViewModel* gesetzten aktiven
Menüpunkt (*ActiveMenuitem:string*) und als *ConverterParameter* den Wert, bei dem es sichtbar sein soll:

```xml
<ContentControl Visibility="{Binding ActiveMenuitem, ConverterParameter=Add, Converter={StaticResource MenuVisibilityConverter}}">
    <ContentControl.Content>
        <local:AddPersonControl/>
    </ContentControl.Content>
</ContentControl>
<ContentControl Visibility="{Binding ActiveMenuitem, ConverterParameter=Edit, Converter={StaticResource MenuVisibilityConverter}}">
    <ContentControl.Content>
        <local:EditPersonControl/>
    </ContentControl.Content>
</ContentControl>  
```

Der Converter selbst prüft dann nur, ob die Werte übereinstimmen. Wenn ja, wird Visible geliefert:

```c#
/// <summary>
/// Converter für die Sichtbarkeit der Controls.
/// </summary>
public class MenuVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Wandelt den value in eine Visibility Eigenschaft um.
    /// </summary>
    /// <param name="value">Über das Binding aus XAML übergebener Wert (das aktuelle Menüiten).</param>
    /// <param name="targetType"></param>
    /// <param name="parameter">Über ConverterParameter aus XAML übergebener wert, wann Visible geliefert werden soll.</param>
    /// <param name="culture"></param>
    /// <returns>true, wenn value = parameter. Sonst false.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string activeMenuitem = value?.ToString() ?? "";
        string targetMenuitem = parameter?.ToString() ?? "";
        return activeMenuitem == targetMenuitem ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```
