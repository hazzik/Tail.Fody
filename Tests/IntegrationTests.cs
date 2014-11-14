using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class IntegrationTests
{
    readonly Assembly assembly;
    readonly string beforeAssemblyPath;
    readonly string afterAssemblyPath;

    public IntegrationTests()
    {
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);

        var moduleDefinition = ModuleDefinition.ReadModule(afterAssemblyPath, new ReaderParameters());
        var weavingTask = new ModuleWeaver
        {
            ModuleDefinition = moduleDefinition,
        };

        weavingTask.Execute();
        moduleDefinition.Write(afterAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void FibStatic1()
    {
        Type t = assembly.GetType("ClassToTest", true);
        var method = t.GetMethod("FibStatic1");
        var result = method.Invoke(null, new object[] {int.MaxValue, 0});
    }

    [Test]
    public void FibStatic2()
    {
        Type t = assembly.GetType("ClassToTest", true);
        var method = t.GetMethod("FibStatic2");
        var result = method.Invoke(null, new object[] {int.MaxValue, 0});
    }


    [Test]
    public void Fib1()
    {
        Type t = assembly.GetType("ClassToTest", true);
        dynamic c = Activator.CreateInstance(t);
        c.Fib1(int.MaxValue, 0);
    }

    [Test]
    public void Fib2()
    {
        Type t = assembly.GetType("ClassToTest", true);
        dynamic c = Activator.CreateInstance(t);
        c.Fib2(int.MaxValue, 0);
    }

    [Test]
    public void FibVirtual1()
    {
        Type t = assembly.GetType("ClassToTest", true);
        dynamic c = Activator.CreateInstance(t);
        c.FibVirtual1(int.MaxValue, 0);
    }

    [Test]
    public void FibVirtual2()
    {
        Type t = assembly.GetType("ClassToTest", true);
        dynamic c = Activator.CreateInstance(t);
        c.FibVirtual2(int.MaxValue, 0);
    }

#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath, afterAssemblyPath);
    }
#endif

}