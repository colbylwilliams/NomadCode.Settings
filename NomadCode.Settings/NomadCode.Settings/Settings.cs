using System;

#if __IOS__
using System.IO;
using Foundation;
using static Foundation.NSUserDefaults;
#else
using Android.App;
using Android.Preferences;
#endif

namespace NomadCode.Settings
{
	public static class Settings
	{
		#region consts

#if __IOS__

		const string _settingsDir = "Settings";

		const string _bundleName = "bundle";

		const string _rootPlist = "Root.plist";

		const string _key = "Key";

		const string _defaultValue = "DefaultValue";

		const string _preferenceSpecifiers = "PreferenceSpecifiers";

#else

		const string zero = "0";

#endif

		#endregion

		#region utilities

#if __IOS__

		public static void RegisterDefaultSettings ()
		{
			var path = Path.Combine (NSBundle.MainBundle.PathForResource (_settingsDir, _bundleName), _rootPlist);

			using (NSString keyString = new NSString (_key), defaultString = new NSString (_defaultValue), preferenceSpecifiers = new NSString (_preferenceSpecifiers))
			using (var settings = NSDictionary.FromFile (path))
			using (var preferences = (NSArray)settings.ValueForKey (preferenceSpecifiers))
			using (var registrationDictionary = new NSMutableDictionary ())
			{
				for (nuint i = 0; i < preferences.Count; i++)
					using (var prefSpecification = preferences.GetItem<NSDictionary> (i))
					using (var key = (NSString)prefSpecification.ValueForKey (keyString))
						if (key != null)
							using (var def = prefSpecification.ValueForKey (defaultString))
								if (def != null)
									registrationDictionary.SetValueForKey (def, key);

				StandardUserDefaults.RegisterDefaults (registrationDictionary);

				Synchronize ();
			}
		}


		public static void Synchronize () => StandardUserDefaults.Synchronize ();


		public static void SetSetting (string key, string value) => StandardUserDefaults.SetString (value, key);


		public static void SetSetting (string key, bool value) => StandardUserDefaults.SetBool (value, key);


		public static void SetSetting (string key, int value) => StandardUserDefaults.SetInt (value, key);


		public static void SetSetting (string key, DateTime value) => SetSetting (key, value.ToString ());


		public static int Int32ForKey (string key) => Convert.ToInt32 (StandardUserDefaults.IntForKey (key));


		public static bool BoolForKey (string key) => StandardUserDefaults.BoolForKey (key);


		public static string StringForKey (string key) => StandardUserDefaults.StringForKey (key);


		public static DateTime DateTimeForKey (string key)
		{
			DateTime outDateTime;

			return DateTime.TryParse (StandardUserDefaults.StringForKey (key), out outDateTime) ? outDateTime : DateTime.MinValue;
		}

#else

		public static void RegisterDefaultSettings () { }


		public static void SetSetting (string key, string value)
		{
			using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (Application.Context))
			using (var sharedPreferencesEditor = sharedPreferences.Edit ())
			{
				sharedPreferencesEditor.PutString (key, value);
				sharedPreferencesEditor.Commit ();
			}
		}


		public static void SetSetting (string key, bool value)
		{
			using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (Application.Context))
			using (var sharedPreferencesEditor = sharedPreferences.Edit ())
			{
				sharedPreferencesEditor.PutBoolean (key, value);
				sharedPreferencesEditor.Commit ();
			}
		}


		public static void SetSetting (string key, int value, bool asString = false)
		{
			using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (Application.Context))
			using (var sharedPreferencesEditor = sharedPreferences.Edit ())
			{
				if (asString)
				{
					sharedPreferencesEditor.PutString (key, value.ToString ());
				}
				else
				{
					sharedPreferencesEditor.PutInt (key, value);
				}
				sharedPreferencesEditor.Commit ();
			}
		}


		public static void SetSetting (string key, DateTime value)
		{
			using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (Application.Context))
			using (var sharedPreferencesEditor = sharedPreferences.Edit ())
			{
				sharedPreferencesEditor.PutString (key, value.ToString ());
				sharedPreferencesEditor.Commit ();
			}
		}



		public static int Int32ForKey (string key, bool fromString = false)
		{
			using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (Application.Context))
			{
				if (fromString)
				{
					return int.Parse (sharedPreferences.GetString (key, zero));
				}
				else
				{
					return Convert.ToInt32 (sharedPreferences.GetInt (key, 0));
				}
			}
		}


		public static bool BoolForKey (string key)
		{
			using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (Application.Context))
				return sharedPreferences.GetBoolean (key, false);
		}


		public static string StringForKey (string key)
		{
			using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (Application.Context))
				return sharedPreferences.GetString (key, string.Empty);
		}


		public static DateTime DateTimeForKey (string key)
		{
			DateTime outDateTime;

			return DateTime.TryParse (StringForKey (key), out outDateTime) ? outDateTime : DateTime.MinValue;
		}

#endif

		#endregion

	}
}
