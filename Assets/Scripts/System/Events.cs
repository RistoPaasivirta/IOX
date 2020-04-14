using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventInteger : UnityEvent<int> { }
[Serializable]
public class UnityEventFloat : UnityEvent<float> { }
[Serializable]
public class UnityEventString : UnityEvent<string> { }
[Serializable]
public class UnityEventBool : UnityEvent<bool> { }
[Serializable]
public class UnityEventGameObject : UnityEvent<GameObject> { }
[Serializable]
public class UnityEventSprite : UnityEvent<Sprite> { }
[Serializable]
public class UnityEventVector2 : UnityEvent<Vector2> { }
[Serializable]
public class UnityEventVector3 : UnityEvent<Vector3> { }
[Serializable]
public class UnityEventVec2I : UnityEvent<Vec2I> { }
[Serializable]
public class UnityEventBytes : UnityEvent<byte[]> { }
[Serializable]
public class UnityEventAction : UnityEvent<UnityAction> { }
[Serializable]
public class UnityEventMusic : UnityEvent<MusicPlayer.MusicDef> { }
[Serializable]
public class UnityEventMonster : UnityEvent<MonsterCharacter> { }
[Serializable]
public class UnityEventInventoryItem : UnityEvent<InventoryGUIObject> { }