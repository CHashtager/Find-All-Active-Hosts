using System.Diagnostics;
using System.Net.NetworkInformation;

var activeHosts = 0;
var lockObj = new object();

var countdown = new CountdownEvent(1);
var sw = new Stopwatch();
sw.Start();

Console.WriteLine("Enter base range IP ( ex. 192.168.1. ) :");
var ipBase = Console.ReadLine();
Console.WriteLine("------------------------");

for (var i = 1; i < 255; i++)
{
    var ip = ipBase + i;
    var p = new Ping();
    p.PingCompleted += PingCompleted;
    countdown.AddCount();
    p.SendAsync(ip, 100, ip);
}

countdown.Signal();
countdown.Wait();
sw.Stop();
Console.WriteLine("Took {0} milliseconds. {1} hosts active.", sw.ElapsedMilliseconds, activeHosts);
Console.WriteLine("------------------------");
// Console.ReadLine();


void PingCompleted(object sender, PingCompletedEventArgs e)
{
    var ip = (string)e.UserState!;
    switch (e.Reply)
    {
        case { Status: IPStatus.Success }:
        {
            Console.WriteLine("{0} is up: ({1} ms)", ip, e.Reply.RoundtripTime);
            lock (lockObj)
            {
                activeHosts++;
            }

            break;
        }
        case null:
            Console.WriteLine("Pinging {0} failed. (Null Reply object?)", ip);
            break;
    }

    countdown.Signal();
}