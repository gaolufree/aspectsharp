﻿using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace AspectSharp.Weaver
{
    public static class TypeReferenceExtensions
    {
        public static Instruction[] CreateLoadConstantInstruction(this TypeReference type, object value, ModuleDefinition module)
        {
            if (type.FullName == typeof(sbyte).FullName)
            {
                return new Instruction[] { Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)value) };
            }
            else if (type.FullName == typeof(byte).FullName)
            {
                sbyte cast = (sbyte)BitConverter.ToChar(BitConverter.GetBytes((byte)value), 0);

                return new Instruction[] { Instruction.Create(OpCodes.Ldc_I4_S, cast) };
            }
            else if (type.FullName == typeof(short).FullName)
            {
                int cast = (int)(short)value;

                return new Instruction[] { Instruction.Create(OpCodes.Ldc_I4, cast) };
            }
            else if (type.FullName == typeof(ushort).FullName)
            {
                int cast = (int)(ushort)value;

                return new Instruction[] { Instruction.Create(OpCodes.Ldc_I4, cast) };
            }
            else if (type.FullName == typeof(int).FullName)
            {
                int cast = (int)value;

                return new Instruction[] { Instruction.Create(OpCodes.Ldc_I4, cast) };
            }
            else if (type.FullName == typeof(uint).FullName)
            {
                int cast = BitConverter.ToInt32(BitConverter.GetBytes((uint)value), 0);

                return new Instruction[] { Instruction.Create(OpCodes.Ldc_I4, cast) };
            }
            else if (type.FullName == typeof(long).FullName)
            {
                return new Instruction[] { Instruction.Create(OpCodes.Ldc_I8, (long)value) };
            }
            else if (type.FullName == typeof(ulong).FullName)
            {
                long cast = BitConverter.ToInt64(BitConverter.GetBytes((ulong)value), 0);

                return new Instruction[] { Instruction.Create(OpCodes.Ldc_I8, cast) };
            }
            else if (type.FullName == typeof(float).FullName)
            {
                return new Instruction[] { Instruction.Create(OpCodes.Ldc_R4, (float)value) };
            }
            else if (type.FullName == typeof(double).FullName)
            {
                return new Instruction[] { Instruction.Create(OpCodes.Ldc_R8, (double)value) };
            }
            else if (type.FullName == typeof(char).FullName)
            {
                return new Instruction[] { Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)((char)value)) };
            }
            else if (type.FullName == typeof(bool).FullName)
            {
                return new Instruction[] { Instruction.Create((bool)value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0) };
            }
            else if (type.FullName == typeof(string).FullName)
            {
                return new Instruction[] { Instruction.Create(OpCodes.Ldstr, (string)value) };
            }
            else if (type.FullName == typeof(Type).FullName)
            {
                TypeReference type1 = module.Import(typeof(Type));
                TypeReference type2 = module.Import((TypeReference)value);
                MethodReference method = module.Import(type1.GetMethodDefinition(n => n.Name == "GetTypeFromHandle"));

                return new Instruction[]
                {
                    Instruction.Create(OpCodes.Ldtoken, module.Import((TypeReference)value)),
                    Instruction.Create(OpCodes.Call, method)
                };
            }
            else
            {
                throw new NotSupportedException(string.Format("Type '{0}' cannot be a constant value.", type.FullName));
            }
        }

        public static bool IsSubClassOf(this TypeReference type, TypeReference baseType)
        {
            if (type == null)
                return false;

            if (type.FullName == baseType.FullName)
                return true;

            return IsSubClassOf(type.Resolve().BaseType, baseType);
        }

        public static MethodDefinition GetMethodDefinition(this TypeReference type, Func<MethodDefinition, bool> methodSelector)
        {
            TypeDefinition typeResolved = type.Resolve();
            MethodDefinition setMethod = typeResolved.Methods.SingleOrDefault(methodSelector);

            if (setMethod != null)
            {
                return setMethod;
            }
            else if (typeResolved.BaseType == null)
            {
                return null;
            }
            else
            {
                return GetMethodDefinition(typeResolved.BaseType, methodSelector);
            }
        }
    }
}
