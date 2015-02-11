using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

[AddComponentMenu("Game/Profiles/Profile Manager")]
public class ProfileManager : MonoBehaviour {
	public static ProfileManager instance;

	public string directory = "Profiles/";

	public List<Profile> loadedProfiles = new List<Profile>();
	public Profile currentProfile = null;

	void Awake() {
		instance = this;

		if( !Directory.Exists( directory ) )
			Directory.CreateDirectory( directory );

		string[] files = Directory.GetFiles( directory, "*.save" );
		foreach( string filename in files ) {
			try {
				ProcessFile(filename);
			} catch( System.Exception ex ) {
				Debug.LogError( "Failed to load file " + filename );
				Debug.LogError( ex );
			}
		}
	}

	bool ProcessFile( string filename ) {
		if( !File.Exists( filename ) ) {
			Debug.LogError( "No file exists at " + filename );
			return false;
		}

		StreamReader reader = File.OpenText( filename );
		if( reader == null ) {
			Debug.LogError( "Failed to open file at " + filename );
			return false;
		}
		string line = "";

		Profile profile = new Profile();
		profile.filename = filename;

		if( (line = reader.ReadLine()) == null ) {
			Debug.LogError( "Invalid file at " + filename );
			return false;
		}

		profile.name = line.Trim();

		while( (line = reader.ReadLine()) != null ) {
			string[] parts = line.Split(',');
			if( parts.Length < 5 ) {
				Debug.LogError( "Invalid line found in " + filename + ": " + line );
				return false;
			}

			Profile.LevelInfo info = new Profile.LevelInfo();
			info.level = int.Parse( parts[0] );
			info.part = int.Parse( parts[1] );
			info.pen = int.Parse( parts[2] );
			info.pencil = int.Parse( parts[3] );
			info.charcoal = int.Parse( parts[4] );
			profile.levels.Add( info );
		}

		profile.SortLevels();
		if( profile.levels.Count == 0 )
			profile.Reset();

		loadedProfiles.Add ( profile );
		return true;
	}

	public Profile CreateProfile( string name ) {
		if( !IsNameValid(name) )
			return null;

		string path = directory + name + ".save";
		if( File.Exists( path ) ) {
			return null;
		}

		Profile profile = new Profile();
		profile.name = name;
		profile.filename = path;
		profile.levels.Add( new Profile.LevelInfo() );
		profile.Save();

		loadedProfiles.Add( profile );

		return profile;
	}

	public static bool IsNameValid( string name ) {
		if( name.Length == 0 )
			return false;

		for( int i = 0; i < name.Length; i++ ) {
			char cur = name[i];
			if( (cur < 'a' || cur > 'z') && (cur < 'A' || cur > 'Z') && (cur < '0' || cur > '9') )
				return false;
		}
		return true;
	}
}
