using NotepadMon;
using Drk.AspNetCore.MinimalApiKit;
using Microsoft.AspNetCore.Mvc;

// https://blog.darkthread.net/blog/9952/
using Mutex m = new Mutex(false, "Global\\{9BE6C0F7-13F3-47BA-8B91-FB6A50BE09C6}");
if (!m.WaitOne(0, false)) return;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<NotepadMonitor>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/logs", () => string.Join("\n", NotepadMonitor.Logs));

var appIcoResName = typeof(Program).Assembly.GetManifestResourceNames().Single(o => o.EndsWith("App.ico"));
app.RunWithNotifyIcon(new NotifyIconOptions{
    IconStream = typeof(Program).Assembly.GetManifestResourceStream(appIcoResName)!,
    ToolTip = "Notepad Monitor",
    MenuItems = {
        NotifyIconOptions.CreateLaunchBrowserMenuItem("Check logs", (url) => url + "/logs"),
        NotifyIconOptions.CreateMenuSeparator(),
        NotifyIconOptions.CreateActionMenuItem("Lauch Notepad", (state) => {
            System.Diagnostics.Process.Start("notepad.exe");
        })
    }
});