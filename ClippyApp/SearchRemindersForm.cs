using System.Drawing.Drawing2D;

namespace ClippyApp;

class SearchRemindersForm : XpFormBase
{
    private readonly TextBox _query;
    private readonly ListView _grid;
    private readonly Label _count;
    private readonly LunaButton _edit;
    private readonly LunaButton _delete;
    private List<ReminderRecord> _all;

    public SearchRemindersForm() : base("Buscar recordatorios", new Size(480, 300))
    {
        _all = RemindersStore.Load();

        var lblQuery = new Label { Text = "Buscar:", AutoSize = true, Location = new Point(14, 18) };
        _query = new TextBox { Location = new Point(62, 14), Width = 300, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "nombre o categoría" };
        _query.TextChanged += (s, e) => Refresh_();
        var search = MakeButton("🔍 Buscar", (s, e) => Refresh_());
        search.Location = new Point(370, 12);

        _grid = BuildGrid();
        _grid.Location = new Point(14, 44);
        _grid.Size = new Size(452, 176);

        _count = new Label { AutoSize = true, ForeColor = LunaColors.SecondaryText, Location = new Point(14, 228) };

        _edit = MakeButton("✏ Editar", (s, e) => EditSelected());
        _delete = MakeButton("🗑 Eliminar", (s, e) => DeleteSelected());
        _edit.Enabled = false;
        _delete.Enabled = false;

        Body.Controls.AddRange(new Control[] { lblQuery, _query, search, _grid, _count });
        PlaceButtonsRight(224, 14, _delete, _edit);

        _grid.SelectedIndexChanged += (s, e) =>
        {
            _edit.Enabled = _grid.SelectedItems.Count > 0;
            _delete.Enabled = _grid.SelectedItems.Count > 0;
        };

        Refresh_();
    }

    private ListView BuildGrid()
    {
        var grid = new ListView
        {
            View = View.Details,
            FullRowSelect = true,
            MultiSelect = false,
            HeaderStyle = ColumnHeaderStyle.Nonclickable,
            OwnerDraw = true,
            BorderStyle = BorderStyle.FixedSingle,
            Font = LunaColors.Ui,
        };
        grid.Columns.Add("Título", 220);
        grid.Columns.Add("Fecha", 100);
        grid.Columns.Add("Categoría", 120);

        grid.DrawColumnHeader += (s, e) =>
        {
            using var brush = new LinearGradientBrush(e.Bounds, LunaColors.GridHeaderTop, LunaColors.GridHeaderBottom, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, e.Bounds);
            e.Graphics.DrawRectangle(Pens.Gray, e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1);
            TextRenderer.DrawText(e.Graphics, e.Header!.Text, LunaColors.UiBold, e.Bounds, Color.Black,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding);
        };
        grid.DrawItem += (s, e) => { };
        grid.DrawSubItem += (s, e) =>
        {
            bool selected = e.Item!.Selected;
            Color back = selected ? LunaColors.GridSelected : (e.ItemIndex % 2 == 1 ? LunaColors.GridAltRow : Color.White);
            Color fore = selected ? Color.White : Color.Black;
            using (var brush = new SolidBrush(back))
                e.Graphics.FillRectangle(brush, e.Bounds);
            TextRenderer.DrawText(e.Graphics, e.SubItem!.Text, LunaColors.Ui, e.Bounds, fore,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding);
        };

        return grid;
    }

    private void Refresh_()
    {
        var q = _query.Text.Trim().ToLowerInvariant();
        var filtered = _all
            .Where(r => q.Length == 0 || r.Title.ToLowerInvariant().Contains(q) || r.Category.ToLowerInvariant().Contains(q))
            .OrderBy(r => r.Date)
            .ToList();

        _grid.BeginUpdate();
        _grid.Items.Clear();
        foreach (var r in filtered)
        {
            var item = new ListViewItem(r.Title) { Tag = r };
            item.SubItems.Add(r.Date.ToString("dd/MM"));
            item.SubItems.Add(r.Category);
            _grid.Items.Add(item);
        }
        _grid.EndUpdate();

        _count.Text = $"{filtered.Count} resultado(s)";
        _edit.Enabled = false;
        _delete.Enabled = false;
    }

    private ReminderRecord? SelectedRecord() =>
        _grid.SelectedItems.Count > 0 ? (ReminderRecord)_grid.SelectedItems[0].Tag! : null;

    private void EditSelected()
    {
        var record = SelectedRecord();
        if (record == null) return;

        using var editForm = new ReminderEditForm(record);
        if (editForm.ShowDialog(this) == DialogResult.OK)
        {
            RemindersStore.Save(_all);
            Refresh_();
        }
    }

    private void DeleteSelected()
    {
        var record = SelectedRecord();
        if (record == null) return;

        using var confirm = new ConfirmDialog("Confirmar eliminación",
            $"¿Está seguro de que desea eliminar el recordatorio \"{record.Title}\"?\n\nEsta acción no se puede deshacer.");
        if (confirm.ShowDialog(this) != DialogResult.Yes) return;

        _all.Remove(record);
        RemindersStore.Save(_all);
        Refresh_();
    }
}
