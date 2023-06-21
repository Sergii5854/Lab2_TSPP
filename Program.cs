using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace DotNet.Docker
{
    class Program
    {
        static void Main(string[] args)
        {
            int MAX_STOPWACHES = int.Parse(Environment.GetEnvironmentVariable("MAX_STOPWATCHES").Trim());
            int currentUsage = 0;
            Stopwatch[] timers = new Stopwatch[MAX_STOPWACHES];
            string pathSource = @"/interface/request.txt";
            string pathNew = @"/interface/response.txt";
            while (true)
            {
                string response = "";
                try
                {
                    string[] requestText = File.ReadAllLines(pathSource);
                    string[] command = requestText[0].Trim().Split();
                    switch (command[0])
                    {
                        case "start":
                            if (currentUsage < MAX_STOPWACHES)
                            {
                                if (timers.Length == MAX_STOPWACHES)
                                {
                                    Array.Resize(ref timers, MAX_STOPWACHES + 1);
                                }
                                timers[currentUsage] = new Stopwatch();
                                timers[currentUsage].Start();
                                response = $"{currentUsage + 1}";
                                currentUsage++;
                            }
                            else { response = "Exception: max value of timers reached."; }
                            break;
                        case "stop":
                            int id;
                            try
                            {
                                id = int.Parse(command[1]);
                            }
                            catch (Exception)
                            {
                                response = "Exception: id contains symbols.";
                                break;
                            }
                            if (id <= currentUsage)
                            {
                                timers[id - 1].Stop();
                                response = $"{Convert.ToString(GetSeconds(timers[id - 1]))}";
                            }
                            else { response = "Exception: incorrect id."; }
                            break;
                        case "get":
                            int Id;
                            try
                            {
                                Id = int.Parse(command[1]);
                            }
                            catch (Exception)
                            {
                                response = "Exception: id contains symbols.";
                                break;
                            }
                            if (Id <= currentUsage)
                            {
                                response = $"{Convert.ToString(GetSeconds(timers[Id - 1]))}";
                            }
                            else { response = "Exception: incorrect id."; }
                            break;
                        default:
                            response = "Exception: incorrect command.";
                            break;
                    }
                    File.WriteAllText(pathNew, response);
                    Console.WriteLine(response);
                    File.Delete(pathSource);
                }
                catch (Exception)
                {

                }
                Thread.Sleep(3000);
            }
        }
        static long GetSeconds(Stopwatch s)
        {
            return (long)(Math.Floor(s.Elapsed.TotalSeconds));
        }
    }

}