//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Transactions;

//namespace AdventOfCode
//{
//    public class Processor
//    {
//        public Processor(long[] Program)
//        {
//            this.Program = Program.ToList();
//            Inputs = new Queue<long>();
//        }

//        public List<long> Program { get; set; }
//        private List<long> WorkingProgram { get; set; }
//        public Processor ProcessorToListenTo { get; set; }
//        private Queue<long> Inputs { get; set; }

//        public event EventHandler<OutputEventArgs> ProgramOutput;
//        public event EventHandler ProgramFinish;

//        public virtual void OnProgramOutput(OutputEventArgs e)
//        {
//            EventHandler<OutputEventArgs> handler = ProgramOutput;
//            handler?.Invoke(this, e);
//        }

//        public virtual void OnProgramFinish(EventArgs e)
//        {
//            EventHandler handler = ProgramFinish;
//            handler?.Invoke(this, e);
//        }

//        public void AddInput(long Input)
//        {
//            Inputs.Enqueue(Input);
//        }

//        public void ResetInputs()
//        {
//            Inputs.Clear();
//        }

//        public void ListenToProcessor(Processor p)
//        {
//            p.ProgramOutput += P_ProgramOutput;
//        }

//        private void P_ProgramOutput(object sender, OutputEventArgs e)
//        {
//            AddInput(e.OutputValue);
//        }


//        public void ProccessProgram()
//        {
//            WorkingProgram = new List<long>(Program);
//            int PC = 0;

//            while (true)
//            {
//                long instruction = WorkingProgram[PC];
//                Operation op = (Operation)(instruction % 100);

//                long[] opParams = GetParams(instruction, PC, op);

//                switch (op)
//                {
//                    case Operation.Add:
//                        WorkingProgram[(int)opParams[2]] = opParams[0] + opParams[1];
//                        PC += 4;
//                        break;

//                    case Operation.Multiply:
//                        WorkingProgram[(int)opParams[2]] = opParams[0] * opParams[1];
//                        PC += 4;
//                        break;

//                    case Operation.ReadInput:
//                        WorkingProgram[(int)opParams[1]] = opParams[0];
//                        PC += 2;
//                        break;

//                    case Operation.WriteOutput:
//                        var output = new OutputEventArgs
//                        {
//                            OutputValue = opParams[0]
//                        };
//                        OnProgramOutput(output);
//                        PC += 2;
//                        break;

//                    case Operation.JumpTrue:
//                        if (opParams[0] != 0)
//                        {
//                            PC = (int)opParams[1];
//                        }
//                        else
//                        {
//                            PC += 3;
//                        }
//                        break;

//                    case Operation.JumpFalse:
//                        if (opParams[0] == 0)
//                        {
//                            PC = (int)opParams[1];
//                        }
//                        else
//                        {
//                            PC += 3;
//                        }
//                        break;

//                    case Operation.LessThan:
//                        if (opParams[0] < opParams[1])
//                        {
//                            WorkingProgram[(int)opParams[2]] = 1;
//                        }
//                        else
//                        {
//                            WorkingProgram[(int)opParams[2]] = 0;
//                        }
//                        PC += 4;
//                        break;

//                    case Operation.TestEquals:
//                        if (opParams[0] == opParams[1])
//                        {
//                            WorkingProgram[(int)opParams[2]] = 1;
//                        }
//                        else
//                        {
//                            WorkingProgram[(int)opParams[2]] = 0;
//                        }
//                        PC += 4;
//                        break;

//                    case Operation.HALT:
//                        OnProgramFinish(new EventArgs());
//                        return;
//                    default:
//                        throw new Exception("Not a valid Opcode");
//                }
//            }
//        }

