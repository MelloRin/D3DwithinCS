using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;

namespace CSd3d.Lib
{
    class D3D_handler : IDisposable
    {
        private MainForm mainForm = null;
        private Device device = null;

        private bool b_up = true;
        private int B = 1;

        public bool InitallizeApplication(MainForm mainForm)
        {
            this.mainForm = mainForm;

            PresentParameters _pp = new PresentParameters();
            _pp.Windowed = Boolean.Parse(PublicData_manager.settings.get_setting("windowded"));
            _pp.SwapEffect = SwapEffect.Discard;

            try //hw렌더링
            {
                this.device = new Device(0, DeviceType.Hardware, mainForm.Handle,
                    CreateFlags.HardwareVertexProcessing, _pp);
                PublicData_manager.device_created = true;
                Console.WriteLine("HW렌더링");

            }
            catch (DirectXException)
            {
                try
                {
                    this.device = new Device(0, DeviceType.Reference, mainForm.Handle,
                    CreateFlags.SoftwareVertexProcessing, _pp);
                    PublicData_manager.device_created = true;
                    Console.WriteLine("SW렌더링");
                }
                catch (DirectXException) { }
            }
            return true;
        }

        public void loop()
        {
            //Console.WriteLine("메인루프");

            if (PublicData_manager.device_created)
            {
                try
                {
                    background_Render();
                    //Console.WriteLine("{0},{1},{2},{3}", color.A, color.R, color.G, color.B);

                    device.BeginScene();
                    device.EndScene();
                    device.Present();
                }
                catch (DirectXException)
                {
                    Console.WriteLine("D3D 에러");
                }

            }
        }
        private void background_Render()
        {
            Color color = Color.FromArgb(0, 0, 0, B);

            //device.Clear(ClearFlags.Target, Color.White , 1.0f, 0);
            if (b_up)
            {
                device.Clear(ClearFlags.Target, color, 1.0f, 0);

                if (B < 255)
                    B += 2;
                else
                {
                    B = 255;
                    b_up = false;
                }
            }
            else
            {
                device.Clear(ClearFlags.Target, color, 1.0f, 0);

                if (B >= 2)
                    B -= 2;
                else
                {
                    B = 1;
                    b_up = true;
                }
            }
        }

        public void Dispose()
        {
            if (device != null)
                device = null;
        }
    }
}