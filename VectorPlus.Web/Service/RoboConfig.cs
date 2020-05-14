using System;
using System.Net;
using Anki.Vector;

namespace VectorPlus.Web.Service
{
    public class RoboConfig
    {
        public Guid RoboConfigId { get; set; }

        public bool UseEnvironment { get; set; }

        public string RobotName { get; set; }
        public string RobotSerial { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RobotGuid { get; set; }
        public string RobotCert { get; set; }
        public string IpOverrideStr { get; set; }

        public RobotConfiguration ToRobotConfiguration()
        {
            return new RobotConfiguration()
            {
                Certificate = RobotCert,
                Guid = RobotGuid,
                RobotName = RobotName,
                SerialNumber = RobotSerial,
                IPAddress = IpOverrideStr == null ? null : IPAddress.Parse(IpOverrideStr)
            };
        }
    }
}
 