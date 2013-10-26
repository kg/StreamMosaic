using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace StreamMosaic {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main () {
            try {
                using (var npcs = new NamedPipeClientStream("Stream Mosaic")) {
                    try {
                        npcs.Connect(500);

                        var url = Environment.CommandLine.Substring(Environment.CommandLine.IndexOf(' ')).Trim();

                        var textBytes = Encoding.UTF8.GetBytes(url);
                        npcs.Write(textBytes, 0, textBytes.Length);
                        npcs.Flush();

                        var responseBytes = new byte[1];
                        npcs.Read(responseBytes, 0, 1);
                    } catch (TimeoutException) {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new MainWindow());
                    }
                }
            } catch (Exception exc) {
                MessageBox.Show(exc.ToString());
            }
        }
    }
}
