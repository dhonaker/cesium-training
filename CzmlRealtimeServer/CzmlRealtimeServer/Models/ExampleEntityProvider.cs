using System;
using System.Globalization;
using System.IO;
using System.Threading;
using AGI.Foundation;
using AGI.Foundation.Coordinates;
using AGI.Foundation.Time;
using AGI.Foundation.Tracking;

namespace CzmlRealtimeServer.Models
{
    class ExampleEntityProvider
    {
        public ExampleEntityProvider(string dataPath, EntitySet<ExampleEntity> entities)
        {
            m_dataPath = dataPath;
            m_entities = entities;
            m_active = false;
        }

        public EntitySet<ExampleEntity> Entities
        {
            get
            {
                return m_entities;
            }
        }

        public void Start()
        {
            if (!m_active)
            {
                m_active = true;
                m_thread = new Thread(WorkerThread);
                m_thread.IsBackground = true;
                m_thread.Start();
            }
        }

        public void Stop()
        {
            if (m_active)
            {
                m_active = false;
                m_thread.Join();
                m_thread = null;
            }
        }

        private void WorkerThread()
        {
            while (m_active)
            {
                JulianDate time = new JulianDate(DateTime.UtcNow);

                //FIXED WIDTH FORMAT
                //ID(12) X(20) Y(20) Z(20) QX(20) QY(20) QZ(20) QW(20) Marking(12) Symbology(16) Force(2)
                string[] records = File.ReadAllLines(Path.Combine(m_dataPath, "surveillance.txt"));

                foreach (string record in records)
                {
                    if (!m_active)
                    {
                        break;
                    }

                    string entityID = record.Substring(0, 12).Trim();
                    int delay = int.Parse(record.Substring(12, 4).Trim(), CultureInfo.InvariantCulture);
                    double x = double.Parse(record.Substring(16, 20).Trim(), CultureInfo.InvariantCulture);
                    double y = double.Parse(record.Substring(36, 20).Trim(), CultureInfo.InvariantCulture);
                    double z = double.Parse(record.Substring(56, 20).Trim(), CultureInfo.InvariantCulture);
                    double qx = double.Parse(record.Substring(76, 20).Trim(), CultureInfo.InvariantCulture);
                    double qy = double.Parse(record.Substring(96, 20).Trim(), CultureInfo.InvariantCulture);
                    double qz = double.Parse(record.Substring(116, 20).Trim(), CultureInfo.InvariantCulture);
                    double qw = double.Parse(record.Substring(136, 20).Trim(), CultureInfo.InvariantCulture);
                    string marking = record.Substring(156, 12).Trim();
                    string symbology = record.Substring(168, 16).Trim();
                    Force force = (Force)int.Parse(record.Substring(184, 2).Trim(), CultureInfo.InvariantCulture);

                    m_entities.Context.DoTransactionally(
                        delegate(Transaction transaction)
                        {
                            ExampleEntity entity = m_entities.GetEntityById(transaction, entityID);
                            if (entity == null)
                            {
                                entity = new ExampleEntity(m_entities.Context, entityID);
                                m_entities.Add(transaction, entity);
                            }

                            entity.LastUpdate.SetValue(transaction, time);
                            entity.LastUpdateDateTime.SetValue(transaction, time.ToDateTime());
                            entity.Marking.SetValue(transaction, marking);
                            entity.Orientation.SetValue(transaction, new UnitQuaternion(qw, qx, qy, qz));
                            entity.Position.SetValue(transaction, new Cartesian(x, y, z));
                            entity.Affiliation.SetValue(transaction, force);
                            entity.Symbology.SetValue(transaction, symbology);
                        });

                    if (delay > 0)
                    {
                        //Remove anything that hasn't been updated.
                        
                        m_entities.Context.DoTransactionally(
                            delegate(Transaction transaction)
                            {
                                foreach (ExampleEntity entity in m_entities.GetEntities(transaction))
                                {
                                    if (time.Subtract(entity.LastUpdate.GetValue(transaction)).TotalSeconds > .5)
                                    {
                                        m_entities.Remove(transaction, entity);
                                    }
                                }
                            });
                        
                        Thread.Sleep(delay);
                        time = time.AddSeconds((double)delay / 1000.0);
                    }
                }
            }
        }


        EntitySet<ExampleEntity> m_entities;
        Thread m_thread;
        string m_dataPath;
        volatile bool m_active;
    }
}
