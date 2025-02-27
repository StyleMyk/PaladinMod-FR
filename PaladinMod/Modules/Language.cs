﻿//using R2API;
using Zio.FileSystems;
using System.Linq;
using System;

namespace PaladinMod.Modules {

    internal static class Languages {

        public static SubFileSystem fileSystem;

        internal static string languageRoot => System.IO.Path.Combine(Files.assemblyDir, "Language");
        
        internal static string TokensOutput = "";

        public static void PrintOutput(string preface = "") {
            PaladinPlugin.logger.LogWarning($"{preface}\n{{\n    strings:\n    {{{TokensOutput}\n    }}\n}}");
            TokensOutput = "";
        }

        public static void Add(string token, string text) {
            Languages.TokensOutput += $"\n    \"{token}\" : \"{text.Replace(Environment.NewLine, "\\n").Replace("\n", "\\n").Replace("\"", "\\\"")}\",";
        }

        public static void Init() {
            HookRegisterLanguageTokens();
        }

        private static void HookRegisterLanguageTokens() {
            On.RoR2.Language.SetFolders += fixme;
        }

        //Credits to Moffein for this credit
        //Credits to Anreol for this code
        private static void fixme(On.RoR2.Language.orig_SetFolders orig, RoR2.Language self, System.Collections.Generic.IEnumerable<string> newFolders) {
            if (System.IO.Directory.Exists(Languages.languageRoot)) {
                var dirs = System.IO.Directory.EnumerateDirectories(System.IO.Path.Combine(Languages.languageRoot), self.name);
                orig(self, newFolders.Union(dirs));
                return;
            }
            orig(self, newFolders);
        }
    }
}