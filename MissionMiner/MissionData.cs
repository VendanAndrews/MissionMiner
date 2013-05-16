using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace MissionMiner
{
    class MissionData
    {
        public string Name;
        public string Asteroid;
        public int Volume;
        public int VolumePerRoid;
        public bool Obstacles;

        private MissionData(string Name, string Asteroid, int Volume, int VolumePerRoid, bool Obstacles)
        {
            this.Name = Name;
            this.Asteroid = Asteroid;
            this.Volume = Volume;
            this.VolumePerRoid = Volume;
            this.Obstacles = Obstacles;
        }

        static List<MissionData> _All;
        public static List<MissionData> All
        {
            get
            {
                if (_All == null)
                {
                    _All = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("MissionMiner.Missions.xml")).Root.Elements().Select(
                        e => 
                            new MissionData(e.Element("Name").Value, 
                                e.Elements("Asteroid").Count() > 0 ? e.Element("Asteroid").Value : "", 
                                int.Parse(e.Element("Volume").Value),
                                e.Elements("VolumePerRoid").Count() > 0 ? int.Parse(e.Element("VolumePerRoid").Value) : 0,
                                e.Elements("HasObstacles").Count() > 0 ? bool.Parse(e.Element("HasObstacles").Value) : false)).ToList();
                }
                return _All;
            }
        }



    }
}
