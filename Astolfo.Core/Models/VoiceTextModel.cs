using Windows.Media.SpeechSynthesis;

namespace Astolfo.Core.Models
{
    public class VoiceTextModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
        public VoiceInformation Voice { get; set; }
        public bool Done { get; set; }
        public bool SuccessfulExport { get; set; }
    }
}
