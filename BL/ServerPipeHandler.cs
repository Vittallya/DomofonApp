using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BL.Pipes;

namespace BL
{
    public class ServerPipeHandler: IDisposable
    {
        int pipeHandle;

        Thread t;

        bool sendCalled;
        private bool _cont;

        public void Dispose()
        {
            _cont = false;
            t?.Abort();
            if (pipeHandle != -1)
                Import.CloseHandle(pipeHandle);
        }

        public void Init()
        {
            pipeHandle = Import.CreateNamedPipe(
                "\\\\.\\pipe\\ParkingFinePipe",
                Types.PIPE_ACCESS_DUPLEX,
                Types.PIPE_TYPE_BYTE,
                Types.PIPE_UNLIMITED_INSTANCES,
                0,
                1024,
                Types.NMPWAIT_WAIT_FOREVER,
                (uint)0);
            _cont = true;
            t = new Thread(Sender);
            t.Start();
        }

        byte[] _message;

        public void Send(string message)
        {
            temp = count;
            _message = Encoding.Unicode.GetBytes(message);
        }

        int temp = 0;

        int count;

        void Sender()
        {

            while (_cont)
            {
                

                if (Import.ConnectNamedPipe(pipeHandle, 0))
                {

                    byte[] buffRead = new byte[1];
                    uint realBytesReaded = 0; 

                    if (Import.ReadFile(pipeHandle, buffRead, (uint)buffRead.Length, ref realBytesReaded, 0) && realBytesReaded != 0)
                    {
                        count++;
                    }

                    byte[] buffWrite = new byte[] { 0 };
                    uint realBytesWrited = 0;

                    if (temp > 0)
                    {
                        buffWrite = _message;
                        temp--;
                    }


                    Import.WriteFile(pipeHandle, buffWrite, (uint)buffWrite.Length, ref realBytesWrited, 0);


                    Import.DisconnectNamedPipe(pipeHandle);                             // отключаемся от канала клиента 

                    // приостанавливаем работу потока перед тем, как приcтупить к обслуживанию очередного клиента
                }
                Thread.Sleep(100);
            }
        }

    }
}
