using UnityEngine;

/*
 * 
 * 		  ----+-----------+------------------------------------
 * 			  |			  | ^								^
 * 			  |			  | | topPadding					|
 * 			  |			  | V								|
 * 		  ----+-----------+-----------					    |
 * 			  |    		  |									|
 * 			  |    		  |									|
 *        ----+-----------+-----------						|
 *            |  /  /   / |   	^                   		|
 *    		  | /  /   /  |    	|                   		|
 *            |  /  /   / |   	|                   		| mapHeight
 *     	      | /  /   /  |    	| viewHeight      			|
 *            |  /   /  / |    	|                   		|
 *  bottom    | /   /  /  |    	v                   		|   
 *    --------+-----------+---------------------------------|-- max2 = bottom
 *            |           | ^ 								|
 *            |           | | bottomPadding					|	Fix-Bottom
 *            |           | v								V
 *        ----+-----------+------------------------------------ max1 = max2 - bottomPadding
 *            |           |
 *        ----+-----------+------------------------------------ min1 = min2 + topPadding
 * 			  |			  | 
 * 			  |			  | 		    						Fix-Top
 * 			  |			  | 
 *        ----+-----------+------------------------------------ min2 = bottom - (mapHeight - viewHeight)
 *            |			  |										
 * 
 * y
 * ^
 * |
 * |
 * +------> x
 */
public class VerticalScroll
{
	enum FixBoundary
	{
		None,
		Top,
		Bottom
	};
	
	public int   trackSize       	= 10;
	public float topPadding    		= 10f * 0.01f;
	public float bottomPadding 		= 10f * 0.01f;
	
	public float scrollFriction 	= 0.9f;
	public float scrollDamping  	= 0.99f;
	public float scrollBoundaryAcc  = 1.5f  * 0.01f;
	public float scrollMinVelocity	= 1.0f  * 0.01f;
	public float scrollMaxVelocity	= 3.0f  * 0.01f;
	
	public float easing 			= 0.1f;
	public float easingMinVelocity  = 0.05f	* 0.01f;
	
	/** Native Scroll */
	
	// The friction constant
	private float _scrollFriction;
	
	// The damping constant
	private float _scrollDamping;
	
	// The boundary acceleration
	private float _scrollBoundaryAcc;
	
	// The minimum velocity
	private float _scrollMinVel;
	
	// The maximum velocity
	private float _scrollMaxVel;
	
	// The array of y-positions
	private float[] _scrollTracks;
	
	// The previous y-position
	private float _scrollPrev;
	
	// The current touch y-position
	private float _scrollTouch;
	
	// The velocity
	private float _scrollVel;
	
	// The type of fix boundary
	private FixBoundary _scrollFixBoundary;
	
	/** Easing */
	
	// The easing constant
	private float _easing;
	
	// The minimum velocity
	private float _easingMinVel;
	
	// The source y-position
	private float _easingSource;
	
	// The target y-position
	private float _easingTarget;
	
	/** */
	
	// The boundary
	private float _minPos1, _minPos2;
	private float _maxPos1, _maxPos2;
	
	// The current y-position
	private float _position;
	
	// The current friction
	private float _friction;
	
	// Is scrolling?
	private bool _isScrolling;
	
	// Is holding?
	private bool _isHolding;

