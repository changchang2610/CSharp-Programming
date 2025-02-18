﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn021
{
    internal class Example_Inverse_Control
    {
        interface IClassB
        {
            public void ActionB();
        }
        interface IClassC
        {
            public void ActionC();
        }

        class ClassC : IClassC
        {
            public ClassC() => Console.WriteLine("ClassC is created");
            public void ActionC() => Console.WriteLine("Action in ClassC");
        }

        class ClassB : IClassB
        {
            IClassC c_dependency;
            public ClassB(IClassC classc)
            {
                c_dependency = classc;
                Console.WriteLine("ClassB is created");
            }
            public void ActionB()
            {
                Console.WriteLine("Action in ClassB");
                c_dependency.ActionC();
            }
        }


        class ClassA
        {
            IClassB b_dependency;
            public ClassA(IClassB classb)
            {
                b_dependency = classb;
                Console.WriteLine("ClassA is created");
            }
            public void ActionA()
            {
                Console.WriteLine("Action in ClassA");
                b_dependency.ActionB();
            }
        }

        // ĐỊnh nghĩa thêm: 
        class ClassC1 : IClassC
        {
            public ClassC1() => Console.WriteLine("ClassC1 is created");
            public void ActionC()
            {
                Console.WriteLine("Action in C1");
            }
        }

        class ClassB1 : IClassB
        {
            IClassC c_dependency;
            public ClassB1(IClassC classc)
            {
                c_dependency = classc;
                Console.WriteLine("ClassB1 is created");
            }
            public void ActionB()
            {
                Console.WriteLine("Action in B1");
                c_dependency.ActionC();
            }
        }
        public static void Test()
        {
            IClassC objectC = new ClassC();
            IClassB objectB = new ClassB(objectC);
            ClassA objectA = new ClassA(objectB);

            objectA.ActionA();


            IClassC objectCc = new ClassC1();            // new ClassC();
            IClassB objectBb = new ClassB1(objectCc);     // new ClassB();
            ClassA objectAa = new ClassA(objectBb);

            objectAa.ActionA();
        }
    }
}
