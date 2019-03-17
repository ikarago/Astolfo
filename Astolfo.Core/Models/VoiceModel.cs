﻿using System;
using System.Collections.Generic;
using System.Text;
using Windows.Media.SpeechSynthesis;

namespace Astolfo.Core.Models
{
    public class VoiceModel
    {
        public VoiceInformation Voice { get; set; }
        public string VoiceId { get { return Voice.Id; } }
        public string VoiceName { get { return Voice.DisplayName; } }
        public string VoiceGender { get { return Voice.Gender.ToString(); } }
        public string VoiceLanguage { get { return Voice.Language; } }
        public string VoiceComboBoxName { get { return Voice.DisplayName + " (" + Voice.Language + ", " + Voice.Gender + ")"; } }

        // Contructor
        public VoiceModel(VoiceInformation voice)
        {
            Voice = voice;
        }
    }
}
