using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Game/Ink/Drawer")]
public class Drawer : MonoBehaviour 
{
	public static Drawer instance = null;
    bool hasConnector = false;
	/**
	 * Generic
	 */

	void Awake() 
    {
		instance = this;
	}

	public Transform drawingParent;

	bool drawing = false;
	
	List<Ink> roots = new List<Ink>();

	Connector startConnector;
	Vector3 connectStart;
	Ink currentRoot;
	Ink lastNode;

	public void DestroyAll()
	{
		foreach( Transform child in drawingParent )
			Destroy ( child.gameObject );
	}
	
	void Update() 
    {
		// Cheatz Yo
		if( Input.GetButtonDown("Refill") ) 
			hasPencil = hasCharcoal = hasPen = true;

		StepMenu( Time.deltaTime );

		if( !CheckMenu() || !hasCurrentInk )
			return;

		if( GUIUtility.hotControl == 0 && Input.GetButton( "Draw" ) ) 
        {		
			RaycastHit hit;
			if( Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit, 1000, raycastMask ) ) 
            {
				
				bool isCanvas = ( hit.collider.gameObject.layer == DrawingCanvas.layer );
				bool isConnector = ( hit.collider.tag == "Connector" );

				if( drawing ) 
                {
					if( isCanvas ) 
                    {
                        DrawingCanvas canvas = hit.collider.GetComponent<DrawingCanvas>();

                        if (lastNode == null && startConnector == null)
                            PlaceInk(hit.point, canvas);

                        else /* lastNode isn't null */
                        {
                            Vector3 start = (lastNode != null ? lastNode.transform.position : connectStart);
                            Vector3 diff = (hit.point - start);
                            if (diff.sqrMagnitude > currentInk.distanceBetweenNodes * currentInk.distanceBetweenNodes)
                            {
                                float dist = diff.magnitude;
                                Vector3 dir = diff / dist;

                                Vector3 point = start;
                                while (dist > currentInk.distanceBetweenNodes)
                                {
                                    point += dir * currentInk.distanceBetweenNodes;
                                    PlaceInk(point, canvas);
                                    dist -= currentInk.distanceBetweenNodes;
                                }
                            }
                        }
					} 
                    else if( isConnector ) 
                    {
						if( lastNode != null && lastNode.ConnectTo( hit.collider.attachedRigidbody.GetComponent<Connector>() ) ) 
							lastNode = currentRoot = null;
						StopDrawing();
					} 
                    else 
                    {
						StopDrawing();
					}
					
				} 
                else /* if not drawing already */ 
                {
					DrawingCanvas canvas = hit.collider.GetComponent<DrawingCanvas>();
					
					if( isCanvas ) 
                    {
						//Level.current.paused = true;
						PlaceInk( hit.point, canvas );
						drawing = true;
					}/* else if( isConnector ) {
						//Level.current.paused = true;
						startConnector = hit.collider.GetComponent<Connector>();
						//drawing = true;
					}*/
				}
				
			} 
            else /* if raycast didn't hit anything */ 
				StopDrawing();
			
		} 
        else /* if draw button not pressed */ 
			StopDrawing();
	}

	float currentTimeout = 0;

	private void PlaceInk( Vector3 pos, DrawingCanvas canvas ) 
    {
		bool placeNew = true;

		if( currentInk.connects ) 
        {
			Collider[] hits = Physics.OverlapSphere( pos, currentInk.connectRadius, spherecastMask );
			if( hits.Length > 0 ) 
            {
				foreach( Collider hit in hits ) 
                {
                    if (hit.name != "Pencil(Clone)" && hit.tag == "Connector")
                        hasConnector = true;
                    if (hit.tag == "Connector")
                    {
                        Connector connector = hit.attachedRigidbody.GetComponent<Connector>();

                        if (lastNode == null)
                        {
                            startConnector = connector;
                            connectStart = pos;
                            placeNew = false;
                            break;
                        }
                        else if (lastNode.ConnectTo(connector))
                        {
                            startConnector = connector;
                            connectStart = pos;
                            placeNew = false;
                            lastNode = currentRoot = null;
                            break;
                        }
                    }
				}
			}
		}

		if( placeNew ) 
        {
			Ink node = Instantiate( currentInk, pos, canvas.transform.rotation ) as Ink;
            //Ink node = Instantiate(inkTypes[3], pos, canvas.transform.rotation) as Ink;
            if(node.rigidbody != null)
			    node.rigidbody.constraints = canvas.drawingConstraints;
			if( node.type == Connector.Type.CHARCOAL )
				node.rigidbody.constraints |= RigidbodyConstraints.FreezeRotation;

			if( currentRoot == null ) {
				node.transform.parent = drawingParent;
				currentRoot = node;
				roots.Add( node );

				node.timeoutRemaining = currentTimeout = node.baseTimeout;
			} 
            else 
				node.timeoutRemaining = ( currentTimeout += node.timeoutIncrement );

			if( lastNode != null ) 
				lastNode.AddChild( node );
						
            lastNode = node;

			node.OnCreated();
			
			if( startConnector != null )
				node.ConnectTo( startConnector );
			startConnector = null;

			if( !audio.isPlaying && node.drawingSound != null ) 
            {
				audio.clip = node.drawingSound;
				audio.Play ();
			}
		}
	}

	private void StopDrawing() 
    {
		if( !drawing )
			return;

		foreach( Ink root in roots ) 
        {
			root.OnDrawEnd();
		}
		
		drawing = false;
		roots = new List<Ink>();

        //if the pencil isn't conneted to anything, remove it very quickly from the world.
        Debug.Log(currentInkIndex);
        if (!hasConnector && currentInkIndex == 0)
        {
            float incrememnt = .02f;
            Ink last = lastNode;
            while(currentRoot.gameObject != last.gameObject)
            {
                last.timeoutRemaining = incrememnt;
                last.gameObject.GetComponent<Pencil>().climbable = false;
                last = last.gameObject.transform.parent.GetComponent<Pencil>();
                incrememnt += .02f;
            }
            Material[] mats = { last.gameObject.GetComponent<LineRenderer>().materials[1] };
            last.timeoutRemaining = incrememnt;
            last.gameObject.GetComponent<Pencil>().climbable = false;
            last.gameObject.GetComponent<LineRenderer>().materials = mats;
        }

        

		lastNode = currentRoot = null;
		startConnector = null;

        
		audio.Stop ();

        hasConnector = false;

	}

	/**
	 * Raycasting
	 */
	
	public int[] raycastLayers;
	private int raycastMask 
    {
		get 
        {
			if( raycastLayers == null )
				return 0;
			
			int ret = 0;
			foreach( int layer in raycastLayers ) 
            {
				ret += 1 << layer;
			}
			
			return ret;
		}
	}
	
	public int[] spherecastLayers;
	public int spherecastMask 
    {
		get 
        {
			if( spherecastLayers == null )
				return 0;
			
			int ret = 0;
			foreach( int layer in spherecastLayers )
				ret += 1 << layer;
			
			return ret;
		}
	}

	/**
	 * Get/Set the current ink type.
	 */

	public Ink[] inkTypes;
	public GUITexture[] inkButtons;

	public bool hasPencil = false;
	public bool hasCharcoal = false;
    public bool hasPen = false;

	public bool hasCurrentInk 
    {
		get 
        {
			return HasInk( currentInkIndex );
		}
	}
	public bool HasInk( int index ) 
    {
		switch( index ) 
        {
		case 0:
			return hasPencil;
		case 1:
			return hasCharcoal;
		default:
			return true;
		}
	}
	
	private int _currentInkIndex = 0;
	public int currentInkIndex 
    {
		get { return _currentInkIndex; }
		set 
        {
			_currentInkIndex = value;
			if( _currentInkIndex >= inkButtons.Length )
				_currentInkIndex = 0;
			else if( _currentInkIndex < 0 )
				_currentInkIndex = inkButtons.Length-1;
		}
	}
	
	public Ink currentInk 
    {
		get { return ( currentInkIndex >= 0 && currentInkIndex < inkTypes.Length ? inkTypes[currentInkIndex] : null ); }
		set 
        {
			for( int i = 0; i < inkTypes.Length; i++ ) 
            {
				if( inkTypes[i] == value ) 
                {
					currentInkIndex = i;
					return;
				}
			}
		}
	}
	
	/**
	 * Toolbox UI
	 */
	
	public Transform toolboxTransform;

	private bool _uiOpen = false;
	public bool uiOpen {
		get { return _uiOpen; }
		set 
        {
			if( _uiOpen == value )
				return;

			_uiOpen = value;

			Room.instance.paused = value;

			if( uiOpen ) {} 
            else 
				StopDrawing();
		}
	}
	public float menuSpeed = 1;
	float menuProgress = 0;

	public Vector3 closedToolboxPosition;
	public Vector3 openToolboxPosition;

	public Texture[] boxTextures;
	public GUITexture boxTexture;

	public GUITexture arrowTexture;

	bool CheckMenu() 
    {
		if( Input.GetButtonDown( "DrawMode" ) && ( Player.instance != null && !Player.instance.transformationController.in3D ) ) 
			uiOpen = !uiOpen;
		else if( Player.instance.transformationController.in3D )
			uiOpen = false;

		return uiOpen;
	}

	void StepMenu(float dt) 
    {

		if( uiOpen ) 
			menuProgress = Mathf.Min( 1, menuProgress + dt * menuSpeed );
	    else 
			menuProgress = Mathf.Max( 0, menuProgress - dt * menuSpeed );

		toolboxTransform.position = Vector3.Lerp( closedToolboxPosition, openToolboxPosition, menuProgress );

        if (hasPen)
            boxTexture.texture = boxTextures[3];
		else if( hasCharcoal )
			boxTexture.texture = boxTextures[2];
		else if( hasPencil )
			boxTexture.texture = boxTextures[1];
		else
			boxTexture.texture = boxTextures[0];

		/*Rect inset = arrowTexture.pixelInset;
		inset.y = -arrowTexture.GetScreenRect().height / 2 + 32;
		arrowTexture.pixelInset = inset;*/

		bool pressed = Input.GetMouseButtonDown(0);

		if( pressed && ( Player.instance != null && !Player.instance.transformationController.in3D ) && arrowTexture.GetScreenRect().Contains( Input.mousePosition ) )
			uiOpen = !uiOpen;

		for( int i = 0; i < inkButtons.Length; i++ ) 
        {
			if( !HasInk(i) ) 
            {
				inkButtons[i].gameObject.SetActive(false);
				continue;
			}

			bool on = inkButtons[i].GetScreenRect().Contains( Input.mousePosition );
			bool active = on || i == currentInkIndex;

			inkButtons[i].gameObject.SetActive(active);

			if( on && pressed )
				currentInkIndex = i;
		}
	}
}
