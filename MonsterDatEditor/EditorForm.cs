using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Data;


namespace MonsterDatEditor
{
    public partial class EditorForm : Form
    {

        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dt = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null) ?? DBNull.Value;
                dt.Rows.Add(values);
            }
            return dt;
        }
        public EditorForm()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            InitializeComponent();

            dataGridView1.AllowUserToResizeColumns = false; // Lock column width
            dataGridView1.AllowUserToResizeRows = false;    // Lock row height
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing; // Lock row header width
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing; // Lock column header height

            btnSave.Enabled = false;
            btnExport.Enabled = false;
            btnImport.Enabled = false;
            txtSearch.Enabled = false;
            btnSearch.Enabled = false;
            cmbSearch.Enabled = false;



            cmbSearch.DropDownStyle = ComboBoxStyle.DropDownList;


            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;

            var menu = new ContextMenuStrip();
            var addItem = new ToolStripMenuItem("Add Row");
            addItem.ShortcutKeys = Keys.Control | Keys.N;
            addItem.ShortcutKeyDisplayString = "Ctrl+N";
            addItem.Click += (s, ea) => AddRow();
            menu.Items.Add(addItem);

            var copyItem = new ToolStripMenuItem("Copy");
            copyItem.ShortcutKeys = Keys.Control | Keys.C;
            copyItem.ShortcutKeyDisplayString = "Ctrl+C";
            copyItem.Click += (s, ea) => CopyRow();
            menu.Items.Add(copyItem);

            var pasteItem = new ToolStripMenuItem("Paste");
            pasteItem.ShortcutKeys = Keys.Control | Keys.V;
            pasteItem.ShortcutKeyDisplayString = "Ctrl+V";
            pasteItem.Click += (s, ea) => PasteRow();
            menu.Items.Add(pasteItem);

            var exportItem = new ToolStripMenuItem("Export Selected");
            exportItem.ShortcutKeys = Keys.Control | Keys.E;
            exportItem.ShortcutKeyDisplayString = "Ctrl+E";
            exportItem.Click += (s, ea) => ExportSelected();
            menu.Items.Add(exportItem);

            var delItem = new ToolStripMenuItem("Delete Selected");
            delItem.ShortcutKeys = Keys.Delete;
            delItem.ShortcutKeyDisplayString = "Del";
            delItem.Click += (s, ea) => DeleteSelected();
            menu.Items.Add(delItem);


            dataGridView1.ContextMenuStrip = menu;
            dataGridView1.RowLeave += dataGridView1_RowLeave;
            dataGridView1.DataError += dataGridView1_DataError;

            // Keydown shortcut
            dataGridView1.KeyDown += DataGridView1_KeyDown;
            dataGridView1.MultiSelect = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        }

        public class MonsterType
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int Size { get; set; }
            public int Icon1 { get; set; }
            public int Type { get; set; }
            public int HP { get; set; }
            public int Level { get; set; }
            public string Aura1 { get; set; }
            public string Aura2 { get; set; }
        }






        private void btnLoad_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Monster.dat|Monster.dat|All Files|*.*",
                Title = "Pilih Monster.dat"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var monsters = new List<MonsterType>();

            using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                int count = br.ReadInt32(); // 4 bytes, jumlah monster
                int monsterEntrySize = 312;
                int monsterDataOffset = (int)fs.Length - count * monsterEntrySize;

                for (int i = 0; i < count; i++)
                {
                    long entryOffset = monsterDataOffset + i * monsterEntrySize;
                    fs.Position = entryOffset;
                    byte[] b = br.ReadBytes(monsterEntrySize);

                    if (b.Length < monsterEntrySize)
                    {
                        MessageBox.Show($"Row {i + 1} size kurang: {b.Length} bytes (Expected {monsterEntrySize})");
                        break;
                    }

                    int id = BitConverter.ToInt32(b, 0);
                    string name = Encoding.GetEncoding("GB18030").GetString(b, 4, 16).TrimEnd('\0');
                    int icon1 = BitConverter.ToInt32(b, 20);
                    int type = BitConverter.ToInt32(b, 24);
                    int hp = BitConverter.ToInt32(b, 28);
                    int level = BitConverter.ToInt32(b, 32);
                    int size = BitConverter.ToInt32(b, 48);
                    string aura1 = Encoding.GetEncoding("GB18030").GetString(b, 56, 128).TrimEnd('\0');
                    string aura2 = Encoding.GetEncoding("GB18030").GetString(b, 184, 128).TrimEnd('\0');

                    monsters.Add(new MonsterType
                    {
                        ID = id,
                        Name = name,
                        Icon1 = icon1,
                        Type = type,
                        HP = hp,
                        Level = level,
                        Size = size,
                        Aura1 = aura1,
                        Aura2 = aura2
                    });
                }
            }

            // Convert ke DataTable (untuk sorting)
            DataTable dt = ToDataTable(monsters);
            dataGridView1.DataSource = dt;

            // Enable sorting semua column
            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.SortMode = DataGridViewColumnSortMode.Automatic;

            HookRowCountUpdater(); // Auto update rowcount (kalau guna function sama)

            // Set title form dengan total monster
            this.Text = $"Monster.dat Editor by DuaSelipar - Total: {dt.Rows.Count} monsters";

            // Enable semua function sama itemtype
            PopulateSearchFields();
            lblStatus.Text = $"{dataGridView1.Rows.Count} monsters loaded. Ready.";
            btnSave.Enabled = true;
            btnExport.Enabled = true;
            btnImport.Enabled = true;
            txtSearch.Enabled = true;
            btnSearch.Enabled = true;
            cmbSearch.Enabled = true;
        }









        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Guna nama column yang betul
            if (dataGridView1.Columns[e.ColumnIndex].Name == "req_level")
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = 0;
                    e.FormattingApplied = true;
                }
            }
        }








        // Event handler untuk btnSearch
        // Field global untuk track hasil search terakhir
        private int lastSearchRow = -1;

        // Search pertama (btnSearch)
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                dataGridView1.ClearSelection();
                lastSearchRow = -1;
                return;
            }

            FindNext(keyword);
        }


        private void PopulateSearchFields()
        {
            cmbSearch.Items.Clear();
            cmbSearch.Items.Add("All");
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                cmbSearch.Items.Add(col.HeaderText);
            }
            cmbSearch.SelectedIndex = 0; // default to All
        }

        private void FindNext(string keyword)
        {
            int totalRows = dataGridView1.Rows.Count;
            if (totalRows == 0) return;

            int startRow = lastSearchRow + 1;
            string selectedField = cmbSearch.SelectedItem?.ToString();

            // Search from current position to end
            for (int i = startRow; i < totalRows; i++)
            {
                if (IsMatch(i, keyword, selectedField))
                {
                    SelectRow(i);
                    return;
                }
            }
            // Wrap around, search from 0 to lastSearchRow
            for (int i = 0; i < startRow; i++)
            {
                if (IsMatch(i, keyword, selectedField))
                {
                    SelectRow(i);
                    return;
                }
            }
            MessageBox.Show("No more results.");
            lastSearchRow = -1;
        }

        private bool IsMatch(int rowIdx, string keyword, string field)
        {
            if (field == "All")
            {
                foreach (DataGridViewCell cell in dataGridView1.Rows[rowIdx].Cells)
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(keyword))
                        return true;
            }
            else
            {
                var cell = dataGridView1.Rows[rowIdx].Cells[field];
                if (cell != null && cell.Value != null && cell.Value.ToString().ToLower().Contains(keyword))
                    return true;
            }
            return false;
        }

        private void SelectRow(int rowIdx)
        {
            dataGridView1.ClearSelection();
            dataGridView1.Rows[rowIdx].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.Rows[rowIdx].Cells[0];
            dataGridView1.FirstDisplayedScrollingRowIndex = rowIdx;
            lastSearchRow = rowIdx;
        }



        // // Usage: Call FindNext(yourKeyword); for both Search and Next button.
        // // Example: FindNext(txtSearch.Text.Trim().ToLower());


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No data to save.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prompt save location
            var sfd = new SaveFileDialog
            {
                Filter = "Monster.dat|Monster.dat",
                Title = "Save Monster.dat"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            // Collect data dari DataGridView ke list<MonsterType>
            var monsters = new List<MonsterType>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                try
                {
                    monsters.Add(new MonsterType
                    {
                        ID = Convert.ToInt32(row.Cells["ID"].Value),
                        Name = Convert.ToString(row.Cells["Name"].Value),
                        Size = Convert.ToInt32(row.Cells["Size"].Value),
                        Icon1 = Convert.ToInt32(row.Cells["Icon1"].Value),
                        Type = Convert.ToInt32(row.Cells["Type"].Value),
                        HP = Convert.ToInt32(row.Cells["HP"].Value),
                        Level = Convert.ToInt32(row.Cells["Level"].Value),
                        Aura1 = Convert.ToString(row.Cells["Aura1"].Value),
                        Aura2 = Convert.ToString(row.Cells["Aura2"].Value)
                    });
                }
                catch
                {
                    // Skip row yang error
                    continue;
                }
            }

            int count = monsters.Count;
            int entrySize = 312;
            using (var fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                // Tulis count (4 bytes)
                bw.Write(count);

                // *Ikut EPL, skip idTable, terus data* (Cina version: abaikan idTable)
                // Tulis semua entry, ikut urutan asal
                foreach (var m in monsters)
                {
                    byte[] b = new byte[entrySize];
                    BitConverter.GetBytes(m.ID).CopyTo(b, 0); // 0-3
                    var nameBytes = Encoding.GetEncoding("GB18030").GetBytes(m.Name ?? "");
                    Array.Copy(nameBytes, 0, b, 4, Math.Min(nameBytes.Length, 16)); // 4-19

                    BitConverter.GetBytes(m.Icon1).CopyTo(b, 20);  // 20-23
                    BitConverter.GetBytes(m.Type).CopyTo(b, 24);   // 24-27
                    BitConverter.GetBytes(m.HP).CopyTo(b, 28);     // 28-31
                    BitConverter.GetBytes(m.Level).CopyTo(b, 32);  // 32-35
                    BitConverter.GetBytes(m.Size).CopyTo(b, 48);   // 48-51

                    var aura1Bytes = Encoding.GetEncoding("GB18030").GetBytes(m.Aura1 ?? "");
                    Array.Copy(aura1Bytes, 0, b, 56, Math.Min(aura1Bytes.Length, 128)); // 56-183

                    var aura2Bytes = Encoding.GetEncoding("GB18030").GetBytes(m.Aura2 ?? "");
                    Array.Copy(aura2Bytes, 0, b, 184, Math.Min(aura2Bytes.Length, 128)); // 184-311

                    bw.Write(b);
                }
            }

            MessageBox.Show($"Saved {count} monster(s) to file.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }




        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("No data to export.");
                return;
            }

            var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            sfd.FileName = "monster.csv";
            if (sfd.ShowDialog() != DialogResult.OK) return;

            var dt = (DataTable)dataGridView1.DataSource;
            var sb = new StringBuilder();

            // Header
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append($"\"{dt.Columns[i]}\"");
                if (i < dt.Columns.Count - 1)
                    sb.Append(",");
            }
            sb.AppendLine();

            // Rows
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string val = row[i]?.ToString() ?? "";
                    // Escape " with ""
                    val = val.Replace("\"", "\"\"");
                    sb.Append($"\"{val}\"");
                    if (i < dt.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Export complete!");
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportCsv();
        }

        private void ImportCsv()
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "CSV File|*.csv|All files|*.*";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                int added = 0, skipped = 0;
                var lines = File.ReadAllLines(ofd.FileName);

                // Option: skip header kalau ada
                int startLine = (lines.Length > 0 && lines[0].ToLower().Contains("id,")) ? 1 : 0;

                for (int idx = startLine; idx < lines.Length; idx++)
                {
                    var fields = ParseCsvLine(lines[idx], dt.Columns.Count);
                    if (fields.Count == 0) continue;

                    // Cari ID, kalau ada skip
                    string idStr = fields[0];
                    if (!int.TryParse(idStr, out int id)) continue;

                    bool exist = false;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (Convert.ToInt32(row["ID"]) == id)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (exist)
                    {
                        skipped++;
                        continue;
                    }

                    // Add new row
                    var newRow = dt.NewRow();
                    for (int col = 0; col < dt.Columns.Count && col < fields.Count; col++)
                    {
                        var colType = dt.Columns[col].DataType;
                        var val = fields[col];

                        if (colType == typeof(int))
                        {
                            if (int.TryParse(val, out int v))
                                newRow[col] = v;
                            else
                                newRow[col] = 0;
                        }
                        else
                        {
                            newRow[col] = val;
                        }
                    }
                    dt.Rows.Add(newRow);
                    added++;
                }

                MessageBox.Show($"Import completed!\nAdded: {added}\nSkipped (duplicate ID): {skipped}");
                // Optionally update window title here if needed
            }
        }


        // Simple CSV parser (support petik dua & koma & escaped quotes)
        private List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (inQuotes)
                {
                    if (c == '\"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '\"')
                        {
                            sb.Append('\"');
                            i++; // skip escaped quote
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == '\"')
                    {
                        inQuotes = true;
                    }
                    else if (c == ',')
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            result.Add(sb.ToString());
            return result;
        }






        private DataObject clipboardRow = null;

        private void AddRow()
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                var newRow = dt.NewRow();
                // Semua int & string = DBNull (supaya kosong dalam grid, bukan 0)
                foreach (DataColumn col in dt.Columns)
                {
                    newRow[col.ColumnName] = DBNull.Value;
                }
                dt.Rows.Add(newRow);

                // Focus ke row terakhir & select
                int lastRow = dataGridView1.Rows.Count - 1;
                if (lastRow >= 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[lastRow].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[lastRow].Cells[0];
                    dataGridView1.FirstDisplayedScrollingRowIndex = lastRow;
                }
            }
            lblStatus.Text = $"New row added. Total: {dataGridView1.Rows.Count}";
        }



        private void CopyRow()
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            var sb = new StringBuilder();

            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var values = new List<string>();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    string val = cell.Value?.ToString() ?? "";
                    // Escape quotes: ganti " dengan ""
                    val = val.Replace("\"", "\"\"");
                    // Add quotes around field
                    values.Add($"\"{val}\"");
                }
                sb.AppendLine(string.Join(",", values));
            }

            Clipboard.SetText(sb.ToString());
        }


        private void PasteRow()
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                // Ambil clipboard text, asingkan baris
                var lines = Clipboard.GetText().Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0) return;

                int startRow = dataGridView1.CurrentRow?.Index ?? 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (startRow + i >= dt.Rows.Count) // Auto add kalau tak cukup row
                        dt.Rows.Add(dt.NewRow());

                    var fields = ParseCsvLine(lines[i], dt.Columns.Count);

                    for (int col = 0; col < dt.Columns.Count && col < fields.Count; col++)
                    {
                        var colType = dt.Columns[col].DataType;
                        var val = fields[col];

                        if (colType == typeof(int))
                        {
                            if (int.TryParse(val, out int v))
                                dt.Rows[startRow + i][col] = v;
                            else
                                dt.Rows[startRow + i][col] = 0;
                        }
                        else
                        {
                            dt.Rows[startRow + i][col] = val;
                        }
                    }
                }
            }
        }

        // Parser untuk CSV dengan quote & escape ""
        private List<string> ParseCsvLine(string line, int expectCols)
        {
            var list = new List<string>();
            int i = 0;
            while (i < line.Length)
            {
                if (line[i] == '"')
                {
                    // quoted field
                    int start = ++i;
                    var sb = new StringBuilder();
                    while (i < line.Length)
                    {
                        if (line[i] == '"' && i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i += 2;
                        }
                        else if (line[i] == '"')
                        {
                            i++;
                            break;
                        }
                        else
                        {
                            sb.Append(line[i++]);
                        }
                    }
                    list.Add(sb.ToString());
                    // skip comma
                    if (i < line.Length && line[i] == ',') i++;
                }
                else
                {
                    // unquoted (should not happen, just fallback)
                    int start = i;
                    while (i < line.Length && line[i] != ',') i++;
                    list.Add(line.Substring(start, i - start));
                    if (i < line.Length && line[i] == ',') i++;
                }
            }
            // pad if less column
            while (list.Count < expectCols)
                list.Add("");
            return list;
        }

        private void ExportSelected()
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            var sfd = new SaveFileDialog { Filter = "CSV|*.csv" };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            using (var sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
            {
                // Header
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    if (i > 0) sw.Write(",");
                    sw.Write($"\"{dataGridView1.Columns[i].HeaderText}\"");
                }
                sw.WriteLine();

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if (i > 0) sw.Write(",");
                        var v = row.Cells[i].Value?.ToString() ?? "";
                        sw.Write($"\"{v.Replace("\"", "\"\"")}\"");
                    }
                    sw.WriteLine();
                }
            }
            MessageBox.Show("Selected rows exported!");
            int selectedCount = dataGridView1.SelectedRows.Count;
            lblStatus.Text = $"Exported {selectedCount} selected row(s).";
        }
        private void DeleteSelected()
        {
            int before = dataGridView1.Rows.Count;
            var dt = dataGridView1.DataSource as DataTable;
            if (dt == null) return;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (!row.IsNewRow)
                    dataGridView1.Rows.Remove(row);
            }
            int after = dataGridView1.Rows.Count;
            lblStatus.Text = $"Deleted {before - after} row(s). {after} remaining.";
        }

        // Keyboard shortcut
        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C) { CopyRow(); e.Handled = true; }
            else if (e.Control && e.KeyCode == Keys.V) { PasteRow(); e.Handled = true; }
            else if (e.Control && e.KeyCode == Keys.N) { AddRow(); e.Handled = true; }
            else if (e.Control && e.KeyCode == Keys.E) { ExportSelected(); e.Handled = true; }
            else if (e.KeyCode == Keys.Delete) { DeleteSelected(); e.Handled = true; }
        }


        // Panggil lepas dataGridView1.DataSource = dt;
        private void HookRowCountUpdater()
        {
            var dt = dataGridView1.DataSource as DataTable;
            if (dt == null) return;

            dt.RowChanged -= Dt_RowChanged;
            dt.RowDeleted -= Dt_RowChanged; // Handler sama, update bila row delete juga

            dt.RowChanged += Dt_RowChanged;
            dt.RowDeleted += Dt_RowChanged;

            UpdateTotalItemTitle();
        }

        private void Dt_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UpdateTotalItemTitle();
        }

        private void UpdateTotalItemTitle()
        {
            var dt = dataGridView1.DataSource as DataTable;
            int total = dt?.Rows.Count ?? 0;
            this.Text = $"Monster.dat Editor by DuaSelipar - Total: {total} items";
        }

        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                int rowIndex = e.RowIndex;
                if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
                {
                    var row = dataGridView1.Rows[rowIndex];
                    bool isEmpty = true;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        var v = cell.Value;
                        if (v != null && v.ToString() != "" && v.ToString() != "0")
                        {
                            isEmpty = false;
                            break;
                        }
                    }
                    if (isEmpty)
                    {
                        // Remove dari DataTable
                        if (rowIndex < dt.Rows.Count)
                        {
                            dt.Rows.RemoveAt(rowIndex);
                        }
                    }
                }
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Just ignore format errors caused by empty/null values on int columns
            e.Cancel = true;
        }








    }
}
