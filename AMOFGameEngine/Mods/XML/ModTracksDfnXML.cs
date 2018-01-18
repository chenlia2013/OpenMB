﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AMOFGameEngine.Mods.XML
{
    public enum TrackType
    {
        EngineTrack,
        ModuleTrack,
        Scene
    }

    [XmlRoot("Track")]
    public class ModTrackDfnXML
    {
        [XmlElement("Id")]
        public string Id { get; set; }
        [XmlElement("File")]
        public string File { get; set; }
        [XmlElement("Type")]
        public TrackType Type { get; set; }
    }

    [XmlRoot("Tracks")]
    public class ModTracksDfnXML
    {
        [XmlElement("Track")]
        public List<ModTrackDfnXML> Tracks { get; set; }
    }
}
