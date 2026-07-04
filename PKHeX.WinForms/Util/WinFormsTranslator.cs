using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using PKHeX.Core;

namespace PKHeX.WinForms;

public static class WinFormsTranslator
{
    private static readonly Dictionary<string, TranslationContext> Context = [];
    internal static void TranslateInterface(this Control form, string lang) => TranslateForm(form, GetContext(lang));

    internal static string TranslateEnum<T>(T value, string lang) where T : Enum =>
        TranslateEnum(typeof(T).Name, value.ToString(), lang);

    internal static string[] GetEnumTranslation<T>(string lang)
    {
        var type = typeof(T);
        var names = Enum.GetNames(type);
        var result = new string[names.Length];
        for (int i = 0; i < names.Length; i++)
            result[i] = TranslateEnum(type.Name, names[i], lang);
        return result;
    }

    private static string GetTranslationFileNameInternal(ReadOnlySpan<char> lang) => $"lang_{lang}";
    private static string GetTranslationFileNameExternal(ReadOnlySpan<char> lang) => $"lang_{lang}.txt";

    public static string GetKey(ReadOnlySpan<char> formName, ReadOnlySpan<char> name) => $"{formName}.{name}";
    public static IReadOnlyDictionary<string, string> GetDictionary(string lang) => GetContext(lang).Lookup;

    internal static string TranslateText(string key, string fallback, string lang) => GetContext(lang).GetTranslatedText(key, fallback);

    private static TranslationContext GetContext(string lang)
    {
        if (Context.TryGetValue(lang, out var context))
            return context;

        var lines = GetTranslationFile(lang);
        Context.Add(lang, context = new TranslationContext(lines));
        return context;
    }

    private static void TranslateForm(Control form, TranslationContext context)
    {
        form.SuspendLayout();

        // Translate Title
        var formName = form.Name;
        formName = GetSaneFormName(formName);
        form.Text = context.GetTranslatedText(formName, form.Text);

        // Translate Controls
        var translatable = GetTranslatableControls(form);
        foreach (var c in translatable)
            TranslateControl(c, context, formName);

        form.ResumeLayout();
    }

    internal static void TranslateControls(IEnumerable<Control> controls, string baseLanguage)
    {
        var context = GetContext(baseLanguage);
        foreach (var c in controls)
            context.GetTranslatedText(c.Name, c.Text);
    }

    public static void TranslateControls(string formName, IEnumerable<ToolStripMenuItem> controls, string baseLanguage)
    {
        var context = GetContext(baseLanguage);
        foreach (var c in controls)
        {
            if (c.Name is { } name)
                context.GetTranslatedText(GetKey(formName, name), c.Text);
        }
    }

    private static string GetSaneFormName(string formName)
    {
        // Strip out generic form names
        var degen = formName.IndexOf('`');
        if (degen != -1)
            formName = formName[..degen];

        return formName switch
        {
            nameof(SAV_EventFlags2) => nameof(SAV_EventFlags),
            _ => formName,
        };
    }

    private static string TranslateEnum(string type, string value, string lang)
    {
        var context = GetContext(lang);
        var key = $"{type}.{value}";
        return context.GetTranslatedText(key, value);
    }

    private static void TranslateControl(object c, TranslationContext context, ReadOnlySpan<char> formname)
    {
        if (c is Control r)
        {
            var current = r.Text;
            var updated = context.GetTranslatedText(GetKey(formname, r.Name), current);
            if (!ReferenceEquals(current, updated))
                r.Text = updated;
        }
        else if (c is ToolStripItem t)
        {
            var current = t.Text;
            var updated = context.GetTranslatedText(GetKey(formname, t.Name), current);
            if (!ReferenceEquals(current, updated))
                t.Text = updated;
        }
        else if (c is DataGridViewColumn col)
        {
            var current = col.HeaderText;
            var updated = context.GetTranslatedText(GetKey(formname, $"DGV_{col.Name}"), current);
            if (!ReferenceEquals(current, updated))
                col.HeaderText = updated;
        }
    }

    private static ReadOnlySpan<char> GetTranslationFile(ReadOnlySpan<char> lang)
    {
        var file = GetTranslationFileNameInternal(lang);
        // Check to see if the desired translation file exists in the same folder as the executable
        string externalLangPath = GetTranslationFileNameExternal(file);
        if (File.Exists(externalLangPath))
        {
            try { return File.ReadAllText(externalLangPath); }
            catch { /* In use? Just return the internal resource. */ }
        }

        var txt = (string?)Properties.Resources.ResourceManager.GetObject(file);
        return txt ?? string.Empty;
    }