//        private long[] GetParams(long instruction, int PC, Operation op)
//        {
//            long[] res;
//            Mode[] modes = GetModes((int)(instruction / 100));
//            int immediate = -100;
//            switch (op)
//            {
//                case Operation.Add:
//                case Operation.Multiply:
//                case Operation.LessThan:
//                case Operation.TestEquals:
//                    res = new long[3]; //Let's just assume that any operation can take 3 params except reading input (must wait for input) and Halting
//                    for (int i = 0; i < 3; i++)
//                    {
//                        try
//                        {
//                            immediate = (int)WorkingProgram[PC + i + 1];
//                            if (modes[i] == Mode.Position && i != 2) //If it's the output location, we still need teh "immediate" value
//                            {

//                                res[i] = WorkingProgram[immediate];
//                            }
//                            else
//                            {
//                                res[i] = immediate;
//                            }
//                        }
//                        catch (IndexOutOfRangeException e)
//                        {
//                            res[i] = 0;
//                        }
//                    }
//                    break;
//                case Operation.WriteOutput:
//                    res = new long[1]; //Let's just assume that any operation can take 3 params except reading input (must wait for input) and Halting

//                    immediate = (int)WorkingProgram[PC + 1];
//                    if (modes[0] == Mode.Position)
//                    {

//                        res[0] = WorkingProgram[immediate];
//                    }
//                    else
//                    {
//                        res[0] = immediate;
//                    }

//                    break;
//                case Operation.JumpTrue:
//                case Operation.JumpFalse:
//                    res = new long[2];
//                    for (int i = 0; i < 2; i++)
//                    {
//                        try
//                        {
//                            immediate = (int)WorkingProgram[PC + i + 1];
//                            if (modes[i] == Mode.Position)
//                            {

//                                res[i] = WorkingProgram[immediate];
//                            }
//                            else
//                            {
//                                res[i] = immediate;
//                            }
//                        }
//                        catch (IndexOutOfRangeException e)
//                        {
//                            res[i] = 0;
//                        }
//                    }
//                    break;

//                case Operation.ReadInput:
//                    res = new long[2];
//                    while (Inputs.Count == 0) { }
//                    res[0] = Inputs.Dequeue();
//                    res[1] = WorkingProgram[PC + 1]; //writes to ram, must be in position mode
//                    break;

//                case Operation.HALT: return null;
//                default:
//                    throw new Exception("Not a valid Opcode");
//            }

//            return res;
//        }

//        private Mode[] GetModes(int instruction)
//        {
//            var res = new Mode[3];
//            res[0] = (Mode)(instruction % 2);
//            instruction /= 10;

//            res[1] = (Mode)(instruction % 2);
//            instruction /= 10;

//            res[2] = (Mode)(instruction % 2);
//            instruction /= 10;

//            return res;
//        }
//    }

//    enum Operation
//    {
//        Add = 1,
//        Multiply = 2,
//        ReadInput = 3,
//        WriteOutput = 4,
//        JumpTrue = 5,
//        JumpFalse = 6,
//        LessThan = 7,
//        TestEquals = 8,
//        HALT = 99
//    }

//    enum Mode
//    {
//        Position = 0,
//        Immediate = 1
//    }
//    public class OutputEventArgs : EventArgs
//    {
//        public long OutputValue { get; set; }
//    }

//    class Day7
//    {
//        private static List<long> returnedPowers = new List<long>();
//        private static List<long> outPutFromE = new List<long>();
//        static void Main(string[] args)
//        {
//            Stopwatch stopWatch = new Stopwatch();
//            stopWatch.Start();
//            List<long> commands = new List<long>();

//            string text = File.ReadAllText(@"Datasets\day7.txt");
//            foreach (string s in text.Split(','))
//            {
//                long parsed = 0;
//                if (long.TryParse(s, out parsed))
//                {
//                    commands.Add(parsed);
//                }
//                else
//                {
//                    throw new Exception($"Failed to parse '{s}' to a long");
//                }
//            }

