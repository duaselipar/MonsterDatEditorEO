# MonsterDatEditorEO

A modern Windows desktop tool for viewing, editing, and managing `Monster.dat` files from Eudemons Online (EO) private servers.  
Supports full binary editing, import/export, and mass modification features, with a familiar interface based on ItemtypeDatEditorEO.

---

## Features

### 1. Load & View Monster.dat
- **Load Monster.dat**: Instantly loads and parses all monster entries from your EO server's Monster.dat file.
- **Grid View**: Browse all monsters in a sortable, searchable DataGridView.
- **Sorting**: Click column headers to sort by ID, name, HP, type, level, etc.
- **Search**: Quickly filter monsters by any field using the search bar.

### 2. Edit Monsters
- **Inline Editing**: Double-click any cell to edit monster data directly in the grid.
- **Edit Dialog**: Select a monster and edit all fields (ID, Name, Type, HP, Level, Size, Icon1, Aura1, Aura2, etc).
- **Validation**: Field limits and data types are validated to prevent file corruption.

### 3. Add/Delete Monsters
- **Add New**: Create new monster entries with the "Add" button.
- **Delete**: Remove selected monsters instantly.

### 4. Save & Export
- **Save Monster.dat**: Overwrite or save-as to any Monster.dat file, following original EO binary structure.
- **Export CSV**: Export monster data to CSV for easy mass-editing in Excel (if feature implemented).
- **Import CSV**: Import edited monster lists from CSV (if feature implemented).

### 5. Quality-of-Life
- **Row Count**: Status bar always shows current number of monsters loaded.
- **Progress Bar**: Shows load/save progress for large files.
- **Enable/Disable Controls**: UI buttons and menus auto-enable/disable based on file state.
- **Form Title Updates**: Displays total monsters and filename for easy tracking.

---

## Technical Details

- **100% Native C# WinForms** — Fast, stable, no external dependencies.
- **Full Binary Compatibility** — Safely loads and saves original EO Monster.dat (structure: count + entries, 312 bytes per row).
- **Supports Chinese/Unicode** — Uses GB18030 encoding for all name/aura text fields.
- **Safe Editing** — Edits in memory; original file untouched until saved.

---

## How To Use

1. **Open**: Click "Load" and select your Monster.dat file.
2. **Edit**: Modify, add, or delete monsters as needed.
3. **Save**: Click "Save" to write changes to disk.
4. *(Optional)* **Export/Import CSV**: Use CSV features for mass editing (if available).
5. **Enjoy**: Deploy your updated Monster.dat to your EO server.


---

