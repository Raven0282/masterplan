#nullable disable

using Masterplan.Data;
using Masterplan.Tools;
using Masterplan.UI;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Masterplan
{
    static class Program
    {
        internal static bool fIsBeta = true;

        [STAThread]
        public static void Main(string[] args)
        {
            // Set up MessagePack options before any library loading occurs.
            SetupMessagePackResolvers();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                #region Bootstrapping
                Init_logging();

                SplashScreen = new ProgressScreen("Masterplan", 0);
                SplashScreen.CurrentAction = "Loading...";
                SplashScreen.Show();

                Load_preferences();
                Load_libraries();

                foreach (string arg in args)
                    Handle_arg(arg);

                SplashScreen.CurrentAction = "Starting Masterplan...";
                SplashScreen.Actions = 0;

                try
                {
                    MainForm main_form = new MainForm();
                    Application.Run(main_form);
                }
                catch (Exception ex)
                {
                    LogSystem.Trace(ex);
                }

                List<Form> forms = new List<Form>();
                foreach (Form form in Application.OpenForms)
                    forms.Add(form);
                foreach (Form form in forms)
                    form.Close();

                Save_preferences();

                if (IsBeta)
                    Check_for_logs();
            }
            catch (Exception ex)
            {
                LogSystem.Trace(ex);
            }
        }


        /// <summary>
        /// Sets up the MessagePack serializer options with custom and composite resolvers.
        /// This ensures System.Drawing.Color, System.Drawing.Bitmap, and polymorphic types are handled correctly.
        /// </summary>
        private static void SetupMessagePackResolvers()
        {
            try
            {
                // Combine custom formatters and standard resolvers into a single chain.
                var resolver = CompositeResolver.Create(
                    // 1. Custom formatters (highest priority)
                    new IMessagePackFormatter[] {
                        ColorFormatter.Instance,
                        BitmapFormatter.Instance 
                    },
                    // 2. Standard resolvers (order matters)
                    new IFormatterResolver[] {
                        NativeDateTimeResolver.Instance,
                        TypelessContractlessStandardResolver.Instance, // For interfaces, Bitmap, and dynamic types
                        StandardResolver.Instance,
                        ContractlessStandardResolver.Instance
                    }
                );

                MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithResolver(resolver);
            }
            catch (Exception ex)
            {
                LogSystem.Trace(ex);
            }
        }

        static void Init_logging()
        {
            string mp_dir = FileName.Directory(Application.ExecutablePath);
            string logdir = mp_dir + "Log" + Path.DirectorySeparatorChar;

            if (!Directory.Exists(logdir))
            {
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(logdir);
                    if (di == null)
                        throw new UnauthorizedAccessException();
                }
                catch
                {
                }
            }

            string logfile = logdir + DateTime.Now.Ticks + ".log";
            LogSystem.LogFile = logfile;
        }

        static void Load_preferences()
        {
            try
            {
                Assembly ass = Assembly.GetEntryAssembly();
                string root_dir = FileName.Directory(ass.Location);
                string filename = root_dir + "Preferences.xml";

                if (File.Exists(filename))
                {
                    SplashScreen.CurrentAction = "Loading user preferences";

                    Preferences prefs = Serialisation<Preferences>.Load(filename, SerialisationMode.XML);
                    if (prefs != null)
                        Session.Preferences = prefs;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Trace(ex);
            }
        }

        static void Save_preferences()
        {
            try
            {
                Assembly ass = Assembly.GetEntryAssembly();
                string root_dir = FileName.Directory(ass.Location);
                string filename = root_dir + "Preferences.xml";

                Serialisation<Preferences>.Save(filename, Session.Preferences, SerialisationMode.XML);
            }
            catch (Exception ex)
            {
                LogSystem.Trace(ex);
            }
        }

        static void Load_libraries()
        {
            try
            {
                SplashScreen.CurrentAction = "Loading libraries...";

                Assembly ass = Assembly.GetEntryAssembly();
                string root_dir = FileName.Directory(ass.Location);
                string lib_dir = root_dir + "Libraries" + Path.DirectorySeparatorChar;

                if (!Directory.Exists(lib_dir))
                    Directory.CreateDirectory(lib_dir);

                // Move libraries from root directory to Libraries folder
                string[] extensions = { "*.library", "*.mpxpl" };
                foreach (string ext in extensions)
                {
                    string[] files = Directory.GetFiles(root_dir, ext);
                    foreach (string filename in files)
                    {
                        try
                        {
                            string lib_name = lib_dir + Path.GetFileName(filename);
                            if (!File.Exists(lib_name))
                                File.Move(filename, lib_name);
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Trace(ex);
                        }
                    }
                }

                // Load all libraries via Session logic (handles MessagePack vs Binary prioritization)
                // Use a HashSet of unique base names to avoid double-loading
                HashSet<string> libraryBaseNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string file in Directory.GetFiles(lib_dir, "*.library"))
                    libraryBaseNames.Add(Path.GetFileNameWithoutExtension(file));
                foreach (string file in Directory.GetFiles(lib_dir, "*.mpxpl"))
                    libraryBaseNames.Add(Path.GetFileNameWithoutExtension(file));

                SplashScreen.Actions = libraryBaseNames.Count;

                foreach (string baseName in libraryBaseNames)
                {
                    // Pass the .library path; Session.LoadLibrary will prioritize .mpxpl if it exists
                    string filename = Path.Combine(lib_dir, baseName + ".library");
                    Session.LoadLibrary(filename);
                }

                Session.Libraries.Sort();
            }
            catch (Exception ex)
            {
                LogSystem.Trace(ex);
            }
        }

        static void Handle_arg(string arg)
        {
            try
            {
                if (arg == "-creaturestats")
                {
                    Run_creature_stats();
                }

                FileInfo fi = new FileInfo(arg);
                if (fi.Exists)
                {
                    SplashScreen.CurrentAction = "Loading project...";
                    SplashScreen.CurrentSubAction = FileName.Name(fi.Name);

                    // Unified prioritization logic (MessagePack vs Binary)
                    Project p = Session.LoadProject(arg);
                    if (p != null)
                    {
                        Session.CreateBackup(arg);
                        if (Session.CheckPassword(p))
                        {
                            Session.Project = p;
                            Session.FileName = arg;

                            p.Update();
                            p.SimplifyProjectLibrary();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        static void Check_for_logs()
        {
            string logfile = LogSystem.LogFile;
            if (string.IsNullOrEmpty(logfile) || !File.Exists(logfile))
                return;

            string logdir = FileName.Directory(logfile);
            Process.Start(logdir);
        }

        #endregion

        #region Stats

        private static void Run_creature_stats()
        {
            List<Creature> creatures = Session.Creatures;
            bool[] is_minion_options = { false, true };
            bool[] is_leader_options = { false, true };

            string datafile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Creatures.csv";
            StreamWriter sw = new StreamWriter(datafile);
            try
            {
                sw.Write("Level,Flag,Role,Minion,Leader,Tier,TierX,Creatures,Powers");
                foreach (string condition in Conditions.GetConditions())
                    sw.Write("," + condition);
                foreach (DamageType damage in Enum.GetValues(typeof(DamageType)))
                    sw.Write("," + damage);
                sw.WriteLine();

                for (int level = 1; level <= 40; ++level)
                {
                    foreach (bool is_minion in is_minion_options)
                    {
                        foreach (bool is_leader in is_leader_options)
                        {
                            foreach (RoleType role in Enum.GetValues(typeof(RoleType)))
                            {
                                foreach (RoleFlag flag in Enum.GetValues(typeof(RoleFlag)))
                                {
                                    List<Creature> list = Get_creatures(creatures, level, is_minion, is_leader, role, flag);
                                    List<CreaturePower> powers = new List<CreaturePower>();
                                    foreach (Creature c in list)
                                        powers.AddRange(c.CreaturePowers);
                                    if (powers.Count == 0)
                                        continue;

                                    string tier = (level < 11) ? "heroic" : (level < 21) ? "paragon" : "epic";
                                    string tierx = GetTierX(level);

                                    sw.Write(level + "," + flag + "," + role + "," + is_minion + "," + is_leader + "," + tier + "," + tierx + "," + list.Count + "," + powers.Count);

                                    foreach (string condition in Conditions.GetConditions())
                                    {
                                        int count = 0;
                                        string str = condition.ToLower();
                                        foreach (CreaturePower power in powers)
                                            if (power.Details.ToLower().Contains(str))
                                                count += 1;

                                        double pc = (powers.Count != 0) ? (double)count / powers.Count : 0;
                                        sw.Write("," + pc);
                                    }

                                    foreach (DamageType damage in Enum.GetValues(typeof(DamageType)))
                                    {
                                        int count = 0;
                                        string str = damage.ToString().ToLower();
                                        foreach (CreaturePower power in powers)
                                            if (power.Details.ToLower().Contains(str))
                                                count += 1;

                                        double pc = (powers.Count != 0) ? (double)count / powers.Count : 0;
                                        sw.Write("," + pc);
                                    }
                                    sw.WriteLine();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { LogSystem.Trace(ex); }
            finally { sw.Close(); }
        }

        private static string GetTierX(int level)
        {
            if (level < 4) return "early heroic";
            if (level < 8) return "mid heroic";
            if (level < 11) return "late heroic";
            if (level < 14) return "early paragon";
            if (level < 18) return "mid paragon";
            if (level < 21) return "late paragon";
            if (level < 24) return "early epic";
            if (level < 28) return "mid epic";
            if (level < 31) return "late epic";
            return "epic plus";
        }

        private static List<Creature> Get_creatures(List<Creature> creatures, int level, bool is_minion, bool is_leader, RoleType role, RoleFlag flag)
        {
            List<Creature> list = new List<Creature>();
            foreach (Creature c in creatures)
            {
                if (c.Level != level) continue;

                ComplexRole cr = c.Role as ComplexRole;
                Minion m = c.Role as Minion;

                if ((m != null) && (!m.HasRole)) continue;

                if ((m != null) != is_minion) continue;
                bool leader = (cr != null && cr.Leader);
                if (leader != is_leader) continue;

                RoleType rt = RoleType.Blaster;
                RoleFlag rf = RoleFlag.Standard;
                if (cr != null) { rt = cr.Type; rf = cr.Flag; }
                if (m != null) { rt = m.Type; rf = RoleFlag.Standard; }

                if (rt != role || rf != flag) continue;

                list.Add(c);
            }
            return list;
        }

        #endregion

        #region Security
        internal static bool IsBeta => fIsBeta;
        #endregion

        internal static void SetResolution(Image img)
        {
            if (img is Bitmap bmp)
            {
                try
                {
                    float x_dpi = Math.Min(bmp.HorizontalResolution, 96);
                    float y_dpi = Math.Min(bmp.VerticalResolution, 96);
                    bmp.SetResolution(x_dpi, y_dpi);
                }
                catch { }
            }
        }

        public static ProgressScreen SplashScreen = null;

        public static string ProjectFilter = "Masterplan Project|*.masterplan;*.mpxpm";
        public static string LibraryFilter = "Masterplan Library|*.library;*.mpxpl";
        public static string EncounterFilter = "Masterplan Encounter|*.encounter";
        public static string BackgroundFilter = "Masterplan Campaign Background|*.background";
        public static string EncyclopediaFilter = "Masterplan Campaign Encyclopedia|*.encyclopedia";
        public static string RulesFilter = "Masterplan Rules|*.crunch";

        public static string CreatureAndMonsterFilter = "Creatures|*.creature;*.monster";
        public static string MonsterFilter = "Adventure Tools Creatures|*.monster";
        public static string CreatureFilter = "Creatures|*.creature";
        public static string CreatureTemplateFilter = "Creature Template|*.creaturetemplate";
        public static string ThemeFilter = "Themes|*.theme";
        public static string CreatureTemplateAndThemeFilter = "Creature Templates and Themes|*.creaturetemplate;*.theme";
        public static string TrapFilter = "Traps|*.trap";
        public static string SkillChallengeFilter = "Skill Challenges|*.skillchallenge";
        public static string MagicItemFilter = "Magic Items|*.magicitem";
        public static string ArtifactFilter = "Artifacts|*.artifact";
        public static string MapTileFilter = "Map Tiles|*.maptile";
        public static string TerrainPowerFilter = "Terrain Powers|*.terrainpower";

        public static string HTMLFilter = "HTML File|*.htm";
        public static string ImageFilter = "Image File|*.bmp;*.jpg;*.jpeg;*.gif;*.png;*.tga";
        public static string PNGFilter = "Image File|*.png";
        public static string HeroAndPCFilter = "Hero File|*.hero";

    }
}
