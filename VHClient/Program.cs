using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHClient
{
    class Program
    {
        private static void Pipe_USART_Write(int ch)
        {
            Console.WriteLine(ch);
        }
        private static int Pipe_USART_Read()
        {
            return 2;
        }

        static void Main(string[] args)
        {
            var argMaps = GetArgsMap(args);
            //载入相关函数
            //Hacked
            string dllPath = "C:\\Users\\q6027\\Desktop\\testgcc\\a.bin";
            //string dllPath = argMaps["f"];
            try
            {
                VHDllLoader loader = new VHDllLoader(dllPath);
                //设置回掉函数，用来和虚拟环境交互,必须使用CdeclFunc(有返回值)或者CdeclAction(无返回值)，不能使用普通的Func和Action
                //loader.SetupLinks(VHDllLoader.CallBacks.CB_USART_READ, new CdeclFuncInt(Pipe_USART_Read));
                //loader.SetupLinks(VHDllLoader.CallBacks.CB_USART_WRITE, new CdeclActionInt(Pipe_USART_Write));


                //这里已经在运行
                int r = loader.RunMain();

                //这里运行结束
                //do clean
                loader.Close();
                //output return value(useless)
                Console.WriteLine("dll " + dllPath + "run end.return value:" + r);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
            //Debug only
            Console.ReadKey();
        }

        //IO参数处理
        private static Dictionary<String, String> GetArgsMap(string[] args)
        {
            string laststr = "";
            bool doing = false;
            Dictionary<String, String> dictionary = new Dictionary<string, string>();
            foreach (var s in args)
            {
                if (doing)
                {
                    doing = false;
                    dictionary[laststr] = s;
                }
                else if (s.StartsWith("-"))
                {
                    int i = 0;
                    while (s[i] == '-')
                        i++;
                    laststr = s.Substring(i);
                    doing = (laststr != "");
                }
            }
            return dictionary;
        }
    }
}
