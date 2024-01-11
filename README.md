# Overview
![image](https://github.com/clarinetJWD/HCS-Library-Tool/assets/71787034/bff88dfc-4de8-4bfd-8c20-ee194342f1be)

This tool allows members of the Houston Civic Symphony Music Selection Committee to view information about our current library, repertoire history, and musician recommendations in a sortable, filterable, searchable interface.

# Installation
> [!NOTE]
> Prerequisites: This is a .Net Framework application, and requires a Windows 10 or 11 computer (or emulation tool).

To install, go to https://joedombrowski.com/apps/hcs-library-app/ and click the Install button.

> [!IMPORTANT]
> Windows may display one or more messages like "Windows protected your PC" when installing. This is simply because this isn't a well known program, and hasn't been white-listed by Microsoft.
>
> This program poses no threat to your computer.
>
> Click "More Info" and "Run Anyway"

Updates are handled automatically when the program launches.

This program can be uninstalled in the Windows Add or Remove Programs tool.

# User Guide
## Initial Setup

All that's needed to run this program is the HCS music selection committee's passcode. 

Enter it in the form when the application launches, and it will connect automatically, download the latest library file, and load.

> [!NOTE]
> The credentials are managed, and you may lose access when your tenure on the committee ends. When this happens, the program will prompt you for a passcode, and your current one will no longer work.

## Basic Usage

### The Music Recommendations Tab
The first tab is the Music Recommendations Tab. It includes a list of:
- All music that is owned by HCS
- All music that HCS has ever performed
- All music recommended by HCS members using the recommendation form

On this tab, you can search, filter, sort, and group the data in any way you like to facilitate your duties.

Many rows have a [+] button that when clicked will open more information on the recommended piece, including:
1. The list of years HCS performed the work
2. The list of people who recommended the work, along with any notes they included.
3. Alternate spellings of the Title and Composer's name (used to merge the different data sources into a single line per piece).

### The Composers Tab
The second tab shows all composers who we have in the library, as well as their complete list of aliases.

This tab is used for managing and merging composers (See editing data)

### Customization
**Search**
* To search, click the magnifying glass on the grid (or press Ctrl+F) and type your search terms.
* Alrernately, you can right click a column header, and select "Show auto filter row", and search by terms per-column.

**Sort**
* To sort data, just click the column header.

**Grouping**
* To group by a column, drag the column into the grouping area above the grid and drop.
* To ungroup, drag the grouped column box back into the row of column headers.

**Advanced Filtering**
* Mouse over the column header and click the small Filter button.
* From here, you can filter based on ranges, text, values, etc.
* Column filters are shown at the bottom of the grid. From here, you can edit and remove a filter.

This is most useful for showing only pieces we own already, or only pieces that we have not performed in 10+ years.

**Cell Formatting**
TODO

## Editing Data
> [!IMPORTANT]
> Here Be Dragons: Don't go editing stuff and saving unless you're sure you know what you're doing!

### Merging Duplicates
These instructions are the same for Recommendations and Composers.
1. Select one or more rows (ctrl+click or shift+click to select multiple rows).
2. Click "Merge or Edit Selected".
3. If you have more than one item selected, you WILL merge them.
4. Select the primary attributes (Composer and/or Title). You can type a custom value if none of the existing ones are correct.
5. Click the Merge/Save button to confirm changes.
6. Select Save All from the File menu.
> [!NOTE]
> This will save changes on the server, and affect ALL users. Don't save unless you're sure what you see is correct.
>
> If a server upload fails, it will ask you to export instead. Save the exported file, and send it to me to merge in.

