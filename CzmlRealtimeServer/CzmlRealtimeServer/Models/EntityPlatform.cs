using AGI.Foundation.Celestial;
using AGI.Foundation.Cesium;
using AGI.Foundation.Coordinates;
using AGI.Foundation.Geometry;
using AGI.Foundation.Platforms;
using AGI.Foundation.Time;
using CzmlRealtimeServer.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace CzmlRealtimeServer.Models
{
    public class EntityPlatform
    {
        public EntityPlatform(ExampleEntity entity, EarthCentralBody earth, string dataPath)
        {
            //id, string
            m_id = entity.EntityIdentifier.ToString();

            //name, string
            string name = entity.Marking.ToString();

            //Last Update, DateTime
            DateTime time = DateTime.Parse(entity.LastUpdateDateTime.ToString());

            //Position, Cartesian
            string[] xyz = entity.Position.ToString().Split(',');
            Cartesian position = new Cartesian(Convert.ToDouble(xyz[0]), Convert.ToDouble(xyz[1]), Convert.ToDouble(xyz[2]));

            m_platform = new Platform();
            m_platform.Name = name;
            m_platform.LocationPoint = new PointCartographic(earth, earth.Shape.CartesianToCartographic(position));
            m_platform.OrientationAxes = new AxesVehicleVelocityLocalHorizontal(earth.FixedFrame, m_platform.LocationPoint);

            LabelGraphicsExtension labelExtension = new LabelGraphicsExtension(new LabelGraphics
            {
                Text = new ConstantCesiumProperty<string>(name),
                Scale = new ConstantCesiumProperty<double>(0.5),
                FillColor = new ConstantCesiumProperty<Color>(Color.White),
                PixelOffset = new ConstantCesiumProperty<Rectangular>(new Rectangular(35, -15))
            });
            m_platform.Extensions.Add(labelExtension);

            string symbol = entity.Symbology.ToString().Replace('*', '_') + ".png";
            CesiumResource billboardResource = new CesiumResource(new Uri(dataPath + symbol), CesiumResourceBehavior.LinkTo);
            BillboardGraphicsExtension billboardExtension = new BillboardGraphicsExtension(new BillboardGraphics
            {
                Image = new ConstantCesiumProperty<CesiumResource>(billboardResource),
                Show = true,
                Scale = new ConstantCesiumProperty<double>(1)
            });
            m_platform.Extensions.Add(billboardExtension);

            m_czmlDocument = new CzmlDocument();
            m_czmlDocument.Name = "Realtime";
            m_czmlDocument.RequestedInterval = new TimeInterval(new JulianDate(DateTime.Now), new JulianDate(DateTime.Now.AddDays(1.0)));
            m_czmlDocument.Clock = new Clock
            {
                Step = ClockStep.SystemClock
            };

            m_czmlDocument.ObjectsToWrite.Add(m_platform);
        }

        public string GetCzml()
        {
            StringWriter sw = new StringWriter();
            m_czmlDocument.WriteDocument(sw);

            //Replace the random Guid with this Entity's ID
            string s1 = sw.ToString();
            int i1 = s1.LastIndexOf("\"id\":");
            string s2 = s1.Substring(i1 + 6, 36); 
            string s3 = s1.Replace(s2, m_id.ToString());

            return s3;
        } 

        private Platform m_platform;
        public Platform Platform 
        { 
            get { return m_platform; } 
        }
        
        private CzmlDocument m_czmlDocument;
        public CzmlDocument CzmlDocument
        {
            get { return m_czmlDocument; }
        }

        private string m_id;
        public string ID
        {
            get { return m_id; }
        }
    }
}