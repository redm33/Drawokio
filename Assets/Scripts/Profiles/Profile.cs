using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class Profile 
{
	public string name = "Unknown";
	public string filename = "";

	public List<LevelInfo> levels = new List<LevelInfo>();

	public bool isMaster = false;

	public void ReportLevel( int level, int part, int pen, int pencil, int charcoal ) 
    {
		if( !isMaster )
			RemoveAfter( level, part );

		foreach( LevelInfo info in levels ) 
        {
			if( level == info.level && part == info.part ) {
				info.pen = pen;
				info.pencil = pencil;
				info.charcoal = charcoal;
				Save ();
				return;
			}
		}

		LevelInfo info2 = new LevelInfo();
		info2.level = level;
		info2.part = part;
		info2.pen = pen;
		info2.pencil = pencil;
		info2.charcoal = charcoal;
		levels.Add( info2 );

		Save ();
	}

	public void SortLevels() 
    {
		List<LevelInfo> temp = levels;
		levels = new List<LevelInfo>();

		while( temp.Count > 0 ) 
        {
			LevelInfo best = null;

			foreach( LevelInfo info in temp ) 
            {
				if( best == null )
					best = info;
				else if( info.level < best.level || info.part < best.part )
					best = info;
			}

			levels.Add( best );
			temp.Remove( best );
		}
	}

	public void RemoveAfter( int level, int part ) 
    {
		List<LevelInfo> temp = levels;
		levels = new List<LevelInfo>();

		foreach( LevelInfo info in temp ) 
        {
			if( info.level < level || info.part <= part )
				levels.Add( info );
		}
	}

	public void Reset() 
    {
		if( isMaster )
			return;

		levels = new List<LevelInfo>();
		levels.Add( new LevelInfo() );

		Save ();
	}

	public class LevelInfo 
    {
		public int level = 0;
		public int part = 0;
		public int pen = 0;
		public int pencil = 0;
		public int charcoal = 0;
	}

	public void Save() 
    {
		if( isMaster )
			return;

		Thread t = new Thread( Save_Thread );
		t.Start();
	}

	private void Save_Thread() 
    {
		
		StreamWriter writer;
		if( File.Exists( filename ) )
			writer = new StreamWriter( filename, false );
		else
			writer = File.CreateText( filename );
		using( writer ) {
			writer.WriteLine(name);
			foreach( LevelInfo info in levels ) 
            {
				writer.WriteLine( info.level.ToString() + "," + info.part.ToString() + "," + info.pen.ToString() + "," + info.pencil.ToString() + "," + info.charcoal.ToString() );
			}
		}
	}
}
