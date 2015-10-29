using System;
using AGI.Foundation;
using AGI.Foundation.Celestial;
using AGI.Foundation.Coordinates;
using AGI.Foundation.Geometry;
using AGI.Foundation.Infrastructure.Threading;
using AGI.Foundation.Time;
using AGI.Foundation.Tracking;

namespace CzmlRealtimeServer.Models
{
    public enum Force
    {
        Other = 0,
        Friendly = 1,
        Opposing = 2,
        Neutral = 3
    }

    /// <summary>
    /// The ExampleEntityDescriptor which defines general information
    /// regarding all instances of ExampleEntity.  See the "Creating
    /// and Managing Entities" section of the Tracking Library
    /// documentation for more information.
    /// </summary>
    public class ExampleEntityDescriptor :
        EntityDescriptor<ExampleEntity>,
        IEntityPositionDescriptor,
        IEntityOrientationDescriptor
    {
        #region IEntityPositionDescriptor Members

        public ReferenceFrame PositionReferenceFrame
        {
            get { return CentralBodiesFacet.GetFromContext().Earth.FixedFrame; }
        }

        #endregion

        #region IEntityOrientationDescriptor Members

        public Axes OrientationAxes
        {
            get { return PositionReferenceFrame.Axes; }
        }

        #endregion
    }

    /// <summary>
    /// Our ExampleEntity See the "Creating and Managing Entities" section of
    /// the Tracking Library documentation for more information.
    /// </summary>
    public class ExampleEntity :
        IEntityIdentifier,
        IEntityLastUpdate,
        IEntityPosition,
        IEntityOrientation
    {
        public static void RegisterEntityClass()
        {
            EntityDescriptor<ExampleEntity>.Default = new ExampleEntityDescriptor();
        }

        public ExampleEntity(TransactionContext context, string callSign)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (callSign == null)
            {
                throw new ArgumentNullException("callSign");
            }

            m_entityIdentifier = callSign;
            m_lastUpdate = new TransactedProperty<JulianDate>(context, this);
            m_lastUpdateDateTime = new TransactedProperty<DateTime>(context, this);
            m_position = new TransactedProperty<Cartesian>(context, this);
            m_orientation = new TransactedProperty<UnitQuaternion>(context, this);
            m_symbolID = new TransactedProperty<string>(context, this);
            m_marking = new TransactedProperty<string>(context, this, callSign);
            m_force = new TransactedProperty<Force>(context, this, Force.Other);
        }

        #region IEntityIdentifier Members

        public object EntityIdentifier
        {
            get { return m_entityIdentifier; }
        }

        #endregion

        #region IEntityLastUpdate Members

        public TransactedProperty<JulianDate> LastUpdate
        {
            get { return m_lastUpdate; }
        }

        #endregion

        #region IEntityPosition Members

        public TransactedProperty<Cartesian> Position
        {
            get { return m_position; }
        }

        #endregion

        #region IEntityOrientation Members

        public TransactedProperty<UnitQuaternion> Orientation
        {
            get { return m_orientation; }
        }

        #endregion

        public TransactedProperty<string> Marking
        {
            get { return m_marking; }
        }

        public TransactedProperty<string> Symbology
        {
            get { return m_symbolID; }
        }

        public TransactedProperty<Force> Affiliation
        {
            get { return m_force; }
        }

        public TransactedProperty<DateTime> LastUpdateDateTime
        {
            get { return m_lastUpdateDateTime; }
        }

        private string m_entityIdentifier;
        private TransactedProperty<string> m_marking;
        private TransactedProperty<Cartesian> m_position;
        private TransactedProperty<JulianDate> m_lastUpdate;
        private TransactedProperty<DateTime> m_lastUpdateDateTime;
        private TransactedProperty<string> m_symbolID;
        private TransactedProperty<UnitQuaternion> m_orientation;
        private TransactedProperty<Force> m_force;
    }
}