    private static IEnumerable<object> GetTranslatableControls(Control f)
    {
        foreach (var z in f.GetChildrenOfType<Control>())
        {
            switch (z)
            {
                case ToolStrip menu:
                    foreach (var obj in GetToolStripMenuItems(menu))
                        yield return obj;

                    break;
                default:
                    if (string.IsNullOrWhiteSpace(z.Name))
                        break;

                    if (z.ContextMenuStrip is not null) // control has attached MenuStrip
                    {
                        foreach (var obj in GetToolStripMenuItems(z.ContextMenuStrip))
                            yield return obj;
                    }

                    if (Application.IsDarkModeEnabled) // NET10
                        ReformatDark(z);

                    if (z is ListControl or TextBoxBase or LinkLabel or NumericUpDown or ContainerControl)
                        break; // undesirable to modify, ignore

                    if (z is DataGridView { ColumnHeadersVisible: true } dgv)
                    {
                        foreach (DataGridViewColumn col in dgv.Columns)
                        {
                            if (col.Visible && !string.IsNullOrWhiteSpace(col.HeaderText))
                                yield return col;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(z.Text))
                        yield return z;
                    break;
            }
        }
    }

    public static void ReformatDark(Control z)
    {
        if (z is TabControl tc)
        {
            foreach (TabPage tab in tc.TabPages)
                tab.UseVisualStyleBackColor = false;
        }
        else if (z is DataGridView dg)
        {
            dg.EnableHeadersVisualStyles = false;
            dg.BorderStyle = BorderStyle.None;
        }
        else if (z is ComboBox cb)
        {
            cb.FlatStyle = FlatStyle.Popup;
        }
        else if (z is ListBox lb)
        {
            lb.BorderStyle = BorderStyle.None;
        }
        else if (z is RichTextBox rtb)
        {
            rtb.BorderStyle = BorderStyle.None;
        }
        else if (z is TextBoxBase tb)
        {
            tb.BorderStyle = BorderStyle.FixedSingle;
        }
        else if (z is NumericUpDown nud)
        {
            nud.BorderStyle = BorderStyle.FixedSingle;
        }
        else if (z is GroupBox gb)
        {
            gb.FlatStyle = FlatStyle.Popup;
        }
        else if (z is ButtonBase b)
        {
            b.FlatStyle = FlatStyle.Popup;
            if (b is Button { Image: Bitmap bmp })
                b.Image = WinFormsUtil.BlackToWhite(bmp);
        }
    }

    private static IEnumerable<T> GetChildrenOfType<T>(this Control control) where T : class
    {
        foreach (var child in control.Controls.OfType<Control>())
        {
            if (child is T childOfT)
                yield return childOfT;

            if (!child.HasChildren) continue;
            foreach (var descendant in child.GetChildrenOfType<T>())
                yield return descendant;
        }
    }

    private static IEnumerable<object> GetToolStripMenuItems(ToolStrip menu)
    {
        foreach (var i in menu.Items.OfType<ToolStripMenuItem>())
        {
            if (!string.IsNullOrWhiteSpace(i.Text))
                yield return i;
            foreach (var sub in GetToolsStripDropDownItems(i).Where(z => !string.IsNullOrWhiteSpace(z.Text)))
                yield return sub;
        }
    }

    private static IEnumerable<ToolStripMenuItem> GetToolsStripDropDownItems(ToolStripDropDownItem item)
    {
        foreach (var dropDownItem in item.DropDownItems.OfType<ToolStripMenuItem>())
        {
            yield return dropDownItem;
            if (!dropDownItem.HasDropDownItems)
                continue;
            foreach (ToolStripMenuItem subItem in GetToolsStripDropDownItems(dropDownItem))
                yield return subItem;
        }
    }

#if DEBUG
    [RequiresUnreferencedCode("Debug form loading uses reflection to instantiate forms at runtime.")]
    public static void DumpAll(string baseLang, ReadOnlySpan<string> banlist, string dir)
    {
        var context = Context[baseLang];
        context.RemoveBannedEntries(banlist);

        // Reload all contexts
        foreach (var (lang, value) in Context)
        {
            if (lang != baseLang)
                value.CopyFrom(context);
            var exist = GetTranslationFile(lang);
            value.UpdateFrom(exist);

            // Write a new file.
            var fn = GetTranslationFileNameExternal(lang);
            var lines = value.Write();
            File.WriteAllLines(Path.Combine(dir, fn), lines);
        }
    }

    private static bool IsBannedStartsWith(ReadOnlySpan<char> line, ReadOnlySpan<string> banlist)
    {
        foreach (var banned in banlist)
        {
            if (line.StartsWith(banned, StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    [RequiresUnreferencedCode("Debug form loading uses reflection to instantiate forms at runtime.")]
    public static void LoadAllForms(IEnumerable<Type> types, ReadOnlySpan<string> banlist)
    {
        foreach (var t in types)
        {
            if (!typeof(Form).IsAssignableFrom(t) || IsBannedStartsWith(t.Name, banlist))
                continue;

            var constructors = t.GetConstructors();
            try
            {
                if (constructors.Length == 0)
                {
                    Activator.CreateInstance(t, true);
                }
                else
                {
                    foreach (var ctor in constructors)
                    {
                        var parameters = ctor.GetParameters();
                        var args = new object[parameters.Length];
                        ctor.Invoke(args);
                    }
                }
            }
            // This is a debug utility method, will always be logging. Shouldn't ever fail.
            catch (TargetInvocationException)
            {
                // Don't care; forms will sometimes fail to load.
            }
            catch
            {
                System.Diagnostics.Debug.Write($"Failed to create a new form {t}");
            }
        }
    }

    public static void SetUpdateMode(bool status = true)
    {
        foreach (var c in Context)
        {
            if (status)
                c.Value.Clear();
            c.Value.AddNew = status;
        }
    }

    [RequiresUnreferencedCode("Debug settings loading uses reflection to inspect runtime types and attributes.")]
    public static void LoadProperties<T>(string defaultLanguage, Type parent, bool add = true, bool recurse = false)
    {
        var context = (Dictionary<string, string>)Context[defaultLanguage].Lookup;
        Type t = typeof(T);
        LoadAttributes(add, t, context);
        AddProperties(context, t, parent, add, recurse);

    }

    private static void AddProperties(Dictionary<string, string> context, Type t, Type parent, bool add, bool recurse = false)
    {
        var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var key = GetKey(parent.Name, prop.Name);
            if (add)
                context.TryAdd(key, prop.Name);
            else
                context.Remove(key);

            if (recurse)
                AddProperties(context, prop.PropertyType, parent, add, recurse: recurse);
        }
    }

    [RequiresUnreferencedCode("Debug settings loading uses reflection to inspect runtime types and attributes.")]
    public static void LoadPropertyGridFields<T>(string defaultLanguage, bool add = true, bool includeTop = false)
    {
        var context = (Dictionary<string, string>)Context[defaultLanguage].Lookup;
        var t = typeof(T);

        if (includeTop)
        {
            LoadPropertyGridFields(add, t, context);
            return;
        }

        var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
            LoadPropertyGridFields(add, prop.PropertyType, context);
    }

    [RequiresUnreferencedCode("Debug settings loading uses reflection to inspect runtime types and attributes.")]
    public static void LoadPropertyGridFields(string defaultLanguage, Type[] types, bool add = true, bool includeEnums = true)
    {
        var context = (Dictionary<string, string>)Context[defaultLanguage].Lookup;
        foreach (var type in types)
            LoadPropertyGridFields(add, type, context, includeEnums);
    }

    [RequiresUnreferencedCode("Debug settings loading uses reflection to inspect runtime types and attributes.")]
    public static void LoadPropertyGridFields(bool add, Type type, Dictionary<string, string> context, bool includeEnums = true)
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            LoadCategory(context, prop, add);
            LoadProperty(context, prop, add);
            LoadPropertyGridDescription(context, prop, add);

            // If t is an object type, recurse.
            var t = prop.PropertyType;
            if (t.IsArray)
                LoadPropertyGridFields(add, t.GetElementType()!, context, includeEnums);
            else if (t.IsClass && t != typeof(string))
                LoadPropertyGridSubType(add, t, context);
            else if (includeEnums && t.IsEnum)
                LoadEnums(context, t);
        }
    }

    private static void LoadProperty(Dictionary<string, string> context, PropertyInfo prop, bool add)
    {
        {
            var key = GetKey("PropertyGrid", prop.Name);
            if (add)
                context.TryAdd(key, prop.Name);
            else
                context.Remove(key);
        }
    }

    private static void LoadCategory(Dictionary<string, string> context, PropertyInfo prop, bool add)
    {
        var category = (CategoryAttribute[])prop.GetCustomAttributes(typeof(CategoryAttribute), false);
        foreach (var v in category)
        {
            var key = GetKey("PropertyGrid.Category", v.Category);
            if (add)
                context.TryAdd(key, v.Category);
            else
                context.Remove(key);
        }
    }

    private static void LoadPropertyGridDescription(Dictionary<string, string> context, PropertyInfo prop, bool add)
    {
        var description = prop.GetCustomAttribute<DescriptionAttribute>(false);
        if (description is null or LocalizedDescriptionAttribute)
            return;

        var value = description.Description;
        if (string.IsNullOrEmpty(value))
            return;

        var type = prop.DeclaringType?.Name;
        if (string.IsNullOrWhiteSpace(type))
            return;

        var key = GetKey(GetKey("LocalizedDescription", type), prop.Name);
        if (add)
            context.TryAdd(key, value);
        else
            context.Remove(key);
    }

    private static void LoadPropertyGridSubType(bool add, Type type, Dictionary<string, string> context)
    {
        // var name = type.Name;
        // if (name.Contains('`'))
        //     return;
        // var key = GetKey("PropertyGrid.Type", name);
        // 
        // if (add)
        //     context.TryAdd(key, name);
        // else
        //     context.Remove(key);
        // 
        // LoadPropertyGridFields(add, type, context);
    }

    private static void LoadAttributes(bool add, Type type, Dictionary<string, string> context)
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var individual = (LocalizedDescriptionAttribute[])prop.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), false);
            foreach (var v in individual)
            {
                if (add)
                    context.TryAdd(v.Key, v.Fallback);
                else
                    context.Remove(v.Key);
            }

