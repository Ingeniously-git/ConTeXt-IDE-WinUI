﻿
using CodeEditorControl_WinUI;
using ConTeXt_IDE.Helpers;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI;

namespace ConTeXt_IDE.Models
{
	public class Settings : Helpers.Bindable
	{
		public static Settings FromJson(string json) => JsonConvert.DeserializeObject<Settings>(json);

		[JsonIgnore]
		public static Settings Default { get; internal set; } = GetSettings();

		public static Settings RestoreSettings()
		{
			try
			{
				string file = "settings.json";
				var storageFolder = ApplicationData.Current.LocalFolder;
				string settingsPath = Path.Combine(storageFolder.Path, file);
				if (File.Exists(settingsPath))
				{
					File.Delete(settingsPath);
				}
				return GetSettings();
			}
			catch
			{
				return null;
			}
		}

		private static Settings GetSettings()
		{
			try
			{
				string file = "settings.json";
				var storageFolder = ApplicationData.Current.LocalFolder;
				string settingsPath = Path.Combine(storageFolder.Path, file);
				Settings settings;

				if (!File.Exists(settingsPath))
				{
					settings = new Settings();
					string json = settings.ToJson();
					File.WriteAllText(settingsPath, json);
				}
				else
				{
					string json = File.ReadAllText(settingsPath);
					Debug.WriteLine(json);
					settings = FromJson(json);
				}

				settings.PropertyChanged += (o, a) =>
				{
					string json = settings.ToJson();
					File.WriteAllText(settingsPath, json);
				};

				settings.ProjectList.CollectionChanged += (o, a) =>
				{
					string json = settings.ToJson();
					File.WriteAllText(settingsPath, json);
				};

				if (settings.CommandFavorites.Count == 0)
				{
					settings.CommandFavorites =
					new ObservableCollection<CommandFavorite>()
					{ 
					new(@"\startsection"), new(@"\startalignment"), new(@"\setuplayout"), new(@"\setuppapersize"), new(@"\starttext"), new(@"\startfrontmatter"), new(@"\startbodymatter"), new(@"\startappendices"), new(@"\startbackmatter"),
					new(@"\startframed"), new(@"\setupbodyfont"), new(@"\setupfooter"), new(@"\setupheader"),  new(@"\setuphead"),  new(@"\setupcaptions"), new(@"\setupcombinations"), new(@"\setupinteraction"), new(@"\placebookmarks"), new(@"\setuplist"), new(@"\environment"),  new(@"\startenvironment"), new(@"\product"),  new(@"\startproduct"), new(@"\component"),  new(@"\startcomponent"), new(@"\cite"), new(@"\setupTABLE"),
					};
				}

				settings.CommandFavorites.CollectionChanged += (o, a) =>
				{
					string json = settings.ToJson();
					File.WriteAllText(settingsPath, json);
				};

				if (settings.HelpItemList.Count == 0)
				{
					settings.HelpItemList =
					new ObservableCollection<HelpItem>()
									{
																												new HelpItem() { ID = "Modes", Title = "ConTeXt Modes", Text = "Select any number of modes. They will activate the corresponding \n'\\startmode[<ModeName>] ... \\stopmode'\n environments.", Shown = false },
																												new HelpItem() { ID = "Environments", Title = "ConTeXt Environments", Text = "Select any number of environments (usually one). Use this compiler parameter *instead* of the corresponding \n'\\environment[<EnvironmentName>]'\n commands.", Shown = false },
																												new HelpItem() { ID = "AddProject", Title = "Add a Project", Text = "Click this button to open an existing project folder or to create a new project folder from a template.", Shown = false },
									};
				}
				if (settings.TokenColorDefinitions.Count == 0)
				{
					settings.TokenColorDefinitions = new() {
						new() { Token = Token.Normal, Color = Color.FromArgb(255, 220, 220, 220) },
						new() { Token = Token.Comment, Color = Color.FromArgb(255, 30, 180, 40) },

						new() { Token = Token.Command, Color = Color.FromArgb(255, 40, 120, 240) },
						new() { Token = Token.Function, Color = Color.FromArgb(255, 120, 110, 220) },
						new() { Token = Token.Special, Color = Color.FromArgb(255, 120, 110, 220) },
						new() { Token = Token.Environment, Color = Color.FromArgb(255, 50, 190, 150) },
						new() { Token = Token.Primitive, Color = Color.FromArgb(255, 230, 60, 30) },
						new() { Token = Token.Style, Color = Color.FromArgb(255, 220, 50, 150) },
						new() { Token = Token.Array, Color = Color.FromArgb(255, 200, 100, 80) },

						new() { Token = Token.Key, Color = Color.FromArgb(255, 140, 210, 150) },

						
						new() { Token = Token.Reference, Color = Color.FromArgb(255, 180, 140, 40) },
						new() { Token = Token.Math, Color = Color.FromArgb(255, 220, 160, 60) },

						new() { Token = Token.Symbol, Color = Color.FromArgb(255, 140, 200, 240) },
						new() { Token = Token.Bracket, Color = Color.FromArgb(255, 120, 200, 220) },
						new() { Token = Token.Number, Color = Color.FromArgb(255, 180, 220, 180) },

						new() { Token = Token.Keyword, Color = Color.FromArgb(255, 40, 120, 240) },
						new() { Token = Token.String, Color = Color.FromArgb(255, 235, 120, 70) },
						
					};
				}

				if (settings.ContextModules.Count == 0)
				{
					settings.ContextModules = new ObservableCollection<ContextModule>() {
																								new ContextModule() { IsInstalled = false, Name = "filter", Description = "Process contents of a start-stop environment through an external program (Installed Pandoc needs to be in PATH!)", URL = @"https://modules.contextgarden.net/dl/t-filter.zip", Type = ContextModuleType.TDSArchive},
																								new ContextModule() { IsInstalled = false, Name = "gnuplot", Description = "Inclusion of Gnuplot graphs in ConTeXt (Installed Gnuplot needs to be in PATH!)", URL = @"https://mirrors.ctan.org/macros/context/contrib/context-gnuplot.zip", Type = ContextModuleType.Archive, ArchiveFolderPath = @"context-gnuplot\"},
																								new ContextModule() { IsInstalled = false, Name = "letter", Description = "Package for writing letters", URL = @"https://mirrors.ctan.org/macros/context/contrib/context-letter.zip", Type = ContextModuleType.Archive, ArchiveFolderPath = @"context-letter\"},
																								new ContextModule() { IsInstalled = true, Name = "pgf", Description = "Create PostScript and PDF graphics in TeX", URL = @"http://mirrors.ctan.org/install/graphics/pgf/base/pgf.tds.zip", Type = ContextModuleType.TDSArchive},
																								new ContextModule() { IsInstalled = true, Name = "pgfplots", Description = "Create normal/logarithmic plots in two and three dimensions", URL = @"http://mirrors.ctan.org/install/graphics/pgf/contrib/pgfplots.tds.zip", Type = ContextModuleType.TDSArchive},
																				};
				}

				return settings;
			}
			catch (Exception ex)
			{
				//App.VM.Log("Exception on getting Settings: "+ex.Message);

				return RestoreSettings();
			}
		}

		public bool AutoOpenLOG { get => Get(false); set => Set(value); }
		public bool AutoOpenLOGOnlyOnError { get => Get(true); set => Set(value); }
		public bool AutoOpenPDF { get => Get(true); set => Set(value); }
		public bool DistributionInstalled { get => Get(false); set => Set(value); }
		public bool EvergreenInstalled { get => Get(false); set => Set(value); }
		public bool FirstStart { get => Get(true); set => Set(value); }
		public bool HelpPDFInInternalViewer { get => Get(false); set => Set(value); }
		public bool InternalViewer { get => Get(true); set => Set(value); }
		public bool MultiInstance { get => Get(false); set => Set(value); }
		public bool ShowLog { get => Get(false); set => Set(value); }
		public bool ShowCompilerOutput { get => Get(false); set => Set(value); }
		public bool ShowOutline { get => Get(true); set => Set(value); }
		public bool ShowProjectPane { get => Get(true); set => Set(value); }
		public bool ShowCommandReference { get => Get(false); set => Set(value); }
		public bool StartWithLastActiveProject { get => Get(true); set => Set(value); }
		public bool StartWithLastOpenFiles { get => Get(false); set => Set(value); }
		public bool SuggestArguments { get => Get(true); set => Set(value); }
		public bool SuggestCommands { get => Get(true); set => Set(value); }
		public bool SuggestFontSwitches { get => Get(true); set => Set(value); }
		public bool SuggestPrimitives { get => Get(true); set => Set(value); }
		public bool SuggestStartStop { get => Get(true); set => Set(value); }

		public bool TextWrapping { get => Get(false); set => Set(value); }
		public bool LineNumbers { get => Get(true); set => Set(value); }
		public bool LineMarkers { get => Get(true); set => Set(value); }
		public bool ScrollbarMarkers { get => Get(true); set => Set(value); }
		public bool CodeFolding { get => Get(false); set => Set(value); }
		public bool ControlCharacters { get => Get(false); set => Set(value); }
		public bool FilterFavorites { get => Get(false); set => Set(value); }

		public string AccentColor { get => Get("Default"); set { Set(value); } }
		public string ContextVersion { get => Get(""); set { Set(value); } }
		public string ContextDistributionPath { get => Get(ApplicationData.Current.LocalFolder.Path); set => Set(value); }
		public string ContextDownloadLink { get => Get(@"http://lmtx.pragma-ade.nl/install-lmtx/context-mswin.zip"); set => Set(value); }
		public string LastActiveProject { get => Get(""); set => Set(value); }
		public string NavigationViewPaneMode { get => Get("Auto"); set => Set(value); }
		public string PackageID { get => Get(Package.Current.Id.FamilyName); set => Set(value); }
		public int FontSize { get => Get(14); set => Set(value); }
		public int TabLength { get => Get(2); set => Set(value); }
		public string Theme
		{
			get => Get("Default"); set
			{
				Set(value);

				//if (App.MainPage != null)
				//    App.MainPage.SetColor(null, (ElementTheme)Enum.Parse(typeof(ElementTheme),value), false); ;
			}
		}



		public List<CommandGroup> CommandGroups { get => Get(new List<CommandGroup>()); set => Set(value); }

		public ObservableCollection<CommandFavorite> CommandFavorites { get => Get(new ObservableCollection<CommandFavorite>()); set => Set(value); }

		public ObservableCollection<ContextModule> ContextModules { get => Get(new ObservableCollection<ContextModule>()); set => Set(value); }

		public ObservableCollection<Project> ProjectList { get => Get(new ObservableCollection<Project>()); set => Set(value); }

		public ObservableCollection<HelpItem> HelpItemList { get => Get(new ObservableCollection<HelpItem>()); set => Set(value); }
		public ObservableCollection<TokenDefinition> TokenColorDefinitions { get => Get(new ObservableCollection<TokenDefinition>()
		{
			

		}); set => Set(value); }

		[Newtonsoft.Json.JsonIgnore]
		public string[] ThemeOption => Enum.GetNames<ElementTheme>();
	}

	public static class Serialize
	{
		public static string ToJson(this Settings self) => JsonConvert.SerializeObject(self, Formatting.Indented);
	}

	public static class SettingsExtensions
	{
		public static void SaveSettings(this Settings settings)
		{
			string file = "settings.json";
			var storageFolder = ApplicationData.Current.LocalFolder;
			string settingsPath = Path.Combine(storageFolder.Path, file);
			string json = settings.ToJson();
			File.WriteAllText(settingsPath, json);
		}
	}
}