using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NotepadMon
{
    //https://blog.darkthread.net/blog/aspnet-core-background-task/
    public class NotepadMonitor : IHostedService, IDisposable
    {
        static System.Threading.Timer _timer = null!;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new System.Threading.Timer(DoWork, null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        bool running = false;
        int[] lastPids = null!;
        public static List<string> Logs = new List<string>();
        public void DoWork(object? state)
        {
            if (running) return; //no reentrancy
            running = true;
            var notepadPids = Process.GetProcessesByName("Notepad")
                .Select(p => p.Id).ToArray();
            //find new processes
            if (lastPids == null) lastPids = notepadPids;
            if (notepadPids.Except(lastPids).Any())
            {
                Task.Factory.StartNew(async () =>
                {
                    var msg = $"new Notepad started!";
                    Logs.Add($"{DateTime.Now:HH:mm:ss} {msg}");
                    await Task.Delay(500);
                    System.Windows.Forms.MessageBox.Show(msg, "Notepad Alert", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information, 
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                });
            }
            lastPids = notepadPids;
            running = false;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}