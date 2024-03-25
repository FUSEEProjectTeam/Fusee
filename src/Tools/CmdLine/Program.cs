using CommandLine;
using Fusee.Tools.CmdLine.Verbs;
using System;

namespace Fusee.Tools.CmdLine
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Install, Pack, Player, Publish, ProtoSchema>(args)
                .WithParsed<Pack>(pack =>
                {
                    pack.Run();
                })
                .WithParsed<Player>(async player =>
                {
                    await player.Run();
                })
                .WithParsed<ProtoSchema>(protoschema =>
                {
                    protoschema.Run();
                })
                .WithParsed<Publish>(publish =>
                {
                    publish.Run();
                })
                .WithParsed<Install>(install =>
                {
                    install.Run();
                })
                .WithNotParsed(errs =>
                {
                    Environment.Exit((int)ErrorCode.CommandLineSyntax);
                });
        }
    }

    internal enum ErrorCode : int
    {
        Success,
        CommandLineSyntax = -1,
        InputFile = -2,
        InputFormat = -3,
        OutputFile = -4,
        PlatformNotHandled = -5,
        InsufficentPrivileges = -6,
        CouldNotDownloadInputFile = -7,
        CouldNotWriteRegistry = -8,
        CouldNotFindFusee = -9,

        InternalError = -42
    }
}