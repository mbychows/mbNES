using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace mbNES
{
    internal class BusTest
    {
        // Create new bus for testing purposes
        Bus testBus = new Bus();
        public BusTest()
        {
             
                       
            
        }
        // Crappy test because it relies on ReadBus()
        public void WriteTest(ushort address, byte data)
        {
            Console.WriteLine("Bus Write Test - writing " + data.ToString("X") + " to address " + address.ToString("X"));
            testBus.WriteBus(address, data);
            Console.Write("Value at address " + address.ToString("X") + ": ");
            Console.WriteLine(testBus.ReadBus(address).ToString("X"));
        }

        public void ReadTest(ushort address) 
        {
            Console.WriteLine("Bus Read Test - reading from address " + address.ToString("X"));
            testBus.ReadBus(address);
            Console.Write("Value at address " + address.ToString("X") + ": ");
            Console.WriteLine(testBus.ReadBus(address).ToString("X"));
        }
    }
}
