using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using AGI.Foundation;
using AGI.Foundation.Tracking;
using AGI.Foundation.Infrastructure.Threading;
using System.IO;
using AGI.Foundation.Coordinates;
using AGI.Foundation.Time;
using AGI.Foundation.Cesium;
using Newtonsoft.Json;
using AGI.Foundation.Platforms;
using AGI.Foundation.Celestial;
using AGI.Foundation.Geometry;
using CzmlRealtimeServer.Models;

namespace CzmlRealtimeServer
{
    /// <summary>
    /// A static class for managing the Tracking Library data feed as well
    /// as the list of KmlWriters needed to supply results to the clients.
    /// Each session initiated by accessing the KmlNetworkLink page gets
    /// a unique KmlWriter to monitor refresh requests for each client.
    /// </summary>
    public static class SessionManager
    {
        static public void Initialize(string dataPath, string virtualPath)
        {
            Licensing.ActivateLicense(
                @"<License>
                <Field name='Name'>Daniel Honaker</Field>
                <Field name='sfId'>003j000000RSOxBAAX</Field>
                <Component name='Dynamic Geometry Library' expiration='2015/11/18' />
                <Component name='Navigation Accuracy Library' expiration='2015/11/18' />
                <Component name='Terrain Analysis Library' expiration='2015/11/18' />
                <Component name='Spatial Analysis Library' expiration='2015/11/18' />
                <Component name='Communications Library' expiration='2015/11/18' />
                <Component name='Insight3D' expiration='2015/11/18' />
                <Component name='Tracking Library' expiration='2015/11/18' />
                <Component name='Route Design Library' expiration='2015/11/18' />
                <Component name='Orbit Propagation Library' expiration='2015/11/18' />
                <Component name='Auto-Routing Library' expiration='2015/11/18' />
                <Signature>SJH4qBqzoL2HKM+Q9TbP7NkFk56vYPoroBRERMWikQum8SQF9bBL35E4N0JUCxpIdMVETg2l3JSd4ddGUHmm33+r6Iamau3SYhzyZsk4dmeEztt3HpZiKZsxr4bcEuLSpCe/8qMXxoJUrbr0K34fELpeOSb5FIGHjxdExsafF1E=</Signature>
                </License>");
            m_virtualPath = virtualPath;

            m_earth = CentralBodiesFacet.GetFromContext().Earth;

            //Make sure the default descriptor for the ExampleEntity class is set.
            ExampleEntity.RegisterEntityClass();

            //Create the primary TransactionContext that will be used throughout the application.
            TransactionContext context = new TransactionContext();
            context.Committed += context_Committed;

            //Create the master entity set that will be populated by our data feed.
            m_entities = new EntitySet<ExampleEntity>(context);
            m_entities.Changed += m_entities_Changed;

            //Create and start the data feed.
            m_provider = new ExampleEntityProvider(dataPath, m_entities);
            m_provider.Start();
        }

        static void m_entities_Changed(object sender, EntitySetChangedEventArgs<ExampleEntity> e)
        {

            if (e.Added.Count > 0)
            {
                IEnumerator<ExampleEntity> addedEnum = e.Added.GetEnumerator();
                addedEnum.MoveNext();
                ExampleEntity entity = addedEnum.Current as ExampleEntity;
                EntityPlatform platform = new EntityPlatform(entity, m_earth, m_virtualPath);

                m_platforms.Add(entity.EntityIdentifier, platform);
            }

            if (e.RemovedIdentifiers.Count > 0)
            {
                IEnumerator<object> removedEnum = e.RemovedIdentifiers.GetEnumerator();
                removedEnum.MoveNext();

                string removedID = removedEnum.Current.ToString();
                m_platforms.Remove(removedEnum.Current);

                //Write to the Cesium client to remove the entity
                foreach (var kvp in _outputs.ToArray())
                {
                    StreamWriter responseStreamWriter = kvp.Value;
                    try
                    {
                        responseStreamWriter.WriteLine("data:{\"removeID\":\"" + removedID + "\"}\n");
                        responseStreamWriter.Flush();
                    }
                    catch { }
                }
            }

        }

        static void context_Committed(object sender, AGI.Foundation.Infrastructure.Threading.TransactionCommittedEventArgs e)
        {
            //Get the enumerator for the CommittedEventArgs and move to a defined value
            IEnumerator<TransactedObject> o = e.CommittedObjects.GetEnumerator();
            o.MoveNext();
            o.MoveNext();

            ExampleEntity entity = o.Current.Owner as ExampleEntity;

            try
            {
                EntityPlatform platform = m_platforms[entity.EntityIdentifier];

                //Position, Cartesian
                string transProp5 = entity.Position.ToString();
                string[] xyz = transProp5.Split(',');
                Cartesian position = new Cartesian(Convert.ToDouble(xyz[0]), Convert.ToDouble(xyz[1]), Convert.ToDouble(xyz[2]));

                platform.Platform.LocationPoint = new PointCartographic(m_earth, m_earth.Shape.CartesianToCartographic(position));

                foreach (var kvp in _outputs.ToArray())
                {
                    StreamWriter responseStreamWriter = kvp.Value;
                    try
                    {
                        responseStreamWriter.WriteLine("data:" + platform.GetCzml() + "\n");
                        responseStreamWriter.Flush();
                    }
                    catch { }
                }
            }
            catch { }

        }

        public static ConcurrentDictionary<StreamWriter, StreamWriter> Outputs
        {
            get
            {
                return _outputs;
            }
        }

        public static string VirtualPath
        {
            get
            {
                return m_virtualPath;
            }
        }
        private static EntitySet<ExampleEntity> m_entities;
        private static ExampleEntityProvider m_provider;
        private static ConcurrentDictionary<StreamWriter, StreamWriter> _outputs = new ConcurrentDictionary<StreamWriter, StreamWriter>();

        private static EarthCentralBody m_earth;
        private static Dictionary<object, EntityPlatform> m_platforms = new Dictionary<object, EntityPlatform>();
        private static string m_virtualPath;

    }
}
