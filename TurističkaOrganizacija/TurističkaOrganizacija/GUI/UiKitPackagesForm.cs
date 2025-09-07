using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Drawing;

namespace TurističkaOrganizacija.GUI
{
    public class UiKitPackagesForm : UiKit
    {
        public static void PackagesDesign(TextBox txtDestinacija, TextBox txtPrevoz, TextBox txtSmestaj, TextBox txtNaziv)
        {
            TextBoxPlaceholder.SetPlaceholder(txtDestinacija, "Destination");
            TextBoxPlaceholder.SetPlaceholder(txtPrevoz, "Transport type");
            TextBoxPlaceholder.SetPlaceholder(txtSmestaj, "Accommodation type");
            TextBoxPlaceholder.SetPlaceholder(txtNaziv, "Name");
        }
        //==================================================================================================================
        
        // Jednostavan overload – samo kategorije (kolona "vrstaPak" ili auto-detekcija)
        public static void ApplyCategoryColors(DataGridView grid, string categoryColumnName = "vrstaPak")
        {
            if (grid == null) return;
            PaintNow(grid, categoryColumnName, keepSelectionHighlight: false);
            // osvežavanje na promene
            grid.DataBindingComplete += (s, e) => PaintNow(grid, categoryColumnName, false);
            grid.RowsAdded += (s, e) => PaintNow(grid, categoryColumnName, false);
            grid.RowsRemoved += (s, e) => PaintNow(grid, categoryColumnName, false);
            grid.CellValueChanged += (s, e) => PaintNow(grid, categoryColumnName, false);
            grid.ColumnAdded += (s, e) => PaintNow(grid, categoryColumnName, false);
        }

        // Overload koji PRVO pozove podrazumevano UiKit bojanje (sa bojama po izboru),
        // pa zatim nadofarba kategorije fiksnim paletama.
        public static void ApplyCategoryColors(
            DataGridView grid,
            Color baseCellBackColor, Color baseFontColor, Color baseGridLineColor,
            bool keepSelectionHighlight,
            string categoryColumnName = "vrstaPak")
        {
            if (grid == null) return;
            UiKit.StyleGridSolid(grid, baseCellBackColor, baseFontColor, baseGridLineColor, keepSelectionHighlight);
            PaintNow(grid, categoryColumnName, keepSelectionHighlight);

            grid.DataBindingComplete += (s, e) => { UiKit.StyleGridSolid(grid, baseCellBackColor, baseFontColor, baseGridLineColor, keepSelectionHighlight); PaintNow(grid, categoryColumnName, keepSelectionHighlight); };
            grid.RowsAdded += (s, e) => { UiKit.StyleGridSolid(grid, baseCellBackColor, baseFontColor, baseGridLineColor, keepSelectionHighlight); PaintNow(grid, categoryColumnName, keepSelectionHighlight); };
            grid.RowsRemoved += (s, e) => { UiKit.StyleGridSolid(grid, baseCellBackColor, baseFontColor, baseGridLineColor, keepSelectionHighlight); PaintNow(grid, categoryColumnName, keepSelectionHighlight); };
            grid.CellValueChanged += (s, e) => { UiKit.StyleGridSolid(grid, baseCellBackColor, baseFontColor, baseGridLineColor, keepSelectionHighlight); PaintNow(grid, categoryColumnName, keepSelectionHighlight); };
            grid.ColumnAdded += (s, e) => { UiKit.StyleGridSolid(grid, baseCellBackColor, baseFontColor, baseGridLineColor, keepSelectionHighlight); PaintNow(grid, categoryColumnName, keepSelectionHighlight); };
        }

        // --- helper ---
        private static void PaintNow(DataGridView grid, string categoryColumnName, bool keepSelectionHighlight)
        {
            Color SeaColor = Color.FromArgb(0xD9, 0xF1, 0xFF);
            Color MountainColor = Color.FromArgb(0xE8, 0xF7, 0xEA);
            Color ExcursionColor = Color.FromArgb(0xFF, 0xF4, 0xD6);
            Color CruiseColor = Color.FromArgb(0xEE, 0xE6, 0xFF);
            Color TextColor = Color.Black;

            int catIndex = -1;
            if (!string.IsNullOrEmpty(categoryColumnName) && grid.Columns.Contains(categoryColumnName))
                catIndex = grid.Columns[categoryColumnName].Index;
            else if (grid.Columns.Contains("VrstaPak")) catIndex = grid.Columns["VrstaPak"].Index;
            else if (grid.Columns.Contains("Type")) catIndex = grid.Columns["Type"].Index;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                string cat = null;

                if (catIndex >= 0)
                {
                    var v = row.Cells[catIndex].Value;
                    cat = v == null ? null : v.ToString();
                }
                else
                {
                    // fallback: traži ključne reči u redu
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        var s = cell.Value == null ? null : cell.Value.ToString();
                        if (string.IsNullOrWhiteSpace(s)) continue;
                        var t = s.ToLowerInvariant();
                        if (t.Contains("more") || t.Contains("sea")) { cat = "more"; break; }
                        if (t.Contains("planina") || t.Contains("mountain")) { cat = "planina"; break; }
                        if (t.Contains("ekskurzija") || t.Contains("excursion")) { cat = "ekskurzija"; break; }
                        if (t.Contains("krstarenje") || t.Contains("cruise")) { cat = "krstarenje"; break; }
                    }
                }

                Color? tint = null;
                if (!string.IsNullOrEmpty(cat))
                {
                    var t = cat.ToLowerInvariant();
                    if (t.Contains("more") || t.Contains("sea")) tint = SeaColor;
                    else if (t.Contains("planina") || t.Contains("mountain")) tint = MountainColor;
                    else if (t.Contains("ekskurzija") || t.Contains("excursion")) tint = ExcursionColor;
                    else if (t.Contains("krstarenje") || t.Contains("cruise")) tint = CruiseColor;
                }

                if (tint.HasValue)
                {
                    foreach (DataGridViewCell c in row.Cells)
                    {
                        c.Style.BackColor = tint.Value;
                        c.Style.ForeColor = TextColor;
                        c.Style.SelectionForeColor = TextColor;
                        c.Style.SelectionBackColor = keepSelectionHighlight
                            ? ControlPaint.Dark(tint.Value)
                            : tint.Value;
                    }
                }
            }
        }
    }
}
