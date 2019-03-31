using Astolfo.Core.Models;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace Astolfo.Core.Services
{
    public static class ImportService
    {
        // Methods
        public static ObservableCollection<VoiceTextModel> ImportFromXlsx(StorageFile file)
        {
            // Create the list
            ObservableCollection<VoiceTextModel> items = new ObservableCollection<VoiceTextModel>();

            using (ExcelPackage package = new ExcelPackage(new FileInfo(file.Path)))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets[0];

                // #DIRTY Get a query to check how many entries there are in the Key-row and use that number to set the max amount of rows
                var queryMaxRows = (from cell in sheet.Cells["A:A"] select cell);
                int maxRows = queryMaxRows.Count();

                // Go though all the rows and get the data.
                //  Starting at row 2 though, because row 1 has header info we don't want in our items
                for (int currentRow = 2; currentRow <= maxRows; currentRow++)
                {
                    // Build the query
                    // #TODO Make this less crap and more streamlined
                    string addressKey = "A" + currentRow;
                    string addressText = "H" + currentRow;
                    var queryKey = (from cell in sheet.Cells[addressKey] select cell);
                    var queryText = (from cell in sheet.Cells[addressText] select cell);

                    // Get the value from the cell
                    string key = "";
                    string text = "";
                    foreach (var cell in queryKey)
                    {
                        key = (string)cell.Value;
                    }
                    foreach (var cell in queryText)
                    {
                        text = (string)cell.Value;
                    }

                    // Insert these into a new items thingie and add it to the items list
                    items.Add(new VoiceTextModel
                    {
                        Id = (currentRow - 1),
                        Key = key,
                        Text = text
                    });
                }
            }

            // Return the list with all the items
            return items;
        }
    }
}
