﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn020
{
    internal class ForAsync
    {
        //In thông tin, Task ID và thread ID đang chạy
        public static void PintInfo(string info) =>
        Console.WriteLine($"{info,10}    task:{Task.CurrentId,3}    " + $"thread: {Thread.CurrentThread.ManagedThreadId}");

        public static async void RunTask(int i)
        {
            PintInfo($"Start {i,3}");
            // Task.Delay(1000).Wait();          // Task dừng 1s - rồi mới chạy tiếp
            await Task.Delay(1);                 // Task.Delay là một async nên có thể await, RunTask chuyển điểm gọi nó tại đây 
            PintInfo($"Finish {i,3}");
        }

        public static void ParallelFor()
        {
            ParallelLoopResult result = Parallel.For(1, 20, RunTask);
            Console.WriteLine($"All task started: {result.IsCompleted}");
        }
    }
}
