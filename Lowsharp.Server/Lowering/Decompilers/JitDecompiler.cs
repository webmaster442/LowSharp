using System.Reflection;
using System.Runtime.Loader;
using System.Text;

using Iced.Intel;

using Microsoft.IO;

namespace Lowsharp.Server.Lowering.Decompilers;

internal sealed class JitDecompiler : IDecompiler
{
    private readonly StringBuilder _buffer;
    private readonly IntelFormatter _formatter;

    public JitDecompiler()
    {
        _buffer = new StringBuilder(32 * 1024);
        _formatter = new IntelFormatter
        {
            Options =
            {
                UppercaseHex = true,
                HexPrefix = "0x",
                FirstOperandCharIndex = 10,
                DecimalDigitGroupSize = 3,
                DigitSeparator = "_",
            }
        };
    }

    private class CustomLoadContext : AssemblyLoadContext
    {
        public CustomLoadContext() : base(isCollectible: true)
        {
        }
    }

    public bool TryDecompile(RecyclableMemoryStream assemblyStream, RecyclableMemoryStream pdbStream, out string result)
    {
        try
        {
            CustomLoadContext customLoadContext = new CustomLoadContext();

            Assembly assembly = customLoadContext.LoadFromStream(assemblyStream);

            var types = assembly.GetTypes();
            _buffer.Clear();

            foreach (var type in types)
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    _buffer.AppendLine($"// Method: {type.FullName}.{method.Name}");
                    method.ToAsm(_buffer, _formatter);
                    _buffer.AppendLine();
                }
            }

            result = _buffer.ToString();
            _buffer.Clear();
            customLoadContext.Unload();

            return true;
        }
        catch (Exception ex)
        {
            result = ex.Message;
            return false;
        }
    }
}
