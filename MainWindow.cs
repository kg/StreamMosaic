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
        public class StreamResolver {
            public readonly MainWindow Form;
            private readonly int RowIndex;

            private readonly object Mutex = new object();

            private Process LivestreamerProcess;
            private NamedPipeServerStream ServerPipe;

            public StreamResolver (MainWindow form, int rowIndex) {
                Form = form;
                RowIndex = rowIndex;
            }

            public void Resolve () {
                var sourceUrl = Form.Sources.Rows[RowIndex].Cells[0].Value;

                ThreadPool.QueueUserWorkItem(Worker, sourceUrl);
            }

            public void Dispose () {
                if (LivestreamerProcess != null) {
                    try {
                        LivestreamerProcess.Kill();
                        LivestreamerProcess.Dispose();
                    } catch {
                    }
                    LivestreamerProcess = null;
                }

                if (ServerPipe != null) {
                    ServerPipe.Dispose();
                    ServerPipe = null;
                }
            }

            private void Worker (object state) {
                try {
                    var sourceUrl = (string)state;

                    lock (Mutex) {
                        Dispose();

                        string resolvedUrl = null;

                        ServerPipe = new NamedPipeServerStream("Stream Mosaic", PipeDirection.InOut, 64, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                        var connection = ServerPipe.BeginWaitForConnection(null, null);

                        var psi = new ProcessStartInfo(
                            Form.LivestreamerPath,
                            String.Format(
                                "--player-continuous-http --player StreamMosaic.exe \"{0}\" best",
                                sourceUrl
                            )
                        );
                        psi.UseShellExecute = false;
                        psi.CreateNoWindow = true;
                        psi.WindowStyle = ProcessWindowStyle.Hidden;
                        /*
                        psi.RedirectStandardError = true;
                        psi.RedirectStandardOutput = true;
                         */
                        LivestreamerProcess = Process.Start(psi);

                        connection.AsyncWaitHandle.WaitOne();
                        ServerPipe.EndWaitForConnection(connection);

                        var bytes = new byte[10240];
                        var numBytes = ServerPipe.Read(bytes, 0, bytes.Length);
                        resolvedUrl = Encoding.UTF8.GetString(bytes, 0, numBytes);

                        // We keep the pipe open so the child process survives, which keeps livestreamer's proxy alive.

                        var ar = Form.BeginInvoke(
                            (Action<int>)((index) => {
                                Form.Sources.Rows[index].Cells[1].Value = resolvedUrl;
                            }), RowIndex
                        );

                        ar.AsyncWaitHandle.WaitOne();
                    }
                } catch (Exception exc) {
                    MessageBox.Show(exc.ToString());
                }
            }
        }

        public readonly Dictionary<int, StreamResolver> RowResolvers = new Dictionary<int, StreamResolver>();

        public readonly string LivestreamerPath = null;

        public MainWindow () {
            InitializeComponent();

            var paths = new[] {
                @"C:\Program Files (x86)\LiveStreamer\livestreamer.exe",
                @"C:\Program Files\LiveStreamer\livestreamer.exe",
                @"livestreamer.exe"
            };

            foreach (var path in paths) {
                if (File.Exists(path))
                    LivestreamerPath = path;
            }

            if (LivestreamerPath == null)
                MessageBox.Show("Cannot find Livestreamer.exe");

            Sources_RowsAdded(Sources, new DataGridViewRowsAddedEventArgs(0, 0));
        }

        private void Sources_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) {
            for (var i = 0; i < Sources.RowCount; i++)
                Sources.Rows[i].Cells[2].Value = "Resolve";
        }

        private StreamResolver GetRowResolver (int rowIndex) {
            StreamResolver result;

            if (!RowResolvers.TryGetValue(rowIndex, out result))
                result = RowResolvers[rowIndex] = new StreamResolver(this, rowIndex);

            return result;
        }

        private void Sources_CellContentClick (object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex != 2)
                return;

            var resolver = GetRowResolver(e.RowIndex);
            if (resolver == null)
                return;

            var buttonCell = (DataGridViewButtonCell)Sources.Rows[e.RowIndex].Cells[e.ColumnIndex];
            resolver.Resolve();
        }

        private void MainWindow_FormClosing (object sender, FormClosingEventArgs e) {
            foreach (var rr in RowResolvers)
                rr.Value.Dispose();
        }
    }
}
