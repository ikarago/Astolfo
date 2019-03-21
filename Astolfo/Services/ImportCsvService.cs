using Astolfo.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astolfo.Services
{
    public static class ImportCsvService
    {
        // Load csv
        public static ObservableCollection<VoiceTextModel> ImportFromCsv()
        {
            ObservableCollection<VoiceTextModel> items = new ObservableCollection<VoiceTextModel>();

            // TODO Import from CSV file

            return items;
        }

        public static ObservableCollection<VoiceTextModel> UseSampleData()
        {
            ObservableCollection<VoiceTextModel> items = new ObservableCollection<VoiceTextModel>();

            items.Add(new VoiceTextModel { Id = 1, Text = "Oh boy", Key = "boy"});
            items.Add(new VoiceTextModel { Id = 2, Text = "Oh girl", Key = "girl" });
            items.Add(new VoiceTextModel { Id = 3, Text = "Oh noes", Key = "noes" });
            items.Add(new VoiceTextModel { Id = 4, Text = "Oh shit", Key = "shit" });
            items.Add(new VoiceTextModel { Id = 5, Text = "Oh crap", Key = "crap" });
            items.Add(new VoiceTextModel { Id = 6, Text = "Oh holy moly", Key = "holy moly" });
            items.Add(new VoiceTextModel { Id = 7, Text = "Oh snap", Key = "sn-ap" });
            items.Add(new VoiceTextModel { Id = 8, Text = "Oh boy", Key = "boy" });
            items.Add(new VoiceTextModel { Id = 9, Text = "Oh boy", Key = "boy" });



            return items;
        }
    }
}
