using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

public class Pattern<T> : IEnumerable
{
    private readonly ICollection<Func<Instruction, T, bool>> matches = new List<Func<Instruction, T, bool>>(); 
    public Pattern<T> Add(Func<Instruction, bool> func)
    {
        Add((i, c) => func(i));
        return this;
    }

    public Pattern<T> Add(Func<Instruction, T, bool> func)
    {
        matches.Add(func);
        return this;
    }

    public bool Match(Instruction instruction)
    {
        var context = Activator.CreateInstance<T>();
        foreach (var match in matches)
        {
            if (instruction == null) return false;
            if (!match(instruction, context)) return false;
            instruction = instruction.Next;
        }
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return matches.GetEnumerator();
    }
}