            // If t is an object type, recurse.
            var t = prop.PropertyType;
            if (t.IsClass && t != typeof(string))
                LoadAttributes(add, t, context);
        }
    }

    public static void LoadEnums(string defaultLanguage, params ReadOnlySpan<Type> enumTypesToTranslate)
    {
        var context = (Dictionary<string, string>)Context[defaultLanguage].Lookup;
        LoadEnums(context, enumTypesToTranslate);
    }

    private static void LoadEnums(Dictionary<string, string> context, params ReadOnlySpan<Type> enumTypesToTranslate)
    {
        foreach (var t in enumTypesToTranslate)
        {
            var names = Enum.GetNames(t);
            foreach (var name in names)
            {
                var key = GetKey(t.Name, name);
                context.TryAdd(key, name);
            }
        }
    }
#endif
}

public sealed class TranslationContext
{
    public const char Separator = '=';
    private readonly Dictionary<string, string> Translation = [];
    public IReadOnlyDictionary<string, string> Lookup => Translation;
    public bool AddNew { get; set; }

    public void Clear() => Translation.Clear();

    public TranslationContext(ReadOnlySpan<char> content, char separator = Separator)
    {
        var iterator = content.EnumerateLines();
        foreach (var line in iterator)
            LoadLine(line, separator);
    }

