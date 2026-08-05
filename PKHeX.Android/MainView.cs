using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Android.Util;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Android;

/// <summary>
/// Small Android host proof: the shell string comes from Presentation resources and the species
/// name comes from PKHeX.Core's localized game data. Full save/SAF editing belongs to the next phase.
/// </summary>
public sealed class MainView : UserControl
{
    public MainView()
    {
        Log.Info("PKHEX_ANDROID", "MainView: start");
        var language = (int)Language.GetLanguageValue(GameInfo.CurrentLanguage);
        Log.Info("PKHEX_ANDROID", $"MainView: language={language}");
        var pikachu = SpeciesName.GetSpeciesName((ushort)Species.Pikachu, language);
        Log.Info("PKHEX_ANDROID", $"MainView: species={pikachu}");

        Content = new Border
        {
            Padding = new Thickness(28),
            Child = new StackPanel
            {
                Spacing = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new TextBlock
                    {
                        Text = LocalizedStrings.Instance["Hero_Title"],
                        FontSize = 30,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                    },
                    new TextBlock
                    {
                        Text = $"025 · {pikachu}",
                        FontSize = 22,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                    },
                },
            },
        };
        Log.Info("PKHEX_ANDROID", "MainView: content assigned");
    }
}
