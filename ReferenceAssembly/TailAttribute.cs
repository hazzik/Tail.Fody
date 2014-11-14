using System;

/// <summary>
/// Marks the method should be Tailed. 
/// </summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TailAttribute : Attribute
{
}