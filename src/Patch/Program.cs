// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.
// Calli patch based on (https://github.com/mellinoe/vk/blob/master/src/vk.rewrite/Program.cs)

using System;
using System.IO;
using System.Linq;
using CommandLine;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Patch
{
    public static class Program
    {
        public sealed class Options
        {
            [Option('i', "input", Default = null, Required = true, HelpText = "Input dll to patch.")]
            public string Input { get; set; }

            [Option('o', "output", Default = null, Required = false, HelpText = "Output path.")]
            public string Output { get; set; }
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => Run(opts));
        }

        public static void Run(Options opts)
        {
            var inputPath = opts.Input;
            var outputPath = opts.Output;
            bool copiedToTemp = false;
            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = inputPath;
                string copyPath = Path.GetTempFileName();
                File.Copy(inputPath, copyPath, overwrite: true);
                inputPath = copyPath;
                copiedToTemp = true;
            }

            try
            {
                Patch(inputPath, outputPath);
            }
            finally
            {
                if (copiedToTemp)
                {
                    File.Delete(inputPath);
                }
            }
        }

        private static TypeReference s_calliTargetRef;
        //private static MethodReference s_stringToHGlobalUtf8Ref;
        //private static MethodDefinition s_freeHGlobalRef;
        //private static TypeReference s_stringHandleRef;


        private static void Patch(string inputPath, string outputPath)
        {
            using (var assembly = AssemblyDefinition.ReadAssembly(inputPath))
            {
                var mainModule = assembly.Modules[0];

                s_calliTargetRef = mainModule.GetType("CalliAttribute");
                //var interopClass = mainModule.GetType("SharpVulkan.Interop");
                //s_stringToHGlobalUtf8Ref = interopClass.Methods.Single(md => md.Name == "StringToHGlobalUtf8");
                //s_freeHGlobalRef = interopClass.Methods.Single(md => md.Name == "FreeHGlobal");
                //s_stringHandleRef = mainModule.GetType("SharpVulkan.InteropStringHandle");

                foreach (var type in mainModule.Types)
                {
                    foreach (var method in type.Methods)
                    {
                        var calliAttribute = method.CustomAttributes.FirstOrDefault(attribute => attribute.AttributeType == s_calliTargetRef);
                        if (calliAttribute != null)
                        {
                            ProcessCalliMethod(method);
                            method.CustomAttributes.Remove(calliAttribute);
                        }
                    }
                }

                assembly.Write(outputPath);
            }
        }

        private static void ProcessCalliMethod(MethodDefinition method)
        {
            var il = method.Body.GetILProcessor();
            il.Body.Instructions.Clear();

            //var stringParams = new List<VariableDefinition>();
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                EmitLoadArgument(il, i);
                var parameterType = method.Parameters[i].ParameterType;
                if (parameterType.FullName == "System.String")
                {
                    throw new NotImplementedException("String are not supported");
                    //var variableDef = new VariableDefinition(s_stringHandleRef);
                    //method.Body.Variables.Add(variableDef);
                    //il.Emit(OpCodes.Call, s_stringToHGlobalUtf8Ref);
                    //il.Emit(OpCodes.Stloc, variableDef);
                    //il.Emit(OpCodes.Ldloc, variableDef);
                    //stringParams.Add(variableDef);
                }
                else if (parameterType.IsByReference)
                {
                    VariableDefinition byRefVariable = new VariableDefinition(new PinnedType(parameterType));
                    method.Body.Variables.Add(byRefVariable);
                    il.Emit(OpCodes.Stloc, byRefVariable);
                    il.Emit(OpCodes.Ldloc, byRefVariable);
                    il.Emit(OpCodes.Conv_I);
                }
            }

            string functionPtrName = method.Name + "_ptr";
            var field = method.DeclaringType.Fields.SingleOrDefault(fd => fd.Name == functionPtrName);
            if (field == null)
            {
                throw new InvalidOperationException("Can't find function pointer field for " + method.Name);
            }
            il.Emit(OpCodes.Ldsfld, field);

            var callSite = new CallSite(method.ReturnType)
            {
                CallingConvention = MethodCallingConvention.StdCall
            };
            foreach (var pd in method.Parameters)
            {
                TypeReference parameterType;
                if (pd.ParameterType.IsByReference)
                {
                    parameterType = new PointerType(pd.ParameterType.GetElementType());
                }
                else if (pd.ParameterType.FullName == "System.String")
                {
                    throw new NotImplementedException("String are not supported");
                    //parameterType = s_stringHandleRef;
                }
                else
                {
                    parameterType = pd.ParameterType;
                }

                var calliPD = new ParameterDefinition(pd.Name, pd.Attributes, parameterType);
                callSite.Parameters.Add(calliPD);
            }
            il.Emit(OpCodes.Calli, callSite);

            //foreach (var stringVar in stringParams)
            //{
            //    il.Emit(OpCodes.Ldloc, stringVar);
            //    il.Emit(OpCodes.Call, s_freeHGlobalRef);
            //}

            il.Emit(OpCodes.Ret);

            if (method.Body.Variables.Count > 0)
            {
                method.Body.InitLocals = true;
            }
        }

        private static void EmitLoadArgument(ILProcessor il, int i)
        {
            if (i == 0)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            else if (i == 1)
            {
                il.Emit(OpCodes.Ldarg_1);
            }
            else if (i == 2)
            {
                il.Emit(OpCodes.Ldarg_2);
            }
            else if (i == 3)
            {
                il.Emit(OpCodes.Ldarg_3);
            }
            else
            {
                il.Emit(OpCodes.Ldarg, i);
            }
        }
    }
}