    private void LoadLine(ReadOnlySpan<char> line, char separator = Separator)
    {
        var split = line.IndexOf(separator);
        if (split < 0)
            return; // ignore
        var key = line[..split].ToString();
        var value = line[(split + 1)..].ToString();
        Translation.TryAdd(key, value);
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    public string? GetTranslatedText(string val, string? fallback)
    {
        if (Translation.TryGetValue(val, out var translated))
            return translated;

        if (fallback is not null && AddNew)
            Translation.Add(val, fallback);
        return fallback;
    }

    public IEnumerable<string> Write(char separator = Separator)
    {
        return Translation.Select(z => $"{z.Key}{separator}{z.Value}").OrderBy(z => z.Contains('.')).ThenBy(z => z);
    }

    public void UpdateFrom(ReadOnlySpan<char> text)
    {
        var lines = text.EnumerateLines();
        foreach (var line in lines)
        {
            var split = line.IndexOf(Separator);
            if (split < 0)
                continue;
            var key = line[..split].ToString();

            ref var exist = ref CollectionsMarshal.GetValueRefOrNullRef(Translation, key);
            if (!Unsafe.IsNullRef(ref exist))
                exist = line[(split + 1)..].ToString();
        }
    }

    public void RemoveBannedEntries(ReadOnlySpan<string> banlist)
    {
        var badKeys = new List<string>();
        foreach (var (key, _) in Translation)
        {
            if (IsBannedContains(key, banlist))
                badKeys.Add(key);

            static bool IsBannedContains(ReadOnlySpan<char> key, ReadOnlySpan<string> banlist)
            {
                foreach (var line in banlist)
                {
                    if (line.EndsWith(Separator))
                    {
                        if (key.EndsWith(line.AsSpan()[..^1], StringComparison.Ordinal))
                            return true;
                    }
                    else
                    {
                        if (key.Contains(line, StringComparison.Ordinal))
                            return true;
                    }
                }
                return false;
            }
        }

        foreach (var key in badKeys)
            Translation.Remove(key);
    }

    public void CopyFrom(TranslationContext other)
    {
        foreach (var (key, value) in other.Translation)
            Translation.Add(key, value);
    }
}
