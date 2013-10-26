using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace StreamMosaic {
    public partial class MainWindow : Form {
        class OBSLauncher {
            public readonly MainWindow Form;
            private readonly int RowCount;
            private readonly string[] SourceURLs;

            public OBSLauncher (MainWindow form) {
                Form = form;
                RowCount = Form.Sources.RowCount;
                SourceURLs = (from row in Form.Sources.Rows.Cast<DataGridViewRow>() select Convert.ToString(row.Cells[0].Value)).ToArray();
            }

            public void Go (Action onComplete) {
                ThreadPool.QueueUserWorkItem(Worker, onComplete);
            }

            private void Worker (object state) {
                var servers = new List<NamedPipeServerStream>();
                var processes = new List<Process>();
                var resolvedURLs = new string[SourceURLs.Length];

                // First we launch a livestreamer instance per URL to get the resolved URL so we can feed it into OBS

                for (var i = 0; i < RowCount; i++) {
                    var url = SourceURLs[i];
                    if (String.IsNullOrWhiteSpace(url))
                        continue;

                    string resolvedUrl = null;

                    var npss = new NamedPipeServerStream("Stream Mosaic", PipeDirection.InOut, RowCount, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                    servers.Add(npss);
                    var connection = npss.BeginWaitForConnection(null, null);

                    var psi = new ProcessStartInfo(
                        @"C:\Program Files (x86)\LiveStreamer\livestreamer.exe",
                        String.Format(
                            "--player-continuous-http --player StreamMosaic.exe \"{0}\" best",
                            url
                        )
                    );
                    psi.CreateNoWindow = true;
                    psi.WindowStyle = ProcessWindowStyle.Minimized;
                    processes.Add(Process.Start(psi));

                    connection.AsyncWaitHandle.WaitOne();
                    npss.EndWaitForConnection(connection);

                    var bytes = new byte[10240];
                    var numBytes = npss.Read(bytes, 0, bytes.Length);
                    resolvedURLs[i] = resolvedUrl = Encoding.UTF8.GetString(bytes, 0, numBytes);

                    // We keep the pipe open so the child process survives, which keeps livestreamer's proxy alive.

                    var ar = Form.BeginInvoke(
                        (Action<int>)((index) => {
                            Form.Sources.Rows[index].Cells[1].Value = resolvedUrl;
                        }), i
                    );

                    ar.AsyncWaitHandle.WaitOne();
                }

                // Now we update the sources file...

                var sourcesPath = @"..\OBS\scenes.xconfig";
                var sourcesBackupPath = sourcesPath + ".old";
                var existingSources = File.ReadAllText(sourcesPath);

                var re = new Regex(
                    @"(Stream (?'streamIndex'[0-9]*) :).*?\n( *)playlist( *):( *)(?'playlist'[^\n]*)",
                    RegexOptions.ExplicitCapture | RegexOptions.Singleline
                );
                var newSources = re.Replace(
                    existingSources,
                    (m) => {
                        var index = Convert.ToInt32(m.Groups["streamIndex"].Value) - 1;
                        var a = m.Groups["playlist"].Index - m.Index;
                        var b = a + m.Groups["playlist"].Length;

                        var result = m.Value.Substring(0, a) +
                            (resolvedURLs[index] ?? "none") +
                            m.Value.Substring(b);

                        return result;
                    }
                );

                if (File.Exists(sourcesBackupPath))
                    File.Delete(sourcesBackupPath);

                File.Move(sourcesPath, sourcesBackupPath);
                File.WriteAllText(sourcesPath, newSources);

                // Now launch OBS and wait for it to exit.

                using (var obsProcess = Process.Start(@"..\OBS\OBS.exe", "-portable"))
                    obsProcess.WaitForExit();

                // Close down pipes to kill child processes so that livestreamer closes its proxies
                foreach (var npss in servers) {
                    try {
                        npss.Write(new byte[1] { 0 }, 0, 1);
                        npss.Flush();
                    } catch {
                    }
                    try {
                        npss.Dispose();
                    } catch {
                    }
                }

                foreach (var process in processes)
                    process.Dispose();

                var onComplete = (Action)state;
                Form.BeginInvoke(onComplete);
            }
        }

        public MainWindow () {
            InitializeComponent();

            Sources.RowCount = 4;
        }

        private void StartOBS_Click (object sender, EventArgs e) {
            UseWaitCursor = true;
            Sources.Enabled = StartOBS.Enabled = false;

            (new OBSLauncher(this)).Go(ResolveComplete);
        }

        private void ResolveComplete () {
            UseWaitCursor = false;
            Sources.Enabled = StartOBS.Enabled = true;
        }
    }
}
