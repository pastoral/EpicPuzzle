///////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2015 AsNet Co., Ltd.
// All Rights Reserved. These instructions, statements, computer
// programs, and/or related material (collectively, the "Source")
// contain unpublished information proprietary to AsNet Co., Ltd
// which is protected by US federal copyright law and by
// international treaties. This Source may NOT be disclosed to
// third parties, or be copied or duplicated, in whole or in
// part, without the written consent of AsNet Co., Ltd.
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public enum FBActionType
{
	None = -1,
	Send,
	AskFor,
	Unknown
}

public enum FBObjectType
{
	None = -1,
	Coin,
	Mana,
//	InviteCoin,
//	InviteMana,
	Unknown
}

public class RequestData
{
	// The request id
	private string _id;

	private string _fromId;
	private string _fromName;
	private FBActionType _actionType;
	private FBObjectType _objectType;

	public string Id { get { return _id; } }
	public string FromId { get { return _fromId; } }
	public string FromName { get { return _fromName; } }
	public FBActionType ActionType { get { return _actionType; } }
	public FBObjectType ObjectType { get { return _objectType; } }

	public RequestData(string id, string fromId, string fromName, FBActionType actionType, FBObjectType objectType)
	{
		_id 		= id;
		_fromId 	= fromId;
		_fromName   = fromName;
		_actionType = actionType;
		_objectType = objectType;
	}

	public override string ToString()
	{
		return string.Format("[Id={0}, FromId={1}, FromName={2}, ActionType={3}, ObjectType={4}]", _id, _fromId, _fromName, _actionType, _objectType);
	}
}
