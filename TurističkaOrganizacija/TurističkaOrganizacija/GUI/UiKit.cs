using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Infrastructure;
//using TurističkaOrganizacija.Infrastructure.Fakes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace TurističkaOrganizacija.GUI
{
    //this class is used to prevent cluttering Form1 class with to much code to keep the code readable
    public class UiKit
    {
        //=========================================================================================================
        //resize the grid automatically
        public static void WireAutoSizeGrid(DataGridView grid, Form host, double maxW = 0.9, double maxH = 0.6)
        {
            if (grid == null || host == null) return;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            grid.AutoSize = false;               // we control the Size

            Action apply = () =>
            {
                grid.AutoResizeColumns();
                grid.AutoResizeRows();

                int idealW = (grid.RowHeadersVisible ? grid.RowHeadersWidth : 0)
                           + grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).Sum(c => c.Width)
                           + 2; // borders

                int idealH = (grid.ColumnHeadersVisible ? grid.ColumnHeadersHeight : 0)
                           + grid.Rows.Cast<DataGridViewRow>().Where(r => r.Visible).Sum(r => r.Height)
                           + 2;

                int maxWpx = (int)(host.ClientSize.Width * maxW);
                int maxHpx = (int)(host.ClientSize.Height * maxH);

                int finalW = Math.Min(idealW, maxWpx);
                int finalH = Math.Min(idealH, maxHpx);

                bool vScroll = idealH > maxHpx;
                bool hScroll = idealW > maxWpx;
                if (vScroll) finalW = Math.Min(finalW + SystemInformation.VerticalScrollBarWidth, maxWpx);
                if (hScroll) finalH = Math.Min(finalH + SystemInformation.HorizontalScrollBarHeight, maxHpx);

                grid.Size = new Size(finalW, finalH);
            };

            // re-size on data/columns/form changes
            grid.DataBindingComplete += (s, e) => apply();
            grid.RowsAdded += (s, e) => apply();
            grid.RowsRemoved += (s, e) => apply();
            grid.ColumnWidthChanged += (s, e) => apply();
            host.Resize += (s, e) => apply();

            apply();
        }
        //=========================================================================================================
        //Form bc color
        public static void StyleForm(Form f, Color color, double opacity01)
        {
            f.BackColor = color;                // e.g. ColorTranslator.FromHtml("#1f2937")
            f.Opacity = Math.Max(0.0, Math.Min(1.0, opacity01));  // 0..1
        }
        //=========================================================================================================
        //grid cells bc color
        public static void StyleGridSolid(
        DataGridView grid,
        Color cellBackColor,
        Color fontColor,
        Color gridLineColor,
        bool keepSelectionHighlight = false)
        {
            if (grid == null) return;

            // smoother repaint
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, grid, new object[] { true });

            grid.EnableHeadersVisualStyles = false;

            // borders / lines
            grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.GridColor = gridLineColor;
            grid.BackgroundColor = cellBackColor;

            // headers (column + row) and cells
            grid.ColumnHeadersDefaultCellStyle.BackColor = cellBackColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = fontColor;

            grid.RowHeadersDefaultCellStyle.BackColor = cellBackColor;
            grid.RowHeadersDefaultCellStyle.ForeColor = fontColor;

            grid.DefaultCellStyle.BackColor = cellBackColor;
            grid.DefaultCellStyle.ForeColor = fontColor;
            grid.AlternatingRowsDefaultCellStyle.BackColor = cellBackColor;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = fontColor;

            // selection styling
            if (keepSelectionHighlight)
            {
                // keep classic highlight but readable
                grid.DefaultCellStyle.SelectionBackColor = ControlPaint.Dark(cellBackColor);
                grid.DefaultCellStyle.SelectionForeColor = fontColor;
                grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = ControlPaint.Dark(cellBackColor);
                grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = fontColor;
                grid.RowHeadersDefaultCellStyle.SelectionBackColor = ControlPaint.Dark(cellBackColor);
                grid.RowHeadersDefaultCellStyle.SelectionForeColor = fontColor;
            }
            else
            {
                // selection looks identical to normal (uniform color everywhere)
                grid.DefaultCellStyle.SelectionBackColor = cellBackColor;
                grid.DefaultCellStyle.SelectionForeColor = fontColor;
                grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = cellBackColor;
                grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = fontColor;
                grid.RowHeadersDefaultCellStyle.SelectionBackColor = cellBackColor;
                grid.RowHeadersDefaultCellStyle.SelectionForeColor = fontColor;
            }

            // apply to existing columns too (if already added)
            foreach (DataGridViewColumn col in grid.Columns)
            {
                col.DefaultCellStyle.BackColor = cellBackColor;
                col.DefaultCellStyle.ForeColor = fontColor;
            }

            grid.Invalidate(); // repaint
        }

        //=========================================================================================================
        //styles of buttons
        private static readonly Dictionary<Button, Color> _buttonOverlay = new Dictionary<Button, Color>();

        public static void StyleButtons(Color baseColor, int opacity0to255, params Button[] buttons)
        {
            int a = Math.Max(0, Math.Min(255, opacity0to255));
            var overlay = Color.FromArgb(a, baseColor);

            foreach (var btn in buttons)
            {
                if (btn == null) continue;

                // owner-draw look
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 1;
                btn.UseVisualStyleBackColor = false;   // koristimo sopstveno crtanje
                btn.BackColor = btn.Parent != null ? btn.Parent.BackColor : SystemColors.Control;

                _buttonOverlay[btn] = overlay;

                // ne dupliraj event
                btn.Paint -= Button_TintPaint;
                btn.Paint += Button_TintPaint;

                // re-crtaj pri promeni veličine/teksta
                btn.SizeChanged -= Button_Invalidates;
                btn.TextChanged -= Button_Invalidates;
                btn.SizeChanged += Button_Invalidates;
                btn.TextChanged += Button_Invalidates;

                btn.Invalidate();
            }
        }

        private static void Button_Invalidates(object sender, EventArgs e)
        {
            var b = sender as Button;
            if (b != null) b.Invalidate();
        }

        private static void Button_TintPaint(object sender, PaintEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            Color overlay;
            if (!_buttonOverlay.TryGetValue(btn, out overlay)) return;

            // pozadina (kao roditelj)
            var bg = btn.Parent != null ? btn.Parent.BackColor : SystemColors.Control;
            using (var bgBr = new SolidBrush(bg))
                e.Graphics.FillRectangle(bgBr, btn.ClientRectangle);

            // poluprovidna “tinta”
            using (var br = new SolidBrush(overlay))
                e.Graphics.FillRectangle(br, btn.ClientRectangle);

            // ivica
            using (var pen = new Pen(ControlPaint.Dark(overlay, 0.2f)))
                e.Graphics.DrawRectangle(pen, Rectangle.Inflate(btn.ClientRectangle, -1, -1));

            // tekst (centar)
            TextRenderer.DrawText(
                e.Graphics,
                btn.Text ?? string.Empty,
                btn.Font,
                btn.ClientRectangle,
                btn.ForeColor, // ostavi kako je podešeno spolja; po želji možeš postaviti i ovde
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}