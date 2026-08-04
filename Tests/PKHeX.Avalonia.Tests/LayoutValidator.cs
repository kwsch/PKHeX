using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public static class LayoutValidator
{
    public static void Validate(Visual root)
    {
        var errors = new List<string>();
        ValidateRecursive(root, root.Bounds, errors);

        if (errors.Any())
        {
            throw new Xunit.Sdk.XunitException(
                $"Layout Validation Failed with {errors.Count} errors:\n" + 
                string.Join("\n\n", errors));
        }
    }

    private static void ValidateRecursive(Visual visual, Rect rootBounds, List<string> errors)
    {
        if (!visual.IsVisible || visual.Bounds.Width <= 0 || visual.Bounds.Height <= 0)
            return;

        var bounds = visual.Bounds;

        // 1. Check for Vertical Overflow (Child extending beyond Parent)
        // We only check if the parent imposes a clip or fixed size constraint that matters.
        // For now, checking if it goes way out of the window/root bounds is a good sanity check.
        // Transform bounds to root coordinates for global check
        // Note: Avalonia.Headless might not set absolute positions perfectly relative to screen, 
        // but 'Bounds' is relative to parent. 

        // 2. Text Truncation Check
        if (visual is TextBlock tb)
        {
            CheckTextTruncation(tb, errors);
        }

        // 3. Overlap Check (Siblings)
        // We check this at the container level (e.g. Panel)
        if (visual is Panel panel) 
        {
            CheckSiblingOverlap(panel, errors);
        }

        // Recursively check children
        foreach (var child in visual.GetVisualChildren())
        {
            if (child is Visual vChild)
            {
                // Skip internal Avalonia visuals that we don't control
                var typeName = vChild.GetType().Name;
                if (typeName.Contains("Viewbox") || typeName.Contains("ScrollContentPresenter")) 
                {
                     // Still recurse into them, but maybe don't validate them directly?
                     // Actually, we want to recurse INTO them to find OUR controls.
                }

                ValidateRecursive(vChild, rootBounds, errors);
            }
        }
    }

    private static void CheckTextTruncation(TextBlock tb, List<string> errors)
    {
        if (string.IsNullOrEmpty(tb.Text)) return;

        // Calculate needed width
        // TextLayout.Width is the width of the text content.
        // Bounds.Width is the allocated width.
        // We add a small tolerance for floating point layout issues.
        
        // If TextTrimming is None, and content exceeds bounds -> Overflow/Clipping (UGLY)
        // If TextTrimming is Character/WordEllipsis, and content exceeds bounds -> It shows '...' (Maybe OK, but check if unintended)
        
        // If it's wrapped, Bounds.Height should accommodate all lines.
        
        var textLayout = tb.TextLayout;
        if (textLayout == null) return;

        // Check Horizontal Overflow
        // If not wrapping, and TextLayout width > Bounds width -> bad.
        if (!tb.TextWrapping.HasFlag(TextWrapping.Wrap))
        {
            if (textLayout.Width > tb.Bounds.Width + 1.0) // 1px tolerance
            {
                // Verify if Trimming is handling it
                 if (tb.TextTrimming == TextTrimming.None)
                 {
                     errors.Add($"[Text Truncation] '{GetPreviewText(tb)}' (Width: {textLayout.Width:F1}) exceeds Bounds ({tb.Bounds.Width:F1}). Parent: {tb.Parent?.GetType().Name}");
                 }
            }
        }
        else
        {
            // If wrapping, check Vertical Overflow
            // If the text height exceeds the bounds height, it's being clipped vertically
            if (textLayout.Height > tb.Bounds.Height + 1.0)
            {
                 errors.Add($"[Vertical Text/Clip] '{GetPreviewText(tb)}' (Height: {textLayout.Height:F1}) exceeds Bounds ({tb.Bounds.Height:F1}).");
            }
        }
    }

    private static void CheckSiblingOverlap(Panel panel, List<string> errors)
    {
        // Only check exact types of panels where overlap is usually bad
        // Grid: Overlap is common if intended (same cell), bad if unintended. Hard to distinguish.
        // Canvas: Overlap is intended.
        // StackPanel: Should not overlap.
        // DockPanel: Should not overlap.
        
        // We focus on finding controls that overlap significantly when they shouldn't.
        // Let's restrict to static text/input controls interfering with each other.
        
        var children = panel.Children.Where(c => c.IsVisible && c.Bounds.Width > 0 && c.Bounds.Height > 0).ToList();
        
        for (int i = 0; i < children.Count; i++)
        {
            for (int j = i + 1; j < children.Count; j++)
            {
                var c1 = children[i];
                var c2 = children[j];

                // Skip if one is purely decorative (IsHitTestVisible = false)
                if (!c1.IsHitTestVisible || !c2.IsHitTestVisible)
                    continue;
                
                // Skip if one is a background/border that is meant to be behind
                if (c1 is Border || c2 is Border) continue;
                
                // Skip if they are in a Grid and share the same cell? 
                // Grid overlap logic is complex. 
                
                // Let's look for "Visual Collision" between Data Input controls.
                bool isInput1 = c1 is TextBox || c1 is ComboBox || c1 is NumericUpDown || c1 is CheckBox;
                bool isInput2 = c2 is TextBox || c2 is ComboBox || c2 is NumericUpDown || c2 is CheckBox;
                bool isLabel1 = c1 is TextBlock || c1 is Label;
                bool isLabel2 = c2 is TextBlock || c2 is Label;

                if ((isInput1 || isLabel1) && (isInput2 || isLabel2))
                {
                    var r1 = c1.Bounds;
                    var r2 = c2.Bounds;
                    
                    var intersect = r1.Intersect(r2);
                    if (intersect.Width > 0 && intersect.Height > 0)
                    {
                        // Ignore minor touch (like borders touching)
                        // If area is significant
                        if (intersect.Width > 1 && intersect.Height > 1) 
                        {
                            // Heuristic: If it's a Grid, check if they are in different rows/cols.
                            // But Bounds are already calculated layout positions relative to parent.
                            // If they intersect in layout space, they are visually overlapping.
                            
                            errors.Add($"[Overlap] {c1.GetType().Name} ('{GetPreviewText(c1)}') overlaps {c2.GetType().Name} ('{GetPreviewText(c2)}') in {panel.GetType().Name}. Overlap Rect: {intersect}");
                        }
                    }
                }
            }
        }
    }

    private static string GetPreviewText(Control c)
    {
        if (c is TextBlock tb) return tb.Text ?? "";
        if (c is TextBox tbox) return tbox.Text ?? "";
        if (c is ContentControl cc && cc.Content is string s) return s;
        if (c is ContentControl cc2 && cc2.Content is TextBlock tb2) return tb2.Text ?? "";
        return c.Name ?? c.GetType().Name;
    }
}
