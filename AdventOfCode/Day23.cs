using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace AdventOfCode
{
    internal class Day23
    {
        static async Task Main(string[] args)
        {
            var fileText = File.ReadAllText(@"Datasets\day23.txt");
            var strCode = fileText.Split(",").ToArray();
            //var screen = new List<List<char>>();
            var result = 0;
            //var program = new Program(strCode);
            //var obj = new object();
            var networkList = Enumerable.Range(0, 50).ToList();
            //var networkDictionary = new Dictionary<int, Program>();
            var network = new Network();
            foreach (var n in networkList)
            {
                var prog = new Program(n, strCode);
                network.PackageInReady += prog.OnProgramInput;
                prog.PackageOutReady += network.HandleNewPackage;
                prog.StatusChanged += network.OnStatusChanged;
                prog.SendInput(new List<long>() { n });

                network.NetworkDictionary.Add(n, prog);
            }

            await network.Run();

            while (true)
            {

            }
            //while (result != 1)
            //{
            //    lock (obj)
            //    {
            //        foreach (var kpv in networkDictionary)
            //        {
            //            var key = kpv.Key;
            //            var prog = kpv.Value;
            //            if (prog.IsReady)
            //            {
            //                prog.IsReady = false;
            //                var n = int.Parse(prog.Outputs.First());
            //                var package = prog.Outputs.Skip(1).Take(2).Select(int.Parse).ToList();
            //                prog.Outputs = new List<string>();
            //                if (package.First() == 255)
            //                {
            //                    Console.WriteLine($"Address {package.First()} Value {package.Last()}");
            //                }
            //                networkDictionary[n].SendInput(package);
            //                var t = new Thread(new ThreadStart(() =>
            //                 {
            //                     Console.WriteLine($"From #{key} sent to #{n} -> Package X:{package.First()} Y: {package.Last()}");
            //                     prog.RunProgram();
            //                 }));
            //                t.Start();
            //            }

            //        }
            //    }


            //    //foreach (var output in program.Outputs)
            //    //{
            //    //    Console.WriteLine(program.Outputs.Last());
            //    //    program.SendInput(int.Parse(output));
            //    //}


            //}


            //foreach (var output in program.Outputs)
            //{
            //    Console.Write(Convert.ToChar(System.Convert.ToInt32(output)));
            //}

        }

        public class Network
        {
            public event EventHandler<InputEventArgs> PackageInReady;
            public Dictionary<int, Program> NetworkDictionary = new Dictionary<int, Program>();
            public Dictionary<int, bool> NetworkStatus = new Dictionary<int, bool>();
            public List<long> Package = new List<long>();
            public List<long> PreviousPackage = new List<long>();
            private object _lock = new object();
            private int id = 0;
            private int previousId = 0;
            private Timer timer;
            public void HandleNewPackage(object sender, OutputEventArgs e)
            {
                lock (_lock)
                {
                    var address = e.Address;
                    var package = e.OutputPackage;

                    var newPackage = new InputEventArgs(address, package);
                    Console.WriteLine($"{id++} Sent from #{e.From} to #{address} -> Package X:{package.First()} Y: {package.Last()}");
                    if (address != 255)
                    {
                        PackageInReady?.Invoke(null, newPackage);
                    }
                    else
                    {
                        Package = package;
                        //newPackage = new InputEventArgs(0, Package);
                        //Console.WriteLine($"Sent from #{255} to #{address} -> Package X:{Package.First()} Y: {Package.Last()}");
                        //PackageInReady?.Invoke(null, newPackage);
                        ////PackageInReady?.Invoke(null, newPackage);
                    }
                }
            }

            public async Task Run()
            {
                //foreach (var n in NetworkDictionary.Keys)
                //{
                //    NetworkStatus.Add(n, false);
                //}

                lock (_lock)
                {
                    foreach (var n in NetworkDictionary)
                    {
                        var id = n.Key;
                        var prog = n.Value;
                        new Thread((async () => await prog.RunProgram())).Start();

                    }

                    timer = new Timer(state => OnTimerTick(null, null), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

                }




            }

            public void OnStatusChanged(object sender, StatusEventArgs e)
            {
                lock (_lock)
                {
                    var address = e.Address;
                    if (!NetworkStatus.ContainsKey(address)) NetworkStatus.Add(address, e.Status);
                    NetworkStatus[address] = e.Status;
                }
            }

            public void OnTimerTick(object sender, EventArgs eventArgs)
            {
                lock (_lock)
                {
                    var status = NetworkStatus.Values.All(x => x);
                    var zeroStatus = NetworkStatus[0];
                    if (status && Package.Count > 0)
                    {
                        var newPackage = new InputEventArgs(0, Package);
                        Console.WriteLine(
                            $"{id++} Sent from #{255} to #{newPackage.Address} -> Package X:{Package.First()} Y: {Package.Last()}");
                        PackageInReady?.Invoke(null, newPackage);

                        if (PreviousPackage.Count > 0 && PreviousPackage.Last() == Package.Last())
                            Console.WriteLine(
                                $"Previous id{previousId} Y #{PreviousPackage.Last()} -> id{id - 1} Package Y: {Package.Last()}");
                        PreviousPackage = Package;
                        previousId = id - 1;
                    }
                }
            }
        }

        private static IEnumerable<T> Flatten<T>(T[][] map)
        {
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map[0].Length; col++)
                {
                    yield return map[row][col];
                }
            }
        }

        private static IEnumerable<T> Flatten<T>(List<List<T>> map)
        {
            for (int row = 0; row < map.Count; row++)
            {
                for (int col = 0; col < map[0].Count; col++)
                {
                    yield return map[row][col];
                }
            }
        }

        private static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        private static double ReverseDouble(double src)
        {
            double dst = 0;
            int decPoint = 0;

            while (src - (long)src > 0)
            {
                src = src * 10;
                decPoint++;
            }

            int totalDigits = 0;

            while (src > 0)
            {
                int d = (int)src % 10;
                dst = dst * 10 + d;
                src = (long)(src / 10);
                totalDigits++;
            }

            if (decPoint > 0)
            {
                int reversedDecPoint = totalDigits - decPoint;
                for (int i = 0; i < reversedDecPoint; i++) dst = dst / 10;
            }

            return dst;
        }

        public class Program
        {
            private object _lock = new object();
            private readonly int _address;
            private List<long> inputQueue = new List<long>();
            private int _relativeBase;
            private int _pointerPosition;
            private readonly string[] _memory;
            public bool IsReady = false;

            public event EventHandler<StatusEventArgs> StatusChanged;
            public event EventHandler PackageInReady;
            public event EventHandler<OutputEventArgs> PackageOutReady;

            public List<string> Outputs = new List<string>();

            public Program(int address, string[] strCode)
            {
                _address = address;

                var memory = Enumerable.Range(0, 500000).Select(x => "0").ToArray();
                _memory = strCode.Concat(memory).ToArray();
            }

            public void SendInput(List<long> input)
            {
                inputQueue.AddRange(input);
                OnStatusChanged(new StatusEventArgs(_address, inputQueue.Count == 0));
            }

            public virtual void OnStatusChanged(StatusEventArgs e)
            {
                EventHandler<StatusEventArgs> handler = StatusChanged;
                handler?.Invoke(this, e);
            }

            public virtual void OnProgramOutput(OutputEventArgs e)
            {
                EventHandler<OutputEventArgs> handler = PackageOutReady;
                handler?.Invoke(this, e);
            }

            public async void OnProgramInput(object sender, InputEventArgs e)
            {
                lock (_lock)
                {
                    if (e.Address != _address) return;

                    //Console.WriteLine($"Received by #{_address} -> Package X:{e.InputPackage.First()} Y:{e.InputPackage.Last()}");
                    SendInput(e.InputPackage);
                    //await RunProgram();
                }
            }

            public async Task RunProgram()
            {
                //var memory = Enumerable.Range(0, 50000).Select(x => "0").ToArray();
                //memory = strCode.Concat(memory).ToArray();
                for (var pointerPosition = _pointerPosition; pointerPosition < _memory.Length; pointerPosition++)
                {
                    var codeDouble = double.Parse(_memory[pointerPosition]);
                    var codeStr = ReverseDouble(codeDouble).ToString();
                    var modeA = codeStr.Length > 4 ? codeStr.ElementAt(4).ToString() : "0";
                    var modeB = codeStr.Length > 3 ? codeStr.ElementAt(3).ToString() : "0";
                    var modeC = codeStr.Length > 2 ? codeStr.ElementAt(2).ToString() : "0";
                    var code = codeStr.Length > 1
                        ? int.Parse(codeStr[1].ToString() + codeStr[0].ToString())
                        : int.Parse(codeStr);

                    var argument1Pointer = pointerPosition + 1;
                    var relativeArgument1Pointer =
                        modeC == "2" ? int.Parse(_memory[argument1Pointer]) + _relativeBase : 0;
                    var pointer1Position = modeC == "0" ? int.Parse(_memory[argument1Pointer]) : 0;

                    var argument2Pointer = pointerPosition + 2;
                    var relativeArgument2Pointer =
                        modeB == "2" ? int.Parse(_memory[argument2Pointer]) + _relativeBase : 0;
                    var pointer2Position = modeB == "0" ? int.Parse(_memory[argument2Pointer]) : 0;

                    var resultPointer = pointerPosition + 3;
                    var relativeResultPointer = modeA == "2" ? int.Parse(_memory[resultPointer]) + _relativeBase : 0;
                    var resultPointerPosition = modeA == "0" && code != 4 ? int.Parse(_memory[resultPointer]) : 0;

                    if (code == 99)
                    {
                        _pointerPosition = pointerPosition;
                        break;
                    }

                    var argument1 = modeC switch
                    {
                        "1" => _memory[argument1Pointer],
                        "2" => _memory[relativeArgument1Pointer],
                        _ => _memory[pointer1Position]
                    };
                    var argument2 = modeB switch
                    {
                        "1" => _memory[argument2Pointer],
                        "2" => _memory[relativeArgument2Pointer],
                        _ => _memory[pointer2Position]
                    };

                    var resultPosition = modeA switch
                    {
                        "1" => resultPointer,
                        "2" => relativeResultPointer,
                        _ => resultPointerPosition
                    };

                    if (code == 1)
                    {
                        _memory[resultPosition] = (double.Parse(argument1) + double.Parse(argument2)).ToString();
                        pointerPosition += 3;
                    }
                    else if (code == 2)
                    {
                        _memory[resultPosition] = (double.Parse(argument1) * double.Parse(argument2)).ToString();
                        pointerPosition += 3;
                    }
                    else if (code == 3)
                    {
                        //if (waitingForInput)
                        //{
                        //    _pointerPosition = pointerPosition;
                        //    waitingForInput = false;
                        //    return 2;
                        //}

                        long input = -1;
                        if (inputQueue.Count > 0)
                        {
                            input = inputQueue[0];
                            inputQueue.RemoveAt(0);
                        }

                        OnStatusChanged(new StatusEventArgs(_address, inputQueue.Count == 0));

                        var inputPointer = modeC != "2" ? pointer1Position : relativeArgument1Pointer;
                        _memory[inputPointer] = input.ToString();
                        //Console.WriteLine($"Input: {input}, Input Taken: {memory[inputPointer]}");
                        pointerPosition++;
                        //}
                        //else
                        //{
                        //    _pointerPosition = pointerPosition;
                        //    return 2;
                        //}

                        //var inputPointer = modeC != "2" ? pointer1Position : relativeArgument1Pointer;
                        //_memory[inputPointer] = input.ToString();
                        ////Console.WriteLine($"Input: {input}, Input Taken: {memory[inputPointer]}");
                        //pointerPosition++;
                    }
                    else if (code == 4)
                    {
                        //Console.WriteLine($"Output: " + argument1);
                        Outputs.Add(argument1);
                        pointerPosition++;
                        _pointerPosition = pointerPosition + 1;
                        if (Outputs.Count > 2)
                        {
                            OnProgramOutput(new OutputEventArgs(_address, Outputs));
                            Outputs = new List<string>();
                            //return 0;
                        }
                    }
                    else if (code == 5)
                    {
                        if (argument1 != "0")
                        {
                            pointerPosition = int.Parse(argument2);
                            pointerPosition--;
                        }
                        else
                        {
                            pointerPosition += 2;
                        }
                    }
                    else if (code == 6)
                    {
                        if (argument1 == "0")
                        {
                            pointerPosition = int.Parse(argument2);
                            pointerPosition--;
                        }
                        else
                        {
                            pointerPosition += 2;
                        }
                    }
                    else if (code == 7)
                    {
                        if (double.Parse(argument1) < double.Parse(argument2))
                        {
                            _memory[resultPosition] = "1";
                        }
                        else
                        {
                            _memory[resultPosition] = "0";
                        }

                        pointerPosition += 3;
                    }
                    else if (code == 8)
                    {
                        if (argument1 == argument2)
                        {
                            _memory[resultPosition] = "1";
                        }
                        else
                        {
                            _memory[resultPosition] = "0";
                        }

                        pointerPosition += 3;
                    }
                    else if (code == 9)
                    {
                        _relativeBase += int.Parse(argument1);
                        pointerPosition++;
                    }
                    else if (code == 99)
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                //return 1;
            }

        }


    }

    internal class OutputEventArgs : EventArgs
    {
        public long From;
        public long Address;
        public List<long> OutputPackage;

        public OutputEventArgs(int from, List<string> outputsString)
        {
            From = from;
            var intList = outputsString.Select(long.Parse).ToList();
            Address = intList.First();
            OutputPackage = intList.Skip(1).Take(2).ToList();
        }
    }

    internal class InputEventArgs : EventArgs
    {
        public long Address;
        public List<long> InputPackage;

        public InputEventArgs(long address, List<long> inputPackage)
        {
            Address = address;
            InputPackage = inputPackage;
        }
    }

    internal class StatusEventArgs : EventArgs
    {
        public int Address;
        public bool Status;

        public StatusEventArgs(int address, bool status)
        {
            Address = address;
            Status = status;
        }
    }
}