//            List<long[]> firstPerms = GetPermutations(new List<long> { 0, 1, 2, 3, 4 });
//            List<long[]> secondPerms = GetPermutations(new List<long> { 5, 6, 7, 8, 9 });

//            Processor pcA = new Processor(commands.ToArray());
//            Processor pcB = new Processor(commands.ToArray());
//            Processor pcC = new Processor(commands.ToArray());
//            Processor pcD = new Processor(commands.ToArray());
//            Processor pcE = new Processor(commands.ToArray());
//            pcE.ProgramOutput += Pc_ProgramOutput;
//            pcE.ProgramFinish += PcE_ProgramFinish;


//            pcB.ListenToProcessor(pcA);
//            pcC.ListenToProcessor(pcB);
//            pcD.ListenToProcessor(pcC);
//            pcE.ListenToProcessor(pcD);

//            foreach (long[] perm in firstPerms)
//            {
//                pcA.AddInput(perm[0]);
//                pcA.AddInput(0);
//                pcB.AddInput(perm[1]);
//                pcC.AddInput(perm[2]);
//                pcD.AddInput(perm[3]);
//                pcE.AddInput(perm[4]);

//                pcA.ProccessProgram();
//                pcB.ProccessProgram();
//                pcC.ProccessProgram();
//                pcD.ProccessProgram();
//                pcE.ProccessProgram();
//            }

//            long max = returnedPowers.Max();
//            Console.WriteLine($"Part 1: {max}");

//            returnedPowers.Clear();


//            pcA.ListenToProcessor(pcE); //Allow E to loop back to A



//            foreach (long[] perm in secondPerms)
//            {
//                pcA.ResetInputs();
//                pcB.ResetInputs();
//                pcC.ResetInputs();
//                pcD.ResetInputs();
//                pcE.ResetInputs();

//                pcA.AddInput(perm[0]);
//                pcA.AddInput(0);
//                pcB.AddInput(perm[1]);
//                pcC.AddInput(perm[2]);
//                pcD.AddInput(perm[3]);
//                pcE.AddInput(perm[4]);

//                Thread a = new Thread(new ThreadStart(pcA.ProccessProgram));
//                Thread b = new Thread(new ThreadStart(pcB.ProccessProgram));
//                Thread c = new Thread(new ThreadStart(pcC.ProccessProgram));
//                Thread d = new Thread(new ThreadStart(pcD.ProccessProgram));
//                //Gonna start threading for part 2 because wow I hate myself.

//                a.Start();
//                b.Start();
//                c.Start();
//                d.Start();
//                pcE.ProccessProgram(); //run this on the main thread because sometimes it runs too fast to .join()
//            }

//            max = returnedPowers.Max();
//            Console.WriteLine($"Part 2: {max}");

//            returnedPowers.Clear();


//            stopWatch.Stop();
//            Console.WriteLine($"Time Taken: {stopWatch.Elapsed}");

//            Console.ReadLine();
//        }

//        private static void PcE_ProgramFinish(object sender, EventArgs e)
//        {
//            returnedPowers.Add(outPutFromE[outPutFromE.Count - 1]);
//            outPutFromE.Clear();

//        }

//        private static void Pc_ProgramOutput(object sender, OutputEventArgs e)
//        {
//            outPutFromE.Add(e.OutputValue);
//        }


//        private static List<long[]> GetPermutations(List<long> things, List<long> current = null)
//        {
//            List<long[]> res = new List<long[]>();
//            if (current == null)
//            {
//                current = new List<long>();
//            }
//            if (things.Count > 0)
//            {
//                foreach (long t in things)
//                {
//                    List<long> newP = new List<long>(current);
//                    newP.Add(t);

//                    List<long> newThings = new List<long>(things);
//                    newThings.Remove(t);
//                    res.AddRange(GetPermutations(newThings, newP));
//                }
//            }
//            else
//            {
//                res.Add(current.ToArray());
//            }

//            return res;
//        }
//    }
//}
