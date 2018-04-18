using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;

namespace IoTSimulate
{
    //TODO:实现BroadcastPackageWirelessSensorNetwork的一个单片机接口
    public partial class VtmDev
    {
        private AnonymousPipeServerStream BPWSN_Rx, BPWSN_Tx;

        public WLPackageDev WirelessPackageDevice = new WLPackageDev();
        private BPWSN_DevicePipe BPWSN_DevPipe;

        [VtmFunction(VtmFunctionAttribute.FunctionType.Init)]
        private void Init_BPWSN()
        {
            BPWSN_Rx = new AnonymousPipeServerStream(PipeDirection.In, System.IO.HandleInheritability.Inheritable);
            BPWSN_Tx = new AnonymousPipeServerStream(PipeDirection.Out, System.IO.HandleInheritability.Inheritable);

            args += " -bpwsni " + BPWSN_Tx.GetClientHandleAsString();
            args += " -bpwsno " + BPWSN_Rx.GetClientHandleAsString();

            BPWSN_DevPipe = new BPWSN_DevicePipe(BPWSN_Rx, BPWSN_Tx, WirelessPackageDevice);
        }
        

        [VtmFunction(VtmFunctionAttribute.FunctionType.Update)]
        private void Update_BPWSN()
        {
            BPWSN_DevPipe.Update();
        }

        [VtmFunction(VtmFunctionAttribute.FunctionType.Close)]
        private void Close_BPWSN()
        {
            BPWSN_DevPipe.Close();
            WirelessPackageDevice.Close();
        }

        class BPWSN_DevicePipe
        {
            private PipeStream istr, ostr;
            private WLPackageDev packageDev;

            Task task;
            private byte[] buff = new byte[WLPackageDev.PACKAGE_SIZE];

            public BPWSN_DevicePipe(PipeStream instr,PipeStream outstr,WLPackageDev packageDev)
            {
                istr = instr;
                ostr = outstr;
                this.packageDev = packageDev;

                packageDev.OnPackageArrive = (pkg) =>
                {
                    foreach (var b in pkg)
                        ostr.WriteByte(b);
                };
                ostr.Flush();
                NextRead();
            }

            private void NextRead()
            {
                task?.Dispose();
                task = new Task(() =>
                {
                    for(int i = 0; i < buff.Length; i++)
                    {
                        buff[i] = (byte)istr.ReadByte();
                    }
                });
                task.Start();
            }

            public void Update()
            {
                packageDev.Update();
                //istr to packageDev
                if (task.IsCompleted)
                {
                    packageDev.SendPackage(buff, 0, true);
                    NextRead();
                }
            }
            public void Close()
            {
                
            }
        }

    }
}
