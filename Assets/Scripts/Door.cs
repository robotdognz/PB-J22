using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Music;

public class Door : MonoBehaviour
{
    private Collider2D Col;

    [SerializeField] GameObject doorBody;
    [SerializeField] List<Room> parentRooms;

    public bool isLocked = false;

    private bool hasBeenOpened = false;

    public bool AutoClose = false;

    public Sprite[] Animation;
    public float Duration = 0.75f;

    public Sprite Master;

    public Texture2D Forest;
    public Texture2D MidnightDesert;
    public Texture2D Castle;
    public Texture2D Sewers;

    private AudioSource AS;
    public AudioClip OpenSound;
    public AudioClip CloseSound;

    private void Awake()
    {
        if (GetComponent<AudioSource>())
            AS = GetComponent<AudioSource>();
        else
            AS = gameObject.AddComponent<AudioSource>();

        AS.volume = 0.5f;

        Col = GetComponentInChildren<Collider2D>();
        Refresh();
    }

    public void Refresh()
    {
        MusicStarter Mus = MusicStarter.Instance;

        switch (Mus.DungeonType)
        {
            case DungeonType.Forest:
                Master.texture.LoadImage(Forest.EncodeToPNG());
                Master.texture.Apply();
                break;
            case DungeonType.MidnightDesert:
                Master.texture.LoadImage(MidnightDesert.EncodeToPNG());
                Master.texture.Apply();
                break;
            case DungeonType.Castle:
                Master.texture.LoadImage(Castle.EncodeToPNG());
                Master.texture.Apply();
                break;
            case DungeonType.Sewers:
                Master.texture.LoadImage(Sewers.EncodeToPNG());
                Master.texture.Apply();
                break;
        }
    }

    bool Anim = false;

    private IEnumerator OpenAnimation()
    {
        if (GetComponentInChildren<SpriteRenderer>().isVisible)
            AS.PlayOneShot(OpenSound);

        for (int I = 0; I < Animation.Length; I++)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = Animation[I];
            yield return new WaitForSeconds(Duration / Animation.Length);
        }
    }

    private IEnumerator CloseAnimation()
    {
        for (int I = Animation.Length - 1; I >= 0; I--)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = Animation[I];
            yield return new WaitForSeconds(Duration / Animation.Length);
        }

        if (GetComponentInChildren<SpriteRenderer>().isVisible)
            AS.PlayOneShot(CloseSound);
    }

    public void AddRooms(List<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            if (!parentRooms.Contains(room))
            {
                parentRooms.Add(room);
            }
        }
    }

    public void EndCombat()
    {
        isLocked = false;
        if (hasBeenOpened)
        {
            OpenDoor();
        }

    }

    public void CloseDoor()
    {
        if (Anim == true)
        {
            StopAllCoroutines();
            StartCoroutine(CloseAnimation());
            Anim = false;
        }
        Col.enabled = true;
    }

    public void OpenDoor()
    {
        if (isLocked)
        {
            return;
        }
        if (Anim == false)
        {
            StopAllCoroutines();
            StartCoroutine(OpenAnimation());
            Anim = true;
        }
        Col.enabled = false;
        hasBeenOpened = true;
    }
}
