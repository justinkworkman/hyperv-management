using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using HyperVRemote.Source.Interface;

namespace HyperVRemote.Source.Implementation
{

    public class HyperVMachine : IHyperVMachine
    {
        private readonly ManagementObject _rawMachine;

        public HyperVMachine(ManagementObject rawMachine)
        {
            _rawMachine = rawMachine;         
        }

        public ManagementObject FetchRawMachine()
        {
            return _rawMachine;
        }

        public string GetName()
        {
            return _rawMachine["ElementName"] as string;
        }

        public HyperVStatus GetStatus()
        {           
            return (HyperVStatus)_rawMachine["EnabledState"];
        }
        public long GetUptime()
        {
            return long.Parse(_rawMachine["OnTimeInMilleseconds"].ToString());
        }


        public void Reset()
        {
            ChangeState(HyperVStatus.Reset);
        }

        public void Start()
        {
            ChangeState(HyperVStatus.Running);
        }

        public void Stop()
        {
            ChangeState(HyperVStatus.Off);
        }

        public void RestoreLastSnapShot()
        {
            var lastSnapshot = GetSnapshotsQuery("Msvm_MostCurrentSnapshotInBranch")
                .FirstOrDefault();

            if (lastSnapshot == null)
            {
                throw new HyperVException("No Snapshot found");
            }
          
            ApplySnapshot(lastSnapshot);
        }

        public void RestoreSnapShotByName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var snapshot = GetSnapshotsQuery("Msvm_SnapshotOfVirtualSystem")
                .FirstOrDefault(item => name == item["ElementName"] as string);

            if (snapshot == null)
            {
                throw new HyperVException("No Snapshot found");
            }

            ApplySnapshot(snapshot);
        }

        private void ApplySnapshot(ManagementObject lastSnapshot)
        {
            var scope = _rawMachine.Scope;

            var managementService = new ManagementClass(scope, new ManagementPath("Msvm_VirtualSystemSnapshotService"), null)
                .GetInstances()
                .OfType<ManagementObject>()
                .First();

            var inParameters = managementService.GetMethodParameters("ApplySnapshot");

            inParameters["Snapshot"] = lastSnapshot.Path.Path;

            managementService.InvokeMethod("ApplySnapshot", inParameters, null);
        }

        private IEnumerable<ManagementObject> GetSnapshotsQuery(string relation)
        {
            return _rawMachine.GetRelated(
                    "Msvm_VirtualSystemSettingData",
                    relation,
                    null,
                    null,
                    "Dependent",
                    "Antecedent",
                    false,
                    null)
                .OfType<ManagementObject>();
        }

        private uint ChangeState(HyperVStatus state)
        {
            var raw = _rawMachine;
            var scope = _rawMachine.Scope;

            var managementService = new ManagementClass(scope, new ManagementPath("Msvm_VirtualSystemManagementService"), null)
                .GetInstances()
                .OfType<ManagementObject>().FirstOrDefault();

            if (managementService != null)
            {
                var inParameters = managementService.GetMethodParameters("RequestStateChange");

                inParameters["RequestedState"] = (object)state;

                var outParameters = raw.InvokeMethod("RequestStateChange", inParameters, null);

                Debug.WriteLine("Changed state with return " + outParameters);

                if (outParameters != null) {
                    return (uint)outParameters["ReturnValue"];
                }
            }
            else
            {
                throw new HyperVException("Could not find machine management service for rstate change");
            }

            return 0;
        }

    }
}
