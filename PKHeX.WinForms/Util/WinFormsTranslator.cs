using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;

namespace PKHeX.WinForms
{
    public static class WinFormsTranslator
    {
        private static readonly Dictionary<string, TranslationContext> Context = new Dictionary<string, TranslationContext>();
        internal static void TranslateInterface(this Control form, string lang) => TranslateForm(form, GetContext(lang));

        private static string GetTranslationFileNameInternal(string lang) => $"lang_{lang}";
        private static string GetTranslationFileNameExternal(string lang) => $"lang_{lang}.txt";
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
            var formname = form.Name;
            // Translate Title
            form.Text = context.GetTranslatedText(formname, form.Text);
            var translatable = GetTranslatableControls(form);
            foreach (var c in translatable)
            {
                if (c is Control r)
                    r.Text = context.GetTranslatedText($"{formname}.{r.Name}", r.Text);
                else if (c is ToolStripItem t)
                    t.Text = context.GetTranslatedText($"{formname}.{t.Name}", t.Text);
            }
            form.ResumeLayout();
        }

        private static IEnumerable<string> GetTranslationFile(string lang)
        {
            var file = GetTranslationFileNameInternal(lang);
            // Check to see if a the translation file exists in the same folder as the executable
            string externalLangPath = GetTranslationFileNameExternal(file);
            if (File.Exists(externalLangPath))
            {
                try { return File.ReadAllLines(externalLangPath); }
                catch { /* In use? Just return the internal resource. */ }
            }

            return Util.GetStringList(file);
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

                        if (z.ContextMenuStrip != null) // control has attached menustrip
                            foreach (var obj in GetToolStripMenuItems(z.ContextMenuStrip))
                                yield return obj;

                        if (z is ComboBox || z is TextBox || z is MaskedTextBox || z is LinkLabel || z is NumericUpDown)
                            break; // undesirable to modify, ignore

                        if (!string.IsNullOrWhiteSpace(z.Text))
                            yield return z;
                        break;
                }
            }
        }
        private static IEnumerable<T> GetChildrenOfType<T>(this Control control) where T : class
        {
            foreach (Control child in control.Controls)
            {
                var childOfT = child as T;
                if (childOfT != null)
                    yield return childOfT;

                if (!child.HasChildren) continue;
                foreach (var descendant in GetChildrenOfType<T>(child))
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
                if (!dropDownItem.HasDropDownItems) continue;
                foreach (ToolStripMenuItem subItem in GetToolsStripDropDownItems(dropDownItem))
                    yield return subItem;
            }
        }

        public static void UpdateAll(string baseLanguage, IEnumerable<string> others)
        {
            var basecontext = GetContext(baseLanguage);
            foreach (var lang in others)
            {
                var c = GetContext(lang);
                c.UpdateFrom(basecontext);
            }
        }

        public static void DumpAll()
        {
            var results = Context.Select(z => new {Lang = z.Key, Lines = z.Value.Write()});
            foreach (var c in results)
                File.WriteAllLines(GetTranslationFileNameExternal(c.Lang), c.Lines);
        }

        public static void LoadAllForms()
        {
            var q = from t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                where t.BaseType == typeof(Form)
                select t;
            foreach (var t in q)
            {
                var constructors = t.GetConstructors();
                if (constructors.Length == 0)
                { System.Console.WriteLine($"skipped {t.Name}"); continue; }
                var argCount = constructors.First().GetParameters().Length;
                if (argCount <= 0)
                { System.Console.WriteLine($"skipped {t.Name}"); continue; }
                try
                {
                    var _ = (Form)System.Activator.CreateInstance(t, new object[argCount]);
                }
                catch { }
            }
        }
    }

    public class TranslationContext
    {
        public bool AddNew { get; set; } = true;
        public bool RemoveUsedKeys { get; set; } = false;
        private readonly Dictionary<string, string> Translation = new Dictionary<string, string>();
        public TranslationContext(IEnumerable<string> content, char separator = '=')
        {
            var entries = content.Select(z => z.Split(separator)).Where(z => z.Length == 2);
            foreach (var r in entries.Where(z => !Translation.ContainsKey(z[0])))
                Translation.Add(r[0], r[1]);
        }

        public string GetTranslatedText(string val, string fallback)
        {
            if (RemoveUsedKeys)
                Translation.Remove(val);

            if (Translation.TryGetValue(val, out var translated))
                return translated;

            if (fallback != null && AddNew)
                Translation.Add(val, fallback);
            return fallback;
        }

        public IEnumerable<string> Write(char separator = '=')
        {
            return Translation.Select(z => $"{z.Key}{separator}{z.Value}").OrderBy(z => z.Contains(".")).ThenBy(z => z);
        }

        public void UpdateFrom(TranslationContext other)
        {
            bool oldAdd = AddNew;
            AddNew = true;
            foreach (var kvp in other.Translation)
                GetTranslatedText(kvp.Key, kvp.Value);
            AddNew = oldAdd;
        }

        public void RemoveKeys(TranslationContext other)
        {
            foreach (var kvp in other.Translation)
                Translation.Remove(kvp.Key);
        }
    }
}