	// Get current position
	public float Position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = Mathf.Clamp(value, _minPos1, _maxPos1);
		}
	}

	// Get scroll velocity
	public float ScrollVelocity
	{
		get
		{
			return _scrollVel;
		}
	}

	public void Construct(float bottom, float contentHeight, float viewHeight)
	{
		/** Native scroll */
		
		_scrollFriction    = scrollFriction;
		_scrollDamping     = scrollDamping;
		_scrollBoundaryAcc = scrollBoundaryAcc;
		_scrollMinVel      = scrollMinVelocity;
		_scrollMaxVel      = scrollMaxVelocity;

		_scrollTracks = new float[trackSize];

		_scrollPrev  = 0f;
		_scrollTouch = 0f;
		_scrollVel   = 0f;
		
		_scrollFixBoundary = FixBoundary.None;
		
		/** Easing */
		
		_easing       = easing;
		_easingMinVel = easingMinVelocity;
		
		_easingSource = 0f;
		_easingTarget = 0f;
		
		/** */

		_maxPos2 = bottom;
		_maxPos1 = _maxPos2 - bottomPadding;
		_minPos2 = bottom - (contentHeight - viewHeight);
		_minPos1 = _minPos2 + topPadding;
		
		_position = _maxPos1;
		_friction = 0f;
		
		_isScrolling = false;
		_isHolding   = false;
	}

	public void OnTouchPressed(Vector3 position)
	{
		/** Native scroll */
		
		// Clear array of y-positions
		for (int i = 0; i < trackSize; i++)
		{
			_scrollTracks[i] = 0;
		}
		
		// Set previous y-position
		_scrollPrev = position.y;
		
		// Save the current touch y-position
		_scrollTouch = position.y;
		
		// Set velocity to zero
		_scrollVel = 0f;
		
		// Set type of fix boundary
		_scrollFixBoundary = FixBoundary.None;
		
		/** Easing */
		
		// Set source y-position
		_easingSource = position.y;
		
		// Set target y-position
		_easingTarget = position.y;
		
		/** */
		
		// Sets friction
		_friction = _scrollFriction;
		
		//
		_isHolding = true;
	}
	
	public void OnTouchMoved(Vector3 position)
	{
		/** Native scroll */
		
		// Save the current touch y-position
		_scrollTouch = position.y;
		
		/** Easing */
		
		// Set target y-position
		_easingTarget = position.y;
	}
	
	public void OnTouchReleased(Vector3 position)
	{
		/** Native scroll */
		
		float total = 0f;
		
		for (int i = 0; i < trackSize; i++)
		{
			total += _scrollTracks[i];
		}
		
		// Calculate velocity
		_scrollVel = total / trackSize;
		
		//
		_isScrolling = true;
		_isHolding   = false;
	}
	
	public bool OnUpdate(float deltaTime)
	{
		// Holding
		if (_isHolding)
		{
			// Update for holding
			UpdateHolding(deltaTime);
			
			return false;
		}
		
		// Scrolling
		if (_isScrolling)
		{
			// Update for scrolling
			if (UpdateScrolling(deltaTime))
			{
				_isScrolling = false;
			}
			else
			{
				return false;
			}
		}
		
		return true;
	}
	
	void UpdateHolding(float deltaTime)
	{
		/** Update native scroll */
		
		// Calculate delta position
		float delta = _scrollTouch - _scrollPrev;
		
		// Save the current touch y-position
		_scrollPrev = _scrollTouch;
		
		//
		for (int i = 1; i < trackSize; i++)
		{
			_scrollTracks[i] = _scrollTracks[i - 1];
		}
		
		//
		_scrollTracks[0] = delta;
		
		/** Update easing */
		
		// Calculate velocity
		float velocity = (_easingTarget - _easingSource) * _easing;
		
		// Update source y-position
		if (Mathf.Abs(velocity) < _easingMinVel)
		{
			velocity = _easingTarget - _easingSource;
			_easingSource = _easingTarget;
		}
		else
		{
			_easingSource += velocity;
		}
		
		// Update y-position
		_position += velocity;
		
		// Clamp y-position
		if (_position < _minPos2)
		{
			_position = _minPos2;
		}
		else if (_position > _maxPos2)
		{
			_position = _maxPos2;
		}
	}
	
	bool UpdateScrolling(float deltaTime)
	{
		bool isFinished = true;
		
		/** Update native scroll */
		
		// Update y-velocity
		if (_scrollVel != 0f)
		{
			_scrollVel *= _friction;
			
			if (Mathf.Abs(_scrollVel) < _scrollMinVel)
			{
				_scrollVel = 0f;
			}
			else
			{
				// Update y-position
				_position += _scrollVel;
			}
			
			//
			bool isReverse = false;
			
			// Clamp y-position
			if (_position < _minPos2)
			{
				_position = _minPos2;
				isReverse  = true;
			}
			else if (_position <= _minPos1)
			{
				if (_friction >= _scrollFriction)
				{
					_friction *= _scrollDamping;
				}
			}
			else if (_position > _maxPos2)
			{
				_position = _maxPos2;
				isReverse  = true;
			}
			else if (_position >= _maxPos1)
			{
				if (_friction >= _scrollFriction)
				{
					_friction *= _scrollDamping;
				}
			}
			
			//
			if (isReverse)
			{
				// Clamp scroll velocity
				if (_scrollVel > 0)
				{
					if (_scrollVel > _scrollMaxVel)
					{
						_scrollVel = _scrollMaxVel;
					}
				}
				else
				{
					if (-_scrollVel > _scrollMaxVel)
					{
						_scrollVel = -_scrollMaxVel;
					}
				}
				
				_scrollVel = -_scrollVel;
				_friction  = _scrollFriction * _scrollDamping * 1.01f;
			}
			
			isFinished = false;
		}
		
		// Check boundary
		if (_scrollFixBoundary != FixBoundary.None)
		{
			if (_scrollFixBoundary == FixBoundary.Top)
			{
				if (_position > _minPos1)
				{
					_position  = _minPos1;
					_scrollVel = 0f;
					_scrollFixBoundary = FixBoundary.None;
				}
				else
				{
					_scrollVel += _scrollBoundaryAcc;
					isFinished = false;
				}
			}
			else if (_scrollFixBoundary == FixBoundary.Bottom)
			{
				if (_position < _maxPos1)
				{
					_position  = _maxPos1;
					_scrollVel = 0f;
					_scrollFixBoundary = FixBoundary.None;
				}
				else
				{
					_scrollVel -= _scrollBoundaryAcc;
					isFinished = false;
				}
			}
		}
		else
		{
			if (_position < _minPos1)
			{
				_scrollFixBoundary = FixBoundary.Top;
				isFinished = false;
			}
			else if (_position > _maxPos1)
			{
				_scrollFixBoundary = FixBoundary.Bottom;
				isFinished = false;
			}
		}
		
		return isFinished;
	}
}
