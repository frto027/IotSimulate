using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSimulate.BroadPackageWSN
{
    public enum SensorType { TempSensor }
    public class BPWSNPackage
    {
        public const int PACKAGE_SIZE = WLPackageDev.PACKAGE_SIZE;
        /// <summary>
        /// 整个封包的Data数据
        /// </summary>
        public readonly byte[] Data = new byte[PACKAGE_SIZE];
        /// <summary>
        /// 消息长度
        /// </summary>
        public int Length { get { return MessageLength; } }
        private const int
            SensorTypeIndex = 2,
            SensorIdIndex = 3,
            MessageBeginIndex = 4,
            MessageLength = 11,
            CheckByteIndex = 15;
        //0 0x55
        //1 0xaa
        //2 SensorType
        //3 SensorId
        //4 - 14 Message
        //15 CRC

        public SensorType SensorType
        {
            get { return (SensorType)Enum.ToObject(typeof(SensorType),Data[SensorTypeIndex]); }
            set { Data[SensorTypeIndex] = (byte)value; }
        }

        public byte SensorId
        {
            get { return Data[SensorIdIndex]; }
            set { Data[SensorIdIndex] = value; }
        }
        /// <summary>
        /// 提供消息的克隆
        /// </summary>
        public byte[] Message
        {
            get {
                byte[] ret = new byte[MessageLength];
                for(int i = 0; i < ret.Length; i++)
                {
                    ret[i] = Data[i + MessageBeginIndex];
                }
                return ret;
            }
        }
        /// <summary>
        /// 提供对消息的直接访问
        /// </summary>
        /// <param name="index">消息下标</param>
        public byte this[int index]
        {
            set {
                Data[index + MessageBeginIndex] = value;
            }
            get
            {
                return Data[index + MessageBeginIndex];
            }
        }

        public BPWSNPackage()
        {
            WLPackageDev.Head.CopyTo(Data, 0);
        }

        public BPWSNPackage(byte[] arr) : this()
        {
            for(int i = 0; i < PACKAGE_SIZE; i++)
            {
                Data[i] = arr[i];
            }
        }

        public void SetCheckByte()
        {
            //余数校验
            byte x = 0;
            for(int i = 0; i < CheckByteIndex; i++)
            {
                x += Data[i];
            }
            Data[CheckByteIndex] = x;
        }

        public bool IsCheckOK {
            get {
                byte x = 0;
                for (int i = 0; i < CheckByteIndex; i++)
                {
                    x += Data[i];
                }
                return x == Data[CheckByteIndex];
            }
        }
    }
}
