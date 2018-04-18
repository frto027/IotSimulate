using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSimulate
{
    /// <summary>
    /// 封包封装的无线设备，封包大小为PACKAGE_SIZE
    /// 如果使用此类，不要混用SendMessage和ReadMessage
    /// </summary>
    public class WLPackageDev:WLDev
    {
        public const int PACKAGE_SIZE = 16;
        public readonly static byte[] Head = { 0x55, 0xaa };

        PackageMaker maker = new PackageMaker(PACKAGE_SIZE); 

        public Action<byte[]> OnPackageArrive;
        /// <summary>
        /// 封包大小为固定长度PACKAGE_SIZE
        /// </summary>
        public void SendPackage(byte[] buff,int offset,bool broadcast = false)
        {
            SendMessage(buff, offset, PACKAGE_SIZE, broadcast);
        }

        public virtual void Update()
        {
            maker.SendToRead(ReadMessage);
            byte [] b;
            while((b = maker.GetPackage()) != null)
            {
                OnPackageArrive(b);
            }
        }
    }
    /// <summary>
    /// 将数据打包Package的主机，可以不用
    /// </summary>
    public class WLPackageHostDev : WLHostDev
    {
        public const int PACKAGE_SIZE = 16;

        private PackageMaker maker = new PackageMaker(PACKAGE_SIZE);

        public Action<byte[]> OnPackageArrive;

        /// <summary>
        /// 封包大小为固定长度PACKAGE_SIZE
        /// </summary>
        public void SendPackage(byte[] buff, int offset/*, bool broadcast = false*/)
        {
            SendMessage(buff, offset, PACKAGE_SIZE, true);//作为Host，只能广播发送，协议中没有地址
        }


        public virtual void Update()
        {
            maker.SendToRead(ReadMessage);
            byte[] b;
            while ((b = maker.GetPackage()) != null)
            {
                OnPackageArrive(b);
            }
        }
    }

    public class PackageMaker
    {

        private readonly byte[] Head = WLPackageDev.Head;

        private int PackageSize;

        public delegate int ReadBytes(byte[] buff, int offset, int len);

        private Queue<byte> packages = new Queue<byte>();

        private int matchedHead;
        private int remainBody;

        public PackageMaker(int packageSize)
        {
            PackageSize = packageSize;
            matchedHead = 0;
        }

        /// <summary>
        /// 送入字节流
        /// </summary>
        /// <param name="readAction">参数依次为buff,offset,length</param>
        public void SendToRead(ReadBytes readAction)
        {
            int remain;
            byte[] buff = new byte[PackageSize];
            while((remain = readAction(buff, 0, PackageSize)) != 0)
            {
                for(int i = 0; i < remain; i++)
                {
                    if(remainBody > 0)
                    {
                        remainBody--;
                        packages.Enqueue(buff[i]);
                    }
                    else
                    {
                        if (matchedHead < Head.Count() && buff[i] == Head[matchedHead])
                        {
                            matchedHead++;//匹配到下一个满足的头部
                        }
                        if(matchedHead == Head.Count())
                        {
                            matchedHead = 0;//prepair for next
                            remainBody = PackageSize - Head.Count();//匹配到完整的头部
                            foreach (var b in Head)
                                packages.Enqueue(b);
                        }
                    }
                }
            }
        }

        public byte[] GetPackage()
        {
            if(packages.Count() >= PackageSize)
            {
                byte[] r = new byte[PackageSize];
                for(int i = 0; i < PackageSize; i++)
                {
                    r[i] = packages.Dequeue();
                }
                return r;
            }
            else
            {
                return null;
            }
        }
    }


}
