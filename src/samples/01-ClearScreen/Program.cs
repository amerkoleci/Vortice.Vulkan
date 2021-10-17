using System;
using Vortice.ShaderCompiler;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace DrawTriangle
{
    public static unsafe partial class Program
    {

        public static void Main()
        {
            try
            {
                var testApp = new TestApp();
                testApp.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
