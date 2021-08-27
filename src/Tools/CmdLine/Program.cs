using CommandLine;
using Fusee.Tools.CmdLine.Verbs;
using System;

namespace Fusee.Tools.CmdLine
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Install, Pack, Player, Publish, ProtoSchema, Verbs.Convert>(args)
                .WithParsed<Install>(install =>
                {
                    install.Run();
                })
                .WithParsed<Pack>(pack =>
                {
                    pack.Run();
                })
                .WithParsed<Player>(player =>
                {
                    player.Run();
                })
                .WithParsed<ProtoSchema>(protoschema =>
                {
                    protoschema.Run();
                })
                .WithParsed<Publish>(publish =>
                {
                    publish.Run();
                })
                .WithParsed<Verbs.Convert>(convert =>
                {
                    convert.Run();
                })
                .WithNotParsed(errs =>
                {
                    /*foreach (var error in errs)
                    {
                        Console.Error.WriteLine(error);
                    }
                    */
                    Environment.Exit((int)ErrorCode.CommandLineSyntax);
                });
        }
    }

    enum ErrorCode : int
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