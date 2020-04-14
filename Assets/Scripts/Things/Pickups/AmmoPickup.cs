using System.IO;
using UnityEngine;

public class AmmoPickup : MonoBehaviour, SaveGameObject
{
    [SerializeField] private AudioClip PickupSound = null;
    [SerializeField] private float Volume = .4f;
    [SerializeField] private float MinPitch = 0.9f;
    [SerializeField] private float MaxPitch = 1.1f;
    [SerializeField] private int AmmoType = 0;
    [SerializeField] private int AmmoAmount = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerControls pc = collision.GetComponent<PlayerControls>();
        MonsterCharacter mc = collision.GetComponent<MonsterCharacter>();

        if (pc == null || mc == null)
            return;

        if (mc.ammunition[AmmoType] >= mc.maxAmmo[AmmoType])
            return;

        mc.ammunition[AmmoType] = Mathf.Min(mc.ammunition[AmmoType] + AmmoAmount, mc.maxAmmo[AmmoType]);
        mc.OnAmmoPickup.Invoke(AmmoType);

        if (PickupSound != null)
            Sounds.Create2DSound(PickupSound, Sounds.MixerGroup.Effects, Volume, MinPitch, MaxPitch);

        Destroy(gameObject);
    }

    byte[] SaveGameObject.Serialize()
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);

        bw.Write(transform.position.x);
        bw.Write(transform.position.y);

        byte[] data = stream.ToArray();
        bw.Close();
        stream.Close();

        return data;
    }

    void SaveGameObject.Deserialize(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        transform.position = new Vector3(br.ReadSingle(), br.ReadSingle(), 0);

        br.Close();
        stream.Close();
    }

    void SaveGameObject.AfterCreated() { }

    private int _saveIndex = -1;
    int SaveGameObject.SaveIndex
    {
        get { return _saveIndex; }
        set { _saveIndex = value; }
    }

    private string _spawnName = "";
    string SaveGameObject.SpawnName
    {
        get { return _spawnName; }
        set { _spawnName = value; }
    }

    SaveObjectType SaveGameObject.ObjectType
    {
        get { return SaveObjectType.Pickup; }
    }
}
