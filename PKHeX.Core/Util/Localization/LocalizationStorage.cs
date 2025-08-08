using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace PKHeX.Core;

public sealed record LocalizationStorage<T>(string Name, JsonTypeInfo<T> Info) : LanguageStorage<T>
    where T : notnull
{
#pragma warning disable RCS1158 // Static member in generic type should use a type parameter
    public static JsonSerializerOptions Options => new()
    {
        RespectNullableAnnotations = true,
        AllowTrailingCommas = true,
        WriteIndented = true,
        IndentCharacter = ' ',
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
#pragma warning restore RCS1158 // Static member in generic type should use a type parameter

    private string GetJson(string language) => Util.GetStringResource(GetFileName(language));
    public string GetFileName(string language) => $"{Name}_{language}.json";

    /// <param name="language"><see cref="LanguageID"/> index</param>
    /// <inheritdoc cref="Get(LanguageID)"/>
    public T Get(LanguageID language) => Get(language.GetLanguageCode());

    /// <summary>
    /// Gets the localization for the requested language.
    /// </summary>
    /// <param name="language">Language code</param>
    protected override T Create(string language)
    {
        var text = GetJson(language);
        return JsonSerializer.Deserialize(text, Info)
               ?? throw new JsonException($"Failed to deserialize {nameof(T)} for {language}");
    }
